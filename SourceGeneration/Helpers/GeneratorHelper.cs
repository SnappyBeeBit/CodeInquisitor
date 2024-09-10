using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGeneration.Helpers
{
    public static class GeneratorHelper
    {
        public static string AddGeneratorSuffix(this string file)
        {
            return file + ".g.cs";
        }
    }
}
