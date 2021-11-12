using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace XSDCustomToolVSIX.Generate_Helpers
{
 
    /// <summary>
    /// Abstract Base Class for all CodeGenerator classes. <br/>
    /// Uses the <see cref="CodeDomProvider"/> selected by the <see cref="ParsedFile.LanguageProvider"/> to Save the the code to text. <br/>
    /// This class  also provides the methods  shared across all generated files.
    /// </summary>
    abstract class CodeGenerator_Base
    {
        private CodeGenerator_Base() { }

        protected CodeGenerator_Base(ParsedFile parsedFile) { this.ParsedFile = parsedFile; }

        /// <summary> Reference to ParsedFile object that controls which Generators are firing. </summary>
        internal protected ParsedFile ParsedFile { get; }

        /// <summary> This is the FileInfo object that houses the path when writing this file to disk. </summary>
        public abstract FileInfo FileOnDisk { get; }


        //////////////////////////////////////////////////////////////////////////////

        #region < CodeDom >

        protected virtual CodeDomProvider LanguageProvider => ParsedFile.LanguageProvider;

        #endregion </ CodeDom >

        //////////////////////////////////////////////////////////////////////////////

        #region < ShortCuts for XSD Parameters >        

        /// <inheritdoc cref="XSDCustomTool_ParametersXSDexeOptions.NameSpace"/>
        protected string NameSpace => XSDInstance.XSDexeOptions.NameSpace;

        protected bool IsGeneratingClass => XSDInstance.IsGeneratingClass;

        protected bool IsGeneratingDataSet => XSDInstance.IsGeneratingDataSet;

        #endregion < ShortCuts for XSD Parameters >

        //////////////////////////////////////////////////////////////////////////////

        #region < Shortcuts to the ParsedFile Properties >

        /// <inheritdoc cref="ParsedFile.TopLevelClass"/>
        protected DiscoveredClass TopLevelClass => ParsedFile.TopLevelClass;

        /// <inheritdoc cref="ParsedFile.xSD_Instance"/>
        internal protected XSD_Instance XSDInstance => ParsedFile.xSD_Instance;

        /// <inheritdoc cref="ParsedFile.DiscoveredClasses"/>
        protected DiscoveredClass[] DiscoveredClasses => ParsedFile.DiscoveredClasses;

        #endregion < Shortcuts to the ParsedFile Properties >

        //////////////////////////////////////////////////////////////////////////////

        #region < Abstract Methods >

        /// <summary>
        /// Run this method to create a supplement file for the parial classes XSD.exe generated. <br/>
        /// This method will end by calling <see cref="CodeGenerator_Base.Save(CodeCompileUnit);"/>
        /// </summary>
        public abstract void Generate();

        /// <summary>Generates the comment header of the file that says the file was automatically generated and when it will be overwritten.</summary>
        protected abstract CodeCommentStatementCollection GetComment_AutoGen();

        #endregion </ Abstract Methods >

        #region < Methods >

        /// <summary> ShortHand for Environment.NewLine </summary>
        protected static string NL => Environment.NewLine;

        ///<inheritdoc cref="VSTools.TabIndent(int)"/>
        protected virtual string TabLevel(int i) => VSTools.TabIndent(i);

        /// <summary>Generate the StartRegion directive for the Constructors region </summary>
        protected virtual CodeRegionDirective StartRegion_Constructor() => StartRegion("Constructors");
        /// <summary>Generate the EndRegion directive for the Constructors region </summary>
        protected virtual CodeRegionDirective EndRegion_Constructor() => EndRegion("Constructors");

        /// <summary> 
        /// Indicates the start of a region of code. <br/> 
        /// </summary>
        protected virtual CodeRegionDirective StartRegion(string RegionHeader)
        {
            CodeRegionDirective region = new CodeRegionDirective();
            region.RegionMode = CodeRegionMode.Start;
            region.RegionText = $"< {RegionHeader} >";
            return region;
        }

        /// <summary> 
        /// Indicates the end of a region of code.
        /// </summary>
        protected virtual CodeRegionDirective EndRegion(string RegionHeader)
        {
            CodeRegionDirective region = new CodeRegionDirective();
            region.RegionMode = CodeRegionMode.End;
            region.RegionText = $"</ {RegionHeader} >";
            return region;
        }

        protected void Save(CodeCompileUnit OutputUnit)
        {
            ICodeGenerator Generator = LanguageProvider.CreateGenerator(this.FileOnDisk.FullName);
            Generator.GenerateCodeFromCompileUnit(OutputUnit, TextWriter.Null, new CodeGeneratorOptions { BlankLinesBetweenMembers = true });
            AddToProject(FileOnDisk);
        }

        /// <summary>
        /// Adds the file to the project.
        /// </summary>
        protected virtual void AddToProject(FileInfo file)
        {
            if (file.Exists)
                VSTools.AddFileToProject(XSDInstance.InputFile, file);
        }

        #endregion < Methods >
    }
}
