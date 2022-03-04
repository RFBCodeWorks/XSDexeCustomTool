using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSDCustomToolVSIX.BaseClasses;
using XSDCustomToolVSIX.Interfaces;

namespace XSDCustomToolVSIX.Language_Specific_Overrides
{
    internal class LinqOverrides_CSharp : LinqClassGenerator_Base
    {
        public LinqOverrides_CSharp(IParsedFile parsedFile) : base(parsedFile) { }

    }
}
