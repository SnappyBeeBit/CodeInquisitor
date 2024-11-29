using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGeneration.Helpers;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;

namespace SourceGeneration
{
    [Generator]
    public class MyGenerator : CodeInquisitor
    {
        public override void Initialize(IncrementalGeneratorInitializationContext context)
        {
            base.Initialize(context);
        }
        protected override bool Filter(SyntaxNode node, CancellationToken token)
        {
            return node is ClassDeclarationSyntax;
           
        }
        protected override object ConvertNode(GeneratorSyntaxContext context, CancellationToken token)
        {
            return (ClassDeclarationSyntax)context.Node;
        }
        protected override void Execute(SourceProductionContext context, (Compilation compilation, ImmutableArray<object> nodes) tuple)
        {
            ExecuteChecks<ClassDeclarationSyntax> executeChecks = ExecuteHelper<ClassDeclarationSyntax>.RunChecks(tuple);
            if(executeChecks.ToLeave) return;

            var compilation = tuple.compilation;
           
            foreach (ClassDeclarationSyntax syntaxNode in executeChecks.ConvertedNodes)
            {
                var namedTypedSymbol = compilation.GetSemanticModel(syntaxNode.SyntaxTree).GetDeclaredSymbol(syntaxNode) as INamedTypeSymbol;             
                context.AddSource(namedTypedSymbol.MetadataName.AddGeneratorSuffix(), $"// Return Size {Settings.ReturnSize} \n //Ternary Density {Settings.TernaryDensity} \n //Ternary Line Count {Settings.TernaryLineCount} \n //Class Size {Settings.ClassSize}");
            }
        }
        
      
    }
}