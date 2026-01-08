using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Baumeister.Generators.Building
{
    [Generator]
    public class BuilderSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsDerivedFromBuilderBase(s),
                    transform: static (ctx, _) => GetClassDeclaration(ctx))
                .Where(static m => m is not null);

            var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

            context.RegisterSourceOutput(compilationAndClasses, (spc, source) => Execute(source.Left, source.Right!, spc));
        }

        private static bool IsDerivedFromBuilderBase(SyntaxNode node)
        {
            return node is ClassDeclarationSyntax classDeclaration &&
                   classDeclaration.BaseList?.Types.Any(baseType =>
                       baseType.Type is GenericNameSyntax genericName &&
                       genericName.Identifier.Text == "BuilderBase") == true;
        }

        private static ClassDeclarationSyntax? GetClassDeclaration(GeneratorSyntaxContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;
            return classDeclaration;
        }

        private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
        {
            foreach (ClassDeclarationSyntax candidate in classes)
            {
                var className = candidate.Identifier.ValueText;
                
                StringBuilder sourceBuilder = GenerateNewClassesCodeFor(candidate);
                AddCodeForInitializationOfDefaultValues(className, sourceBuilder);
                AddNewMethod(className, sourceBuilder);
                AddWithMethods(candidate, compilation, sourceBuilder);
                AppendClassFooter(sourceBuilder);

                context.AddSource($"{className}Extensions.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }
               
        private static StringBuilder GenerateNewClassesCodeFor(ClassDeclarationSyntax candidate)
        {
            var className = candidate.Identifier.ValueText;
            var namespaceName = GetNamespace(candidate) ?? "Generated";

            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("#nullable enable");
            sourceBuilder.AppendLine("using System;");
            sourceBuilder.AppendLine("");
            sourceBuilder.AppendLine($"namespace {namespaceName}");
            sourceBuilder.AppendLine("{");
            sourceBuilder.AppendLine($"    public partial class {className}");
            sourceBuilder.AppendLine("    {");

            return sourceBuilder;
        }

        private static string? GetNamespace(ClassDeclarationSyntax candidate)
        {
            // 1. Block-scoped namespace (namespace A { ... })
            var blockNamespace = candidate
                .Ancestors()
                .OfType<BaseNamespaceDeclarationSyntax>()
                .FirstOrDefault();

            if (blockNamespace != null)
            {
                return blockNamespace.Name.ToString();
            }

            // 2. File-scoped namespace (namespace A.B;)
            var compilationUnit = candidate.SyntaxTree.GetRoot() as CompilationUnitSyntax;

            var fileNamespace = compilationUnit?
                .Members
                .OfType<FileScopedNamespaceDeclarationSyntax>()
                .FirstOrDefault();

            return fileNamespace?.Name.ToString();
        }

        private static void AddCodeForInitializationOfDefaultValues(string className, StringBuilder sourceBuilder)
        {
            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine($"        public {className}()");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine("            OnInitializeDefaults();");
            sourceBuilder.AppendLine("        }");
            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine("        partial void OnInitializeDefaults();");
        }

        private static void AddNewMethod(string builderTypeName, StringBuilder sourceBuilder)
        {
            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine($"        public static {builderTypeName} New()");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine($"            return new {builderTypeName}();");
            sourceBuilder.AppendLine("        }");
        }

        private static void AddWithMethods(ClassDeclarationSyntax candidate, Compilation compilation, StringBuilder sourceBuilder)
        {
            var className = candidate.Identifier.ValueText;
            var namedTypeSymbolToBuild = GetNamedTypeSymbolToCreateBuilderFor(candidate, compilation);
            if (namedTypeSymbolToBuild != null) 
            {
                var propertyWithMethodInfos = GetWithMethodInfosForProperties(namedTypeSymbolToBuild);
                var ctorWithMethodInfos = GetWithMethodInfosForCtors(namedTypeSymbolToBuild);
                var allWithMethodInfos = propertyWithMethodInfos.Concat(ctorWithMethodInfos).Distinct();

                foreach (var withMethodInfo in allWithMethodInfos)
                {
                    AddWithMethodFor(className, withMethodInfo.ParameterName, withMethodInfo.ParameterTypeName, sourceBuilder);
                }
            }
        }

        private static IEnumerable<WithMethodInfo> GetWithMethodInfosForProperties(INamedTypeSymbol namedTypeSymbolToBuild)
        {
            foreach (var propertySymbol in namedTypeSymbolToBuild.GetMembers().OfType<IPropertySymbol>())
            {
                if (IsPropertySymbolValidForBuilder(propertySymbol))
                {
                    yield return CreateWithMethodInfoFor(propertySymbol);
                }
            }
        }

        private static IEnumerable<WithMethodInfo> GetWithMethodInfosForCtors(INamedTypeSymbol namedTypeSymbolToBuild)
        {
            foreach (var constructor in namedTypeSymbolToBuild.InstanceConstructors)
            {
                foreach (var parameterSymbol in constructor.Parameters)
                {
                    yield return CreateWithMethodInfoFor(parameterSymbol);
                }
            }
        }

        private static bool IsPropertySymbolValidForBuilder(IPropertySymbol propertySymbol)
        {
            return 
                propertySymbol.Name != "EqualityContract" && 
                propertySymbol.IsStatic == false &&
                propertySymbol.SetMethod != null;
        }

        private static INamedTypeSymbol? GetNamedTypeSymbolToCreateBuilderFor(ClassDeclarationSyntax candidate, Compilation compilation)
        {
            var semanticModel = compilation.GetSemanticModel(candidate.SyntaxTree);
            var baseType = candidate.BaseList?.Types
                .Select(baseType => semanticModel.GetTypeInfo(baseType.Type).Type)
                .OfType<INamedTypeSymbol>()
                .FirstOrDefault(type => type.Name == "BuilderBase");

            if (baseType != null && baseType.TypeArguments.Length == 1)
            {
                if (baseType.TypeArguments[0] is INamedTypeSymbol typeToBuild)
                {
                    return typeToBuild;
                }
            }

            return null;
        }

        private static WithMethodInfo CreateWithMethodInfoFor(IParameterSymbol parameterSymbol) => new(parameterSymbol.Type.ToString(), parameterSymbol.Name);

        private static WithMethodInfo CreateWithMethodInfoFor(IPropertySymbol propertySymbol) => new(propertySymbol.Type.ToString(), propertySymbol.Name);

        private static void AddWithMethodFor(string builderTypeName, string name, string typeName, StringBuilder sourceBuilder)
        {
                        sourceBuilder.AppendLine();
            sourceBuilder.AppendLine($"        public {builderTypeName} With{name.FirstCharToUpper()}({typeName} {name.ToLower(CultureInfo.InvariantCulture)})");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine($"            this.With(\"{name.FirstCharToUpper()}\", {name.ToLower(CultureInfo.InvariantCulture)});");
            sourceBuilder.AppendLine("            return this;");
            sourceBuilder.AppendLine("        }");
        }

        private static void AppendClassFooter(StringBuilder sourceBuilder)
        {
            sourceBuilder.AppendLine("    }");
            sourceBuilder.AppendLine("}");
            sourceBuilder.AppendLine("#nullable restore");
        }
    }
}