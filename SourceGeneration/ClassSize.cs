using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGeneration.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;

namespace SourceGeneration
{
    [Generator]
    internal class ClassSize : CodeInquisitor
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
            return context.Node;
        }
        protected override void Execute(SourceProductionContext context, (Compilation compilation, ImmutableArray<object> nodes) tuple)
        {
            ExecuteChecks<ClassDeclarationSyntax> executeChecks = ExecuteHelper<ClassDeclarationSyntax>.RunChecks(tuple);
            if (executeChecks.ToLeave)
            {
                return;
            }
            foreach (ClassDeclarationSyntax node in executeChecks.ConvertedNodes)
            {
              
                if (node.GetText().Lines.Count > 1000)
                {
                   
                    context.ReportDiagnostic(Diagnostic.Create(ClassSizeDDs.ClassSizeViolation,  node.GetLocation(), ClassSizeDDs.CSDescription));
                }
            }
        }
        public static class ClassSizeDDs
        {
            public static DiagnosticDescriptor ClassSizeViolation = new("CI_ClassSize", "Class Size Error", "'{0}'", "", DiagnosticSeverity.Error, true);

            public static string CSDescription = "Class is over 1000 lines, break it down into smaller parts";
        }


    }
}
