using Baumeister.Generators.Building;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace Baumeister.Generators.UnitTests;

public class BuilderSourceGeneratorTest
{
    [Test]
    public void Generator_ShouldGenerateOnlyForCorrectProperties()
    {
        var source = @"
namespace Baumeister.Building
{
    public partial class TestClassBuilder : BuilderBase<TestClass>;
       
    public class TestClass
    {
        private string _notSettable = ""initial"";
        public static string StaticProperty { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
";

        var compilation = CreateCompilation(source);

        // Act: run the source generator
        var generator = new BuilderSourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);

        // Assert: no diagnostics from the generator run
        Console.WriteLine($"Diagnostics count: {diagnostics.Length}");
        foreach (var diagnostic in diagnostics)
        {
            Console.WriteLine($"Diagnostic: {diagnostic}");
        }
        Assert.That(diagnostics, Is.Empty, "Expected no diagnostics from source generator.");

        var generatedSourceCode = updatedCompilation.SyntaxTrees.ElementAt(1).GetRoot().ToFullString();
       
        // Debug: Print generated code
        Console.WriteLine("Generated code:");
        Console.WriteLine(generatedSourceCode);

        // Basic assertions that the important parts exist in generated code
        Assert.That(generatedSourceCode, Does.Contain("#nullable enable"));
        Assert.That(generatedSourceCode, Does.Contain("public partial class TestClassBuilder"));
        Assert.That(generatedSourceCode, Does.Contain("public TestClassBuilder()"));
        Assert.That(generatedSourceCode, Does.Contain("OnInitializeDefaults();"));
        Assert.That(generatedSourceCode, Does.Contain("public static TestClassBuilder New()"));
        Assert.That(generatedSourceCode, Does.Contain("public TestClassBuilder WithName(string name)"));
        Assert.That(generatedSourceCode, Does.Contain("this.With(\"Name\", name);"));
        Assert.That(generatedSourceCode, Does.Contain("this.With(\"Age\", age);"));
        Assert.That(generatedSourceCode, Does.Not.Contain("WithEqualityContract"));
        Assert.That(generatedSourceCode, Does.Not.Contain("StaticProperty"));
        Assert.That(generatedSourceCode, Does.Not.Contain("notSettable"));
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var parseOptions = new CSharpParseOptions(languageVersion: LanguageVersion.CSharp12);
        var syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(source, System.Text.Encoding.UTF8), parseOptions);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Baumeister.Abstractions.Building.BuilderBase<>).Assembly.Location)
        };

        return CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}
