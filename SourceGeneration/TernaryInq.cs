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

            foreach (var node in nodes)
            {
                var conditionalExpressionSyntax =  node as ConditionalExpressionSyntax;
                var x = conditionalExpressionSyntax.GetText();
                if(x.Lines.Count > 2)
                {
                    Console.WriteLine("LOCATED");
                    context.AddSource("Vioalation".AddGeneratorSuffix(), $"/////Ternary Violation: {conditionalExpressionSyntax.GetLocation().GetLineSpan().ToString()}");
                }
            }
        }
    }
}
