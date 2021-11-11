using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XSDCustomToolVSIX.Generate_Helpers.CSharp
{
    internal class DiscoveredClass_CSharp : DiscoveredClass
    {
        /// <inheritdoc cref="DiscoveredClass.DiscoveredClass(ParsedFile,string,int,int, bool)"/>
        internal DiscoveredClass_CSharp (ParsedFile parsedFile, string className, int startIndex, int endIndex, bool hasConstructor) :
            base(parsedFile, className, startIndex, endIndex, hasConstructor)
        { }

        internal override Enums.SupportedLanguages ClassLanguage => Enums.SupportedLanguages.CSharp;
        internal override string EndOfLineChar => ";";

        internal override string GetConstructor(int IndentLevel)
        {
            string ret = $"{VSTools.TabIndent(IndentLevel)}public {this.ClassName}() {{{Environment.NewLine}";
            foreach (DiscoveredProperty p in this.ClassProperties )
            {
                ret += p.GetProperyInitializer(IndentLevel + 1, true);
            }
            ret += $"{Environment.NewLine}{VSTools.TabIndent(IndentLevel)}}}";
            return ret;
        }

        /// <returns>"{Public/Private} {ClassName} {PropertyName} {get;set;}"</returns>
        /// <inheritdoc cref="DiscoveredClass.GetPropertyString(int, bool)"/>
        internal override string GetPropertyString(int IndentLevel, bool IsPublic = true)
        {
            return String.Concat(
                $"{VSTools.TabIndent(IndentLevel)}/// <summary>  </summary>{Environment.NewLine}",
                $"{VSTools.TabIndent(IndentLevel)}{(IsPublic ? "public" : "private")} {ClassName} {HelperClass_PropertyName} {{ ",
                $"get; {(IsPublic ? "private " : "")}set; }}"
                );
        }

        //internal override string BuildClassTree(int IndentLevel)
        //{
        //    string properties = String.Empty;
        //    string nested = String.Empty;

        //    foreach (DiscoveredClass IC in InnerClasses)
        //        properties = String.Concat(properties, IC.GetPropertyString(IndentLevel + 1), Environment.NewLine);

        //    foreach (DiscoveredClass IC in InnerClasses)
        //        nested = String.Concat(nested, IC.BuildClassTree(IndentLevel + 1), Environment.NewLine);

        //    string thisclass = String.Concat(
        //        $"{VSTools.TabIndent(IndentLevel)}#region < {this.ClassName} >\n",
        //        this.GetConstructor(IndentLevel + 1), Environment.NewLine,

        //        $"{VSTools.TabIndent(IndentLevel)}#region < Nested Class Objects Properties >\n",
        //        properties, Environment.NewLine,
        //        $"{VSTools.TabIndent(IndentLevel)}#endregion </ Nested Class Objects Properties >\n", Environment.NewLine,

        //        $"{VSTools.TabIndent(IndentLevel)}#region < Nested Classes >\n",
        //        nested, Environment.NewLine,
        //        $"{VSTools.TabIndent(IndentLevel)}#endregion </ Nested Classes >\n",

        //        $"{VSTools.TabIndent(IndentLevel)}#endregion </ {this.ClassName} >"
        //        );

        //    return thisclass;
        //}
    }
}
