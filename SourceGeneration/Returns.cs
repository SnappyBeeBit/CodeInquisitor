using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGeneration.Helpers;

namespace SourceGeneration
{
    [Generator]
    public class Returns : CodeInquisitor
    {
        protected override bool Filter(SyntaxNode node, CancellationToken token)
        {
            return node is ReturnStatementSyntax returnStatement && returnStatement.GetText().Lines.Count > 1;
        }
        protected override object ConvertNode(GeneratorSyntaxContext context, CancellationToken token)
        {
            return context.Node;
        }

        protected override void Execute(SourceProductionContext context, (Compilation compilation, ImmutableArray<object> nodes) tuple)
        {
            ExecuteChecks<ReturnStatementSyntax> executeChecks = ExecuteHelper<ReturnStatementSyntax>.RunChecks(tuple);
            if(executeChecks.ToLeave)
                return;
            
            foreach(ReturnStatementSyntax node in executeChecks.ConvertedNodes)
            {
               if(node.GetText().Lines.Count > 3)
               {
                    context.ReportDiagnostic(Diagnostic.Create(ReturnStatementDDs.ReturnStatementLineCountViolation, node.GetLocation(), ReturnStatementDDs.RSLCViolation));
               }
            }
        }
        public static class ReturnStatementDDs
        {
            public static DiagnosticDescriptor ReturnStatementLineCountViolation = new("CI_Return", "Return Statement Line Count Error", "'{0}'", "", DiagnosticSeverity.Error, true);

            public static string RSLCViolation = "Return Statement is more than 5 lines long, consider doing operations beforehand";

        }


    }
}
