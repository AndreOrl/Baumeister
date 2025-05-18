using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
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

        private void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
        {
            foreach (var candidate in classes)
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

                AddNewMethod(className, sourceBuilder);

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

                sourceBuilder.AppendLine("    }");
                sourceBuilder.AppendLine("}");
                sourceBuilder.AppendLine("#nullable restore");

                context.AddSource($"{className}Extensions.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }

        private void AddNewMethod(string builderTypeName, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"        public static {builderTypeName} New()");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine($"            return new {builderTypeName}();");
            stringBuilder.AppendLine("        }");
        }

        private void AddWithMethodFor(ITypeSymbol propertyType, string propertyName, string builderTypeName, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"        public {builderTypeName} With{propertyName}({propertyType} {propertyName.ToLower()})");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine($"            this.With(\"{propertyName}\", {propertyName.ToLower()});");
            stringBuilder.AppendLine("            return this;");
            stringBuilder.AppendLine("        }");
        }
    }
}