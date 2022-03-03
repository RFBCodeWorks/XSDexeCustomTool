using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSDCustomToolVSIX.BaseClasses;
using XSDCustomToolVSIX.Interfaces;
using XSDCustomToolVSIX.Language_Specific_Overrides;

namespace XSDCustomToolVSIX.BaseClasses
{
    /// <summary>
    /// Class that reads in an XSD file directly, then generates a class using LINQ style properties.
    /// </summary>
    internal abstract class LinqClassGenerator_Base : ILinqClassGenerator
    {
        #region < Class Factory >

        protected LinqClassGenerator_Base() 
        {
            CodeDomProvider = CodeDomObjectProvider.GetObjectProvider(Language);
        }

        internal static ILinqClassGenerator Factory(XSD_Instance xSD)
        {
            switch (xSD.XSDexeOptions.Language)
            {
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VB:
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VJS:
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.JS:
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.CS:
                default:
                    return new LinqOverrides_CSharp();
            }
        }

        #endregion

        #region < Properties >

        protected abstract XSDCustomTool_ParametersXSDexeOptionsLanguage Language { get; }

        protected ICodeDomObjectProvider CodeDomProvider { get; }

        #endregion

    }
}
