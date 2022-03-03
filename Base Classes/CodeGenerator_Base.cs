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
    internal abstract class CodeGenerator_Base
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

        /// <summary>Generate the Region directives for the Constructors region </summary>
        protected CodeRegionDirective Region_Constructor(CodeRegionMode mode) => mode == CodeRegionMode.Start ? StartRegion("Constructors") : mode == CodeRegionMode.End ? EndRegion("Constructors") : null; 

        /// <summary>Generate the Region directives for the Properties region </summary>
        protected CodeRegionDirective Region_Properties(CodeRegionMode mode) => mode == CodeRegionMode.Start ? StartRegion("Properties") : mode == CodeRegionMode.End ?  EndRegion("Properties") : null;

        /// <summary>Generate the Region directives for the Methods region </summary>
        protected CodeRegionDirective Region_Methods(CodeRegionMode mode) => mode == CodeRegionMode.Start ? StartRegion("Methods") : mode == CodeRegionMode.End ? EndRegion("Methods") : null;

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

        /// <summary>
        /// Process the CodeCompileUnit and save it to disk, then add it to the project.
        /// </summary>
        /// <param name="OutputUnit">Generate Code by looping through all NameSpaces. DOes not generate code at the CodeCompileUnit level to avoid the automatic comment.</param>
        /// <param name="AddAsSubFile">Set TRUE to add as a file below the XSD file (where a SingleFileGenerator would typically put files.)<br/> Set FALSE to add to the project itself so it appears on same level as the xsd file in the solution explorer tree.</param>
        protected void Save(CodeCompileUnit OutputUnit, bool AddAsSubFile)
        {
            //Delete the file if it exists
            if (File.Exists(FileOnDisk.FullName)) FileOnDisk.Delete();
            
            // Only create the file if its missing
            if (!File.Exists(FileOnDisk.FullName))
            {
                ICodeGenerator Generator = LanguageProvider.CreateGenerator(this.FileOnDisk.FullName);
                using (IndentedTextWriter writer = new IndentedTextWriter(new StreamWriter(FileOnDisk.FullName)))
                {
                    if (OutputUnit == null)
                    {
                        Generator.GenerateCodeFromStatement(ParsedFile.ObjectProvider.UnableToParseComment, writer, SaveOptions);
                    }
                    else
                    {
                        foreach (CodeNamespace NS in OutputUnit.Namespaces)
                            Generator.GenerateCodeFromNamespace(NS, writer, SaveOptions);
                    }
                    writer.Close();
                }
            }
            if (FileOnDisk.Exists)
            {
                if (AddAsSubFile)
                    VSTools.AddFileToProject(XSDInstance.InputFile, FileOnDisk);
                else
                    VSTools.AddFileToProject(FileOnDisk);
            }

        }

        /// <summary>
        /// Modify the Default <see cref="CodeGeneratorOptions"/> this file is saved with.
        /// </summary>
        /// <remarks>
        /// Base has the following settings: <br/>
        /// BlankLinesBetweenMembers = true <br/>
        /// VerbatimOrder = true<br/>
        /// </remarks>
        protected virtual CodeGeneratorOptions SaveOptions => new CodeGeneratorOptions()
        {
            BlankLinesBetweenMembers = true,
            VerbatimOrder = true
        };

        #endregion < Methods >
    }

}
