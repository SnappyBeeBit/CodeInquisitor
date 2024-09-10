using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace SourceGeneration
{
    [Generator]
    public class TernaryInq : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var provider = context.SyntaxProvider.CreateSyntaxProvider(
              predicate: Filter,
              transform: ConvertNode

              ).Where(i => i is not null);

            var compilation = context.CompilationProvider.Combine(provider.Collect());
            context.RegisterSourceOutput(compilation, Execute);
        }
        private bool Filter(SyntaxNode node, CancellationToken token)
        {
            return node.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ConditionalExpression);
        }
        private object ConvertNode(GeneratorSyntaxContext context, CancellationToken token)
        {
            return (ConditionalExpressionSyntax)context.Node;
        }
        private void Execute(SourceProductionContext context, (Compilation Left, ImmutableArray<object> Right) tuple)
        {
            var compilation = tuple.Left;
            var nodes = tuple.Right;
            int counter = 0;
            foreach (var node in nodes)
            {
              
                var conditionalExpressionSyntax =  node as ConditionalExpressionSyntax;
                var x = conditionalExpressionSyntax.GetText();
                bool lineViolation = x.Lines.Count > 2;
                bool nestedViolation = conditionalExpressionSyntax.DescendantNodes().Where(i => i as ConditionalExpressionSyntax is ConditionalExpressionSyntax).Count() >= 3;
                if(lineViolation)
                {
                    context.ReportDiagnostic(Diagnostic.Create(TernaryDiagnosticDescriptors.TooManyLinesTernaryViolation, 
                                            conditionalExpressionSyntax.GetLocation(), TernaryDiagnosticDescriptors.TMLVDescription));
                }
                if (nestedViolation)
                {
                    context.ReportDiagnostic(Diagnostic.Create(TernaryDiagnosticDescriptors.NestedTernaryViolation,
                                            conditionalExpressionSyntax.GetLocation(), TernaryDiagnosticDescriptors.NTVDescription));
                }
                counter++;
            }

        }
    }
    public static class TernaryDiagnosticDescriptors
    {
        public static DiagnosticDescriptor NestedTernaryViolation = new("CI_Ternary", "Nested Ternary Error", "'{0}'", "", DiagnosticSeverity.Error, true);

        public static string NTVDescription = "Too many nested ternary operators, consider creating an if/else or swtich statement";

        public static DiagnosticDescriptor TooManyLinesTernaryViolation = new("CI_Ternary", "Too Many Lines Error", "'{0}'", "", DiagnosticSeverity.Error, true);

        public static string TMLVDescription = "Too many lines in ternary operation, consider creating an if/else or switch statement";
    }

}
