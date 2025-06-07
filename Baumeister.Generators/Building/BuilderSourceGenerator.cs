using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
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
            var namespaceDeclaration = candidate.Ancestors().OfType<NamespaceDeclarationSyntax>().First();
            var namespaceName = namespaceDeclaration.Name.ToString();

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
            var semanticModel = compilation.GetSemanticModel(candidate.SyntaxTree);
            var baseType = candidate.BaseList?.Types
                .Select(baseType => semanticModel.GetTypeInfo(baseType.Type).Type)
                .OfType<INamedTypeSymbol>()
                .FirstOrDefault(type => type.Name == "BuilderBase");

            if (baseType != null && baseType.TypeArguments.Length == 1)
            {
                if (baseType.TypeArguments[0] is INamedTypeSymbol typeToBuild)
                {
                    foreach (var property in typeToBuild.GetMembers().OfType<IPropertySymbol>())
                    {
                        AddWithMethodFor(property.Type, property.Name, className, sourceBuilder);
                    }
                }
            }
        }

        private static void AddWithMethodFor(ITypeSymbol propertyType, string propertyName, string builderTypeName, StringBuilder sourceBuilder)
        {
            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine($"        public {builderTypeName} With{propertyName}({propertyType.Name} {propertyName.ToLower(CultureInfo.InvariantCulture)})");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine($"            this.With(\"{propertyName}\", {propertyName.ToLower(CultureInfo.InvariantCulture)});");
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