using AICodeReview.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AICodeReview.Services.CodeAnalyser
{
    public class CodeAnalyzerService : ICodeAnalyzerService
    {
        public List<string> Analyze(string code)
        {
            var warnings = new List<string>();
            if (string.IsNullOrWhiteSpace(code))
                return warnings;

            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            var methods = root.DescendantNodes()
                              .OfType<MethodDeclarationSyntax>();

            foreach (var method in methods)
            {
                var lines = method.ToString().Split('\n').Length;

                if (lines > 40)
                    warnings.Add($"Método '{method.Identifier}' muito grande ({lines} linhas)");
            }

            var classes = root.DescendantNodes()
                              .OfType<ClassDeclarationSyntax>();

            foreach (var c in classes)
            {
                if (c.Members.Count > 15)
                    warnings.Add($"Classe '{c.Identifier}' possui muitos membros ({c.Members.Count})");
            }

            if (code.Contains("int "))
            {
                warnings.Add("Use var");
            }

            return warnings;
        }
    }
}
