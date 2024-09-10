using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SourceGeneration.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace SourceGeneration
{
    [Generator]
    public class TernaryOperations : CodeInquisitor
    {
        public override void Initialize(IncrementalGeneratorInitializationContext context)
        {
           base.Initialize(context);
        }
        protected override bool Filter(SyntaxNode node, CancellationToken token)
        {
            return node is ConditionalExpressionSyntax expressionSyntax && (expressionSyntax.DescendantNodes().Count() > (3 * 3) || expressionSyntax.GetText().Lines.Count > 1);
        }
        protected override object ConvertNode(GeneratorSyntaxContext context, CancellationToken token)
        {
            return context.Node;
        }
        protected override void Execute(SourceProductionContext context, (Compilation compilation, ImmutableArray<object> nodes) tuple)
        {
            ExecuteChecks<ConditionalExpressionSyntax> executeChecks = ExecuteHelper<ConditionalExpressionSyntax>.RunChecks(tuple);
            if (executeChecks.ToLeave) return;

            var compilation = tuple.compilation;
            int count = 0;
            foreach (ConditionalExpressionSyntax node in executeChecks.ConvertedNodes)
            {

                context.AddSource($"{count} output.g.cs", $"{node.DescendantNodes().Count()}");

                var x = node.GetText();
                bool lineViolation = x.Lines.Count > 2;
                bool nestedViolation = node.DescendantNodes().Where(i => i as ConditionalExpressionSyntax is not null).Count() >= 3;
                if(lineViolation)
                {
                    context.ReportDiagnostic(Diagnostic.Create(TernaryDDs.TooManyLinesTernaryViolation, node.GetLocation(), TernaryDDs.TMLVDescription));
                }
                if (nestedViolation)
                {
                    context.ReportDiagnostic(Diagnostic.Create(TernaryDDs.NestedTernaryViolation, node.GetLocation(), TernaryDDs.NTVDescription));
                }
                count++;    
            }
        }
    }
    public static class TernaryDDs
    {
        public static DiagnosticDescriptor NestedTernaryViolation = new("CI_Ternary", "Nested Ternary Error", "'{0}'", "", DiagnosticSeverity.Error, true);

        public static string NTVDescription = "Too many nested ternary operators, consider creating an if/else or swtich statement";

        public static DiagnosticDescriptor TooManyLinesTernaryViolation = new("CI_Ternary", "Too Many Lines Error", "'{0}'", "", DiagnosticSeverity.Error, true);

        public static string TMLVDescription = "Too many lines in ternary operation, consider creating an if/else or switch statement";
    }

}
