using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGeneration
{
    public static class GeneratorHelper
    {
        public static string AddGeneratorSuffix(this string file)
        {
            return file + ".g.cs";
        }
    }
}
