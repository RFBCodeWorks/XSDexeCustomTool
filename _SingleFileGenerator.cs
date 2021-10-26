using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using System.Text;
using System.Collections.Generic;
using System.IO;
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

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = "_readme.xml";
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
                if (OutputGenerated) xsdParams.SaveXMLFile(); else throw new Exception("XSD.exe Failed during Execution!");

                //Step 5: Evaluate the output file and generate the helper class if it is missing
                IHelperClass HelperClassFile = GenerateHelperClass_Base.HelperClassFactory(xsdParams);
                if (!HelperClassFile.FileOnDisk.Exists && OptionsProvider.GetUserDefaults().GenerateHelperClass)
                {
                    Write("Generating helper class:", 6, false);
                    HelperClassFile.Generate();
                }

                Write("Writing Readme File:", 7, false);
                // If the project name contains dashes replace with underscores since 
                // namespaces do not permit dashes (underscores will be default to).
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                string resourceName = "Resources.XSDCustomTool_Readme.xml";
                string InternalResourceName = $"XSDCustomToolVSIX.{resourceName}"; //{assembly.GetName().Name.Replace("-", "_")}

                // Pull the fully qualified resource name from the provided assembly
                using (var resource = assembly.GetManifestResourceStream(InternalResourceName))
                {
                    if (resource == null)
                        throw new FileNotFoundException($"Could not find [{resourceName}] in {assembly.FullName}!");

                    //Copy the file into a buffer, then pass the buffer out as a result
                    //C# int has a max of 2147483647, which is over 2GB. If you have a 2GB generated file class, you have other problems.
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
    }
}