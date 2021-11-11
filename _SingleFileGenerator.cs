using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Community.VisualStudio.Toolkit;


/// <summary>
/// 
/// </summary>
namespace XSDCustomToolVSIX
{
    /// <summary>
    /// This class is triggered when the .XSD file is updated. 
    /// VS will call the IVsSingleFileGenerator.Generate() method which triggers this extension to do its thing.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.shell.interop.ivssinglefilegenerator?view=visualstudiosdk-2019</remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("XSDCustomTool", "XSD class generator for use as alternative for MSDataSetGenerator", "1.0")]
    [Guid("83FBB942-657D-4C93-B99E-3F71D4410584")]
    [ComVisible(true)]
    [ProvideObject(typeof(XSDCustomTool))]
    [CodeGeneratorRegistration(typeof(XSDCustomTool), "XSDCustomTool", "{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}", GeneratesDesignTimeSource = true)]
    public sealed class XSDCustomTool : IVsSingleFileGenerator
    {
        //Registers for the other languages
        //[CodeGeneratorRegistration(typeof(XSDexeCustomTool), "XSDCustomTool", ProjectTypes.VB, GeneratesDesignTimeSource = true)]
        //[CodeGeneratorRegistration(typeof(XSDexeCustomTool), "XSDCustomTool", ProjectTypes.J_SHARP, GeneratesDesignTimeSource = true)]
        //[CodeGeneratorRegistration(typeof(XSDexeCustomTool), "XSDCustomTool", ProjectTypes.NODE_JS, GeneratesDesignTimeSource = true)] //Don't know if this is actually correct

        private int MaxStepNumber = 5;
        private int CurrentStepNumber = 0;

        #region Write To Output Pane

        private void Write(string OutputText) => VSTools.WriteOutputPane(OutputText);
        private async Task WriteAsync(string OutputText) => await VSTools.WriteOutputPaneAsync(OutputText);

        private async Task WriteAsync(string OutputText, int currentStepNumber, bool NewLine = false)
        {
            CurrentStepNumber = currentStepNumber;
            await VSTools.WriteOutputPaneAsync($"{(NewLine ? Environment.NewLine : "")}Step {CurrentStepNumber} of {MaxStepNumber} : {OutputText}");
        }

        private void Write(string OutputText, int currentStepNumber, bool NewLine = false)
        {
            CurrentStepNumber = currentStepNumber;
            VSTools.WriteOutputPane($"{( NewLine? Environment.NewLine : "" )}Step {CurrentStepNumber} of {MaxStepNumber} : {OutputText}");
        }

        #endregion Write To Output Pane

        public static string GetDefaultExtension() => "_Parameters.xml";
        
        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = GetDefaultExtension();
            return pbstrDefaultExtension != "" ? VSConstants.S_OK : VSConstants.S_FALSE;
        }

        /// <summary>
        /// This routine is triggered when the XSD file is updated, or by the user right-clicking and selecting "run custom tool". This kicks off the chain reaction that of work this extension does.
        /// </summary>
        /// <param name="wszInputFilePath">This is the file path of the XSD file that VS will submit to this generator</param>
        /// <param name="bstrInputFileContents">If a filepath is not submitted, this can be saubmitted as either a string of binary bits or a string a characters. But since we expect a filepath, this is largely ignored.</param>
        /// <param name="wszDefaultNamespace">The NameSpace the user has set up under the CustomTool in the options pane for the xsd file.</param>
        /// <param name="rgbOutputFileContents">OUT PARAMETER. This is used to return a point in memory for VS to create a file from. Essentially this is byte[]</param>
        /// <param name="pcbOutput">Length of the return file in bytes</param>
        /// <param name="pGenerateProgress">Can report progress to the front end. </param>
        /// <returns></returns>
        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {

            Write("");
            Write(DateTime.Now.ToString() + " -- Starting XSD Conversion with XSDexeCustomTool.");

            byte[] buffer = new byte[0];

            try
            {
                //Step 1: Check if parameter file exists. If not, generate one. 
                Write("Creating / Retrieving Parameter File.", 1);
                XSD_Instance xsdParams = new XSD_Instance(wszInputFilePath, wszDefaultNamespace);

                if (xsdParams.InputFile.Extension != ".xsd")
                    throw new InvalidDataException("Invalid File Type -> Expected an .XSD file.");

                //Step 2 & 3: Run the command. This will also save the output file to the project.
                Write("Generating XSD.exe Command:", 2, false);
                xsdParams.GenerateCommand(out string commandResult);
                Write(commandResult);
                Write("Running XSD.exe Command:", 3, false);
                bool OutputGenerated = xsdParams.Run(out commandResult);
                Write(commandResult);

                //Step 4: Save the parameters and add them to the project.
                Write("Saving parameter file:", 5, false);
                string tmpPath = Path.ChangeExtension(Path.GetTempFileName(), ".txt");
                if (OutputGenerated) xsdParams.SaveXMLToPath(tmpPath); else throw new Exception("XSD.exe Failed during Execution!");

                //Step 5: Make Corrections to the XSD.exe file output 
                //MakeCorrectionsToXSDexeOutputFile(xsdParams);

                //Step 6: Evaluate the output file and generate the helper class if it is missing
                if (xsdParams.XSDexeOptions.Language == XSDCustomTool_ParametersXSDexeOptionsLanguage.CS) // Only run for C# currently, as the other languages are not set up!
                {
                    ParsedFile FileGenerator = ParsedFile.ParsedFileFactory(xsdParams);
                    if (!FileGenerator.HelperClass.FileOnDisk.Exists && OptionsProvider.GetUserDefaults().GenerateHelperClass)
                    {
                        Write("Generating helper class:", 6, false);
                        FileGenerator.HelperClass.Generate();
                    }
                    if (!FileGenerator.Supplement.FileOnDisk.Exists && OptionsProvider.GetUserDefaults().GenerateHelperClass)
                    {
                        Write("Generating Supplement File:", 7, false);
                        FileGenerator.Supplement.Generate();
                    }
                }

                // Pull the fully qualified resource name from the provided assembly
                using (var resource = File.OpenRead(tmpPath)) //assembly.GetManifestResourceStream(InternalResourceName))
                {
                    //Copy the file into a buffer, then pass the buffer out as a result
                    buffer = new byte[resource.Length];
                    resource.Read(buffer, 0, (int)resource.Length);
                }

                //Specify the output vars
                int length = buffer.Length;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(length);
                Marshal.Copy(buffer, 0, rgbOutputFileContents[0], length);
                pcbOutput = (uint)length;

            }
            catch (Exception ex)
            {
                pcbOutput = 0;
                rgbOutputFileContents[0] = new IntPtr();
                Write(Environment.NewLine + "ERROR! -- " + ex.Message);
            }
            finally
            {
                //Delete the Temporary Files
                //Write(Environment.NewLine + "Cleaning up temporary files");
                buffer = null;
            }
            return pcbOutput > 0 ? VSConstants.S_OK : VSConstants.E_FAIL;
        }

        /// <summary>
        /// Read in the class file. The ParseLoop method is called from here.
        /// This will then store the DiscoveredClasses output by the parse loop into to the DiscoveredClasses and TopLevelClass properties.
        /// </summary>
        private void MakeCorrectionsToXSDexeOutputFile(XSD_Instance xsdParams)
        {
            List<string> FileText = new List<string> { };
            string ln;
            bool MustCorrectAttribute = false;
            //Read the file into memory
            using (StreamReader rdr = xsdParams.OutputFile.OpenText())
            {
                do
                {
                    ln = rdr.ReadLine();
                    FileText.Add(ln);
                    if (MustCorrectAttribute)
                    {
                        FileText[FileText.Count - 2] = CorrectAttributeSerialization(FileText[FileText.Count - 2], FileText[FileText.Count - 1], xsdParams);
                        MustCorrectAttribute = false;
                    }
                    else if (ln != null)
                    {
                        MustCorrectAttribute = ln.Contains("Serialization.XmlAttributeAttribute");
                    }
                } while (ln != null);
            }

            //Write the corrections to the generated file
            ln = "";
            foreach (string s in FileText)
                ln += $"{s}\n";
            File.WriteAllText(xsdParams.OutputFile.FullName, ln);
        }

        /// <summary>
        /// XSD.exe by default neglects to allow for serialization of attributes. This results in &lt;Element /&gt; instead of &lt;Element attr="" /&gt; <br/>
        /// This method is meant to be run during the ParseLoop to correct for that. It should be run as: <para/>
        /// <code>FileText[i] = CorrectAttributeSerialization( FileText[i], FileText[i+1] );</code>
        /// <br/> where the return string is input into the array.
        /// </summary>
        /// <param name="attributeLine"></param>
        /// <param name="PropertyLine"></param>
        /// <returns>Turns : [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)] <br/>
        /// into [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "AttributeNameExtractedFromPropertyLine" Form = System.Xml.Schema.XmlSchemaForm.Qualified)]</returns>

        private string CorrectAttributeSerialization(string attributeLine, string PropertyLine, XSD_Instance xsdParams)
        {
            List<string> sp = PropertyLine.Split(' ').ToList();
            bool Properties = xsdParams.XSDexeOptions.ClassOptions.PropertiesInsteadOfFields;
            string PropName;
            switch (xsdParams.XSDexeOptions.Language)
            {
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.CS: 
                    PropName = Properties ? sp[(sp.LastIndexOf("{") - 1)] : sp[(sp.Count - 1)].Replace(";","");
                    return attributeLine.Replace("XmlAttributeAttribute(Form", $"XmlAttributeAttribute(AttributeName=\"{PropName}\", Form");

                case XSDCustomTool_ParametersXSDexeOptionsLanguage.JS: 
                    PropName = sp[(sp.LastIndexOf(":") - 1)];
                    return attributeLine.Replace("XmlAttributeAttribute(Form", $"XmlAttributeAttribute(AttributeName=\"{PropName}\", Form");

                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VJS:
                    PropName = sp[(sp.LastIndexOf(":") - 1)];
                    return attributeLine.Replace("XmlAttributeAttribute(Form", $"XmlAttributeAttribute(AttributeName=\"{PropName}\", Form");

                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VB:
                    PropName = sp[(sp.LastIndexOf("As") - 1)];
                    return attributeLine.Replace("XmlAttributeAttribute(Form", $"XmlAttributeAttribute(AttributeName:=\"{PropName}\", Form");

                default: return attributeLine;
            }

            
        }
    }
}