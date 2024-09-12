using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace SourceGeneration.Helpers
{

    public record ExecuteChecks<T>(bool ToLeave, IEnumerable<T> ConvertedNodes);
    public static class ExecuteHelper<T>
    {
        public static ExecuteChecks<T> RunChecks((Compilation compilation, ImmutableArray<object> nodes) tuple)
        {
            return new(tuple.nodes.IsDefaultOrEmpty, Distinctify(tuple.nodes));
        }

        public static IEnumerable<T> Distinctify(ImmutableArray<object> nodes)
        {
            if(nodes.IsDefaultOrEmpty)
            {
                return [];
            }
            List<T> result = [];
            foreach (var node in nodes)
            {
                result.Add((T)node);
            }
            return result.Distinct();
        }
    }
}
