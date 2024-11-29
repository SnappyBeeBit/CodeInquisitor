using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SourceGeneration.Helpers
{
    public abstract class CodeInquisitor : IIncrementalGenerator
    {
        public Settings.Settings Settings { get; set;}
        protected abstract bool Filter(SyntaxNode node, CancellationToken token);
        protected abstract object ConvertNode(GeneratorSyntaxContext context, CancellationToken token);
        protected abstract void Execute(SourceProductionContext context, (Compilation compilation, ImmutableArray<object> nodes) tuple);

        public virtual void Initialize(IncrementalGeneratorInitializationContext context)
        {
			var provider = context.SyntaxProvider.CreateSyntaxProvider(
               predicate: Filter,
               transform: ConvertNode

               ).Where(i => i is not null);

            var compilation = context.CompilationProvider.Combine(provider.Collect());
            this.LoadSettings(context);
			context.RegisterSourceOutput(compilation, Execute);
        }
	}

}
