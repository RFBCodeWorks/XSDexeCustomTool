using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace XSDCustomToolVSIX
{
    /// <summary>
    /// This class takes in the parameters, builds an export parameter file, and generates the command to run XSD.exe.
    /// </summary>
    public partial class XSD_Instance : XSDCustomTool_Parameters
    {

        #region < XSD Path Locator >

        internal static string XSD_Path { get; private set; } = OptionsProvider.XSD_Path;
        private static string ProgFiles => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        private static string ProgFilesX86 => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        private const string SubDir_1 = "\\Microsoft SDKs\\Windows\\";

        #endregion </ XSD Path Locator >

        #region < Constants & Enums >

        private const string GENERATE_CLASSES = " /classes";
        private const string GENERATE_DATASET = " /dataset";
        //private const string OUTFOLDER = " /out:{0}"; // DO NOT USE -> Use OUTFOLDER instead
        private const string LANGUAGE = " /language:\"{0}\"";
        private const string NAMESPACE = " /namespace:\"{0}\"";

        private const string OPT_ELEMENT = " /element\"{0}\"";
        private const string OPT_URI = " /uri:\"{0}\"";

        //Bool Options
        private const string OPT_ENABLEDATABINDING = " /enableDataBinding";
        private const string OPT_ENABLELINQDATASET = " /enableLinqDataSet";
        private const string OPT_FIELDS = " /fields";
        private const string OPT_SUPPRESSBANNER = " /nologo";
        private const string OPT_ORDER = " /order";

        internal const string INFILE = " {0} ";
        internal const string OUTFOLDER = " -outputdir:{0}";
        internal const string PARAMETERS = " /parameters:{0}"; // Must point to some ParameterFile.xml

        #endregion </ Constants & Enums >


        /// <summary>
        /// Generate a new command and run it. This will generate the output file, and add it to the project.
        /// </summary>
        /// <param name="OutputText">The results of the command will be output here. If an error is throw, Error.Message is reported here as well. </param>
        /// <returns>TRUE if the run was successful and no errors occured. Otherwise False. </returns>
        public bool Run(out string OutputText)
        {
            if (GenerateCommand(out string cmd))
            {
                OutputText = "";
                bool success = false;
                try
                {
                    VSTools.WriteOutputPaneAsync("XSD.exe Location: ").ConfigureAwait(false);
                    VSTools.WriteOutputPaneAsync(XSD_Path).ConfigureAwait(false);
                    VSTools.WriteOutputPaneAsync("Command Options: ").ConfigureAwait(false);
                    VSTools.WriteOutputPaneAsync(cmd).ConfigureAwait(false);

                    if (tmpInputFile.Exists) tmpInputFile.Delete();
                    InputFile.CopyTo(tmpInputFile.FullName);

                    Process XPro = new Process();
                    XPro.StartInfo.FileName = XSD_Path;
                    XPro.StartInfo.Arguments = cmd;
                    XPro.StartInfo.UseShellExecute = false;
                    XPro.StartInfo.RedirectStandardOutput = true;
                    //XPro.OutputDataReceived += XPro_OutputDataReceived;
                    XPro.Start();
                    OutputText = XPro.StandardOutput.ReadToEnd();
                    XPro.WaitForExit();

                    tmpOutputFile.Refresh();
                    success = tmpOutputFile.Exists;
                    if (success)    // Copy to the output file location, then add to the project.
                    {
                        OutputFile.Refresh();
                        if (OutputFile.Exists) OutputFile.Delete();
                        tmpOutputFile.CopyTo(OutputFile.FullName);
                        OutputFile.Refresh();
                        success = OutputFile.Exists;
                        if (success) VSTools.AddFileToProject(InputFile, OutputFile);
                    }
                }
                catch (Exception e)
                {
                    OutputText += Environment.NewLine + e.Message;
                    success = false;
                }
                finally
                {
                    //delete the tmp files that are no longer needed
                    try { tmpOutputFile.Delete(); } catch { }
                    try { tmpInputFile.Delete(); } catch { }
                }
                return success;
            }
            else
                OutputText = cmd;
                return false;
        }

        /// <summary>
        /// Compiles the currently set up options and formats it to return a string to be appended onto the command line.
        /// </summary>
        /// <returns>Returns the string of ARGUMENTS ready to be appended to a command line that calls out XSD.exe's location.  </returns>
        public bool GenerateCommand(out string result)
        {
            result = "";
            result += String.Format(INFILE, WrapPath(tmpInputFile.FullName));    //This is the input xsd file
            result += String.Format(OUTFOLDER, WrapPath(tmpOutputFile.DirectoryName)); //Set Output Directory

            result += (this.XSDexeOptions.GenerateClass) ? GENERATE_CLASSES : GENERATE_DATASET;
            result += String.Format(LANGUAGE, this.XSDexeOptions.Language);
            result += this.XSDexeOptions.NoLogo ? OPT_SUPPRESSBANNER : "";
            if (this.XSDexeOptions.GenerateClass)
            {
                result += this.XSDexeOptions.ClassOptions.EnableDataBinding ? OPT_ENABLEDATABINDING : "";
                result += this.XSDexeOptions.ClassOptions.Order ? OPT_ORDER : "";
                result += !this.XSDexeOptions.ClassOptions.PropertiesInsteadOfFields ? OPT_FIELDS : "";
            }
            else
            {
                result += this.XSDexeOptions.DataSetOptions.EnableLinqDataSet ? OPT_ENABLELINQDATASET : "";
            }
            result += String.IsNullOrWhiteSpace(this.XSDexeOptions.NameSpace) ? "" : String.Format(NAMESPACE, this.XSDexeOptions.NameSpace);
            result += String.IsNullOrWhiteSpace(URI) ? "" : String.Format(OPT_URI, URI);
            foreach (string el in this.ElementsToGenerateCodeFor)
                result += String.IsNullOrWhiteSpace(el) ? "" : String.Format(OPT_ELEMENT, el);

            return true;
        }

        /// <summary> Method to check if a path needs to be wrapped in quotes</summary>
        /// <returns> 
        /// If the path has a space in it and does not start with a quotation mark, it will wrap the input string in quotations and return the new string. 
        /// <br/> If the path does not require wrapping, return the input string.
        /// </returns>
        internal static string WrapPath(string PathString) => PathString.Contains(" ") && !PathString.StartsWith("\"") ? '\"' + PathString + '\"' : PathString;

        /// <summary>
        /// return the first instance of XSD.exe found
        /// </summary>
        /// <returns></returns>
        public static string FindXSD()
        {
            //C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\XSD.exe
            //C:\Program Files(x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64\XSD.exe
            DirectoryInfo Test = new DirectoryInfo(Path.Combine(ProgFilesX86 + SubDir_1));
            bool SearchAgain = false;
        Search:
            SearchAgain = !SearchAgain;
            DirectoryInfo[] Dirs = Test.GetDirectories();
            foreach (DirectoryInfo unknownDir in Dirs) // Checking the windows versions folders
            {
                DirectoryInfo bin = unknownDir?.GetDirectories()?.Single(binFolder => binFolder.Name == "bin");
                if (bin != null) // Checking within the bin folders
                {
                    foreach (DirectoryInfo dir in bin.GetDirectories("NETFX*Tools"))
                    {
                        foreach (FileInfo f in dir.GetFiles("XSD.exe"))
                            return f.FullName;
                        foreach (DirectoryInfo subdir in dir.GetDirectories("\x64"))
                            foreach (FileInfo f in dir.GetFiles("XSD.exe"))
                                return f.FullName;
                    }
                }
            }
            if (SearchAgain)
            {
                Test = new DirectoryInfo(Path.Combine(ProgFiles + SubDir_1));
                goto Search;
            }
            return "";
        }

    }
}
