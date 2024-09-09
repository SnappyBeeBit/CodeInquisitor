using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;

namespace SourceGeneration
{
    [Generator]
    public class Generator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            var provider = initContext.SyntaxProvider.CreateSyntaxProvider(
                predicate: Filter,
                transform: ConvertNode

                ).Where(i => i is not null);

            var compilation = initContext.CompilationProvider.Combine(provider.Collect());
            initContext.RegisterSourceOutput(compilation, Execute);

        }
        private bool Filter(SyntaxNode node, CancellationToken token)
        {
             return (node is ClassDeclarationSyntax c && c.AttributeLists.Count > 0 ||
                (node is StructDeclarationSyntax s && s.AttributeLists.Count > 0));
           
        }
        private object ConvertNode(GeneratorSyntaxContext context, CancellationToken token)
        {
            if (context.Node is ClassDeclarationSyntax)
            {
                return (ClassDeclarationSyntax)context.Node;
            }
            else
            {
                return (StructDeclarationSyntax)context.Node;
            }
        }
        private void Execute(SourceProductionContext context, (Compilation Left, ImmutableArray<object> Right) tuple)
        {
            var syntaxNodes = tuple.Right;
            var compilation = tuple.Left;
            if (syntaxNodes.IsDefaultOrEmpty)
            {
                return;
            }

            var distinctClasses = syntaxNodes.Distinct();

            foreach (var syntaxNodeObject in distinctClasses)
            {
                var syntaxNode = (SyntaxNode)syntaxNodeObject;
                var namedTypedSymbol = compilation.GetSemanticModel(syntaxNode.SyntaxTree).GetDeclaredSymbol(syntaxNode) as INamedTypeSymbol;
                
                context.AddSource(namedTypedSymbol.MetadataName.AddGeneratorSuffix(), "/////Hello?");
            }
        }
     

      
    }
}