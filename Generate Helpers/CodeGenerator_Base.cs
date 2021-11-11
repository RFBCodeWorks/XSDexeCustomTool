using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XSDCustomToolVSIX.Generate_Helpers
{
 
    class CSCoder : Microsoft.CSharp.CSharpCodeProvider
    {
    }

    abstract class CodeGenerator_Base
    {
        private CodeGenerator_Base() { }

        protected CodeGenerator_Base(ParsedFile parsedFile) { this.ParsedFile = parsedFile; }

        internal protected ParsedFile ParsedFile { get; }

        public abstract FileInfo FileOnDisk { get; }

        //////////////////////////////////////////////////////////////////////////////
        
        #region < ShortCuts for XSD Parameters >

        private  void test ()
        {
            System.CodeDom.CodeCompileUnit code = new CSCoder().Parse();
            code.Namespaces[0].Types[0].Members[0].
        }
        

        /// <inheritdoc cref="XSDCustomTool_ParametersXSDexeOptions.NameSpace"/>
        protected string NameSpace => XSDInstance.XSDexeOptions.NameSpace;

        protected bool IsGeneratingClass => XSDInstance.IsGeneratingClass;

        protected bool IsGeneratingDataSet => XSDInstance.IsGeneratingDataSet;

        #endregion < ShortCuts for XSD Parameters >

        //////////////////////////////////////////////////////////////////////////////

        #region < Shortcuts to the ParsedFile Properties >

        /// <inheritdoc cref="ParsedFile.SummaryIndicator"/>
        protected string SummaryIndicator => ParsedFile.SummaryIndicator;

        /// <inheritdoc cref="ParsedFile.CommentIndicator"/>
        protected string CommentIndicator => ParsedFile.CommentIndicator;

        /// <inheritdoc cref="ParsedFile.TopLevelClass"/>
        protected DiscoveredClass TopLevelClass => ParsedFile.TopLevelClass;

        /// <inheritdoc cref="ParsedFile.xSD_Instance"/>
        internal protected XSD_Instance XSDInstance => ParsedFile.xSD_Instance;

        

        #endregion < Shortcuts to the ParsedFile Properties >

        //////////////////////////////////////////////////////////////////////////////

        #region < Methods >

        /// <summary> ShortHand for Environment.NewLine </summary>
        protected static string NL => Environment.NewLine;

        ///<inheritdoc cref="VSTools.TabIndent(int)"/>
        protected virtual string TabLevel(int i) => VSTools.TabIndent(i);

        /// <summary>Disable the RegionWrap function. Will return the InputTxt value instead.</summary>
        protected virtual bool DisableRegionWrap => false;
        
        /// <summary> 
        /// Indicates the start of a region of code. <br/> 
        /// Override if the language uses something other than #region <br/>
        /// For example, JS will use //#region, so this will have to be overridden.
        /// </summary>
        protected virtual string StartRegion => "#region";
        
        /// <summary> 
        /// Indicates the end of a region of code. <br/> 
        /// Override if the language uses something other than #endregion <br/>
        /// For example, JS will use //#endregion, so this will have to be overridden.
        /// </summary>
        protected virtual string EndRegion => "#endregion";

        /// <summary>
        /// Wrap the InputTxt in a #region .... #endRegion block
        /// </summary>
        /// <param name="inputTxt">Text to wrap - This should have atleast one {Environment.NewLine} at the end of it. </param>
        /// <param name="RegionName">This will be wrapped with carots as the Region Description </param>
        /// <param name="BaseIndentLevel">Number of Indents to put onto the #region / #endregion lines.</param>
        /// <returns></returns>
        protected virtual string RegionWrap(string inputTxt, string RegionName, int IndentLevel)
        {
            if (DisableRegionWrap) return $"{NL}{inputTxt}{NL}";
            string txt = $"{VSTools.TabIndent(IndentLevel)}{StartRegion} < {RegionName} >{NL}{NL}";
            txt += inputTxt;
            txt += $"{VSTools.TabIndent(IndentLevel)}{EndRegion} </ {RegionName} >{NL}{NL}";
            return txt;
        }

        /// <summary>
        /// Writes the file text to the FileOnDisk location, then adds it to the project.
        /// </summary>
        /// <param name="fileText"></param>
        internal protected void Save(string fileText)
        {
            File.WriteAllText(this.FileOnDisk.FullName, fileText);
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
