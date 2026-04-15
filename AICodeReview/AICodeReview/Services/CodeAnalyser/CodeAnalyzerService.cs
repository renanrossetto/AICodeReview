using AICodeReview.Common;
using AICodeReview.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AICodeReview.Services.CodeAnalyser
{
    public class CodeAnalyzerService : ICodeAnalyzerService
    {
        public List<string> Analyze(string? code)
        {
            var warnings = new List<string>();
            
            if (string.IsNullOrWhiteSpace(code))
                return warnings;

            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var method in methods)
            {
                var lines = method.ToString().Split('\n').Length;

                if (lines > 40)
                    warnings.Add(string.Format(Messages.MethodWithManyLines, method.Identifier, lines));               
            }

            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var c in classes)
            {
                if (c.Members.Count > 15)
                    warnings.Add(string.Format(Messages.ClassWithManyMembers, c.Identifier, c.Members.Count));                
            }

            return warnings;
        }
    }
}
