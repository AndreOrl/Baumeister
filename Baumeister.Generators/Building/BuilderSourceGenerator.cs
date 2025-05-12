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
            var classDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Baumeister.Abstractions.Building.BuilderAttribute",
                predicate: static (_, _) => true,
                transform: static (ctx, _) => GetClassDeclaration(ctx))
                .Collect()
                .SelectMany((m, _) => m.Distinct());

            var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

            context.RegisterSourceOutput(compilationAndClasses, (spc, source) => Execute(source.Left, source.Right!, spc));
        }

        private static ClassDeclarationSyntax GetClassDeclaration(GeneratorAttributeSyntaxContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.TargetNode;
            return classDeclaration;
        }

        private void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
        {
            if (compilation.AssemblyName?.EndsWith(".Tests") == true)
            {
                return;
            }

            foreach (var candidate in classes)
            {
                var className = candidate.Identifier.ValueText;
                var namespaceDeclaration = candidate.Ancestors().OfType<NamespaceDeclarationSyntax>().First();
                var namespaceName = namespaceDeclaration.Name.ToString();

                var sourceBuilder = new StringBuilder();
                sourceBuilder.AppendLine("using System;");
                sourceBuilder.AppendLine("");
                sourceBuilder.AppendLine($"namespace {namespaceName}");
                sourceBuilder.AppendLine("{");
                sourceBuilder.AppendLine($"    public partial class {className}");
                sourceBuilder.AppendLine("    {");

                AddNewMethod(className, sourceBuilder);

                var builderAttribute = candidate.AttributeLists
                    .SelectMany(attrList => attrList.Attributes)
                    .FirstOrDefault(attr => (attr.Name as IdentifierNameSyntax)?.Identifier.Text == "Builder");

                if (builderAttribute != null)
                {
                    var semanticModel = compilation.GetSemanticModel(builderAttribute.SyntaxTree);
                    var attributeSymbol = semanticModel.GetSymbolInfo(builderAttribute).Symbol as IMethodSymbol;
                    if (attributeSymbol != null && attributeSymbol.ContainingType.ToDisplayString() == "Baumeister.Abstractions.Building.BuilderAttribute")
                    {
                        var typeOfExpression = builderAttribute.ArgumentList?.Arguments.FirstOrDefault()?.Expression as TypeOfExpressionSyntax;
                        if (typeOfExpression != null)
                        {
                            var typeToBuild = semanticModel.GetTypeInfo(typeOfExpression.Type).Type as INamedTypeSymbol;
                            if (typeToBuild != null)
                            {
                                foreach (var property in typeToBuild.GetMembers().OfType<IPropertySymbol>())
                                {
                                    AddWithMethodFor(property.Type, property.Name, className, sourceBuilder);
                                }
                            }
                        }
                    }
                }

                sourceBuilder.AppendLine("    }");
                sourceBuilder.AppendLine("}");

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
            stringBuilder.AppendLine($"            this.With({propertyName.ToLower()});");
            stringBuilder.AppendLine("            return this;");
            stringBuilder.AppendLine("        }");
        }
    }
}