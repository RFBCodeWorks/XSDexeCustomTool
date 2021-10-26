using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell.Interop;
using HierarchyExt = Microsoft.VisualStudio.Shell.Interop.IVsHierarchyExtensions; // from Community Toolkit
using Microsoft.VisualStudio;
using System.IO;

namespace XSDCustomToolVSIX
{
    /// <summary>
    /// This is a helper class meant to provide easy routines to work with the tools provided within the Community.VisualStuido.Toolkit namespace. <br/>
    /// <see href="https://www.vsixcookbook.com/"/>
    /// </summary>
    static class VSTools
    {

        /// <summary>Gets a TabIndent according to the requested TabLevel. </summary>
        /// <param name="IndentLevel">Insert this many \t characters.</param>
        /// <returns>If <paramref name="IndentLevel"/>&lt;=0, returns <see cref="String.Empty"/>.</returns>
        public static string TabIndent(int IndentLevel)
        {
            string retStr = String.Empty;
            for (int i = 0; i < IndentLevel; i++)
                retStr = retStr + $"\t";
            return retStr;
        }

        #region < Add File to Project >

        /// <inheritdoc cref="AddFileToProjectAsync(FileInfo)"/>
        public static void AddFileToProject(FileInfo file)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            { await AddFileToProjectAsync(file); });
        }

        /// <inheritdoc cref="AddFileToProjectAsync(FileInfo, FileInfo)"/>
        public static void AddFileToProject(FileInfo ParentItem, FileInfo ItemToAdd)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            { await AddFileToProjectAsync(ParentItem,ItemToAdd); });
        }

        /// <summary>Adds a file to the project.</summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task AddFileToProjectAsync(FileInfo file)
        {
            Project project = await VS.Solutions.GetActiveProjectAsync();
            await project.AddExistingFilesAsync(file.FullName);
        }


        /// <summary>Adds a file to the project nested within another file in the solution explorer </summary>
        /// <param name="ParentItem"></param>
        /// <param name="ItemToAdd"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task AddFileToProjectAsync(FileInfo ParentItem, FileInfo ItemToAdd)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            
            PhysicalFile solutionItem = await PhysicalFile.FromFileAsync(ParentItem.FullName);
            //solutionItem.GetItemInfo(out IVsHierarchy heir, out uint itemId, out _);
            //IVsProject proj = (IVsProject)heir;
            //VSADDRESULT[] result = new VSADDRESULT[1];
            //ErrorHandler.ThrowOnFailure(proj.AddItem(itemId, VSADDITEMOPERATION.VSADDITEMOP_LINKTOFILE, string.Empty, 1, new string[]{ ItemToAdd.FullName }, IntPtr.Zero, result));
            //return;
            Project project = await VS.Solutions.GetActiveProjectAsync();
            await project.AddExistingFilesAsync(ItemToAdd.FullName);
            PhysicalFile newItem = await PhysicalFile.FromFileAsync(ItemToAdd.FullName);
            if (newItem != null) await solutionItem.AddNestedFileAsync(newItem);
        }

        #endregion </ Add File to Project >

        #region < Status Bar >

        public static void UpdateProgressBar(int StepNumber, int MaxStepNumber)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            { await UpdateProgressBarAsync(StepNumber, MaxStepNumber); });
        }

        public static async System.Threading.Tasks.Task UpdateProgressBarAsync(int StepNumber, int MaxStepNumber)
        {
            await VS.StatusBar.ShowProgressAsync($"Step {StepNumber} / {MaxStepNumber}", StepNumber, MaxStepNumber);
        }

        #endregion </ Status Bar >

        #region < Output Window Pane >

        public static void ClearOutputPane()
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            { await ClearOutputPaneAsync(); });
        }

        public static async Task ClearOutputPaneAsync()
        {
            if (WindowPaneGUID == null) return;
            OutputWindowPane pane = await GetOutputWindowPaneAsync();
            await pane.ClearAsync();
        }

        public static void WriteOutputPane(string LineText)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate 
            { await WriteOutputPaneAsync(LineText); });
        }

        public static async System.Threading.Tasks.Task WriteOutputPaneAsync(string LineText)
        {
            OutputWindowPane pane = await GetOutputWindowPaneAsync();
            await pane.WriteLineAsync(LineText);
        }

        private static Guid WindowPaneGUID { get; set; }

        public static OutputWindowPane GetOutputWindowPane()
        {
            return ThreadHelper.JoinableTaskFactory.Run(async delegate
            { return await GetOutputWindowPaneAsync(); });
        }

        public static async Task<OutputWindowPane> GetOutputWindowPaneAsync()
        {            
            OutputWindowPane pane;
            if (WindowPaneGUID == null || (pane = await VS.Windows.GetOutputWindowPaneAsync(WindowPaneGUID)) == null)
                pane = await VS.Windows.CreateOutputWindowPaneAsync("XSDexeCustomTool",true);
            WindowPaneGUID = pane.Guid;
            return pane;   
        }

        #endregion </ Output Window Pane >

        #region < IsProjectCompatible >
        
        public static bool IsProjectCompatible()
        {
            return ThreadHelper.JoinableTaskFactory.Run(async delegate
            { return await IsProjectCompatibleLanguageAsync(); });
        }

        public static async Task<bool> IsProjectCompatibleLanguageAsync()
        {
            Project project = await VS.Solutions.GetActiveProjectAsync();
            project.GetItemInfo(out IVsHierarchy projectHierarchy, out _, out _);

            if (projectHierarchy.IsProjectLanguageCSharp() ||
                projectHierarchy.IsProjectLanguageJSharp() ||
                projectHierarchy.IsProjectLanguageVB() ||
                projectHierarchy.IsProjectLanguageJS())
            {
                return true;
            }
            else return false;
        }

        #endregion </ IsProjectCompatible >

        #region < GetProjectLanguage >

        /// <inheritdoc cref="GetProjectLanguage(string, out bool)"/>
        public static Enums.SupportedLanguages GetProjectLanguage(string wszInputFilePath) => GetProjectLanguage(wszInputFilePath, out _);

        /// <inheritdoc cref="GetProjectLanguageAsync"/>
        public static  Enums.SupportedLanguages GetProjectLanguage(string wszInputFilePath, out bool ProjectIsCorrectLanguage)
        {
            ProjectLanguageReturnObject tmp = ThreadHelper.JoinableTaskFactory.Run(async delegate
            { return await GetProjectLanguageAsync(wszInputFilePath).ConfigureAwait(false); });
            ProjectIsCorrectLanguage = tmp.ProjectIsValidLanguage; // Since this tool is only registered for compatible projects, this should always be true!
            return tmp.OutputLanguageSelected;
        }

        /// <summary> Evaluate the project file, then return the appropriate SupportedLanguages enum. </summary>
        /// <param name="wszInputFilePath">This is the XSD file path within the project. </param>
        /// <returns> </returns>
        public static async Task<ProjectLanguageReturnObject> GetProjectLanguageAsync(string wszInputFilePath)
        {
            PhysicalFile solutionItem = await PhysicalFile.FromFileAsync(wszInputFilePath);
            return CheckProjLanguage(solutionItem.ContainingProject);
        }

        public static Enums.SupportedLanguages GetActiveProjectLanguage()
        {
            Project activeProject = ThreadHelper.JoinableTaskFactory.Run(async delegate{ return await VS.Solutions.GetActiveProjectAsync().ConfigureAwait(false); });
            return CheckProjLanguage(activeProject).OutputLanguageSelected;
        }

        public struct ProjectLanguageReturnObject
        {
            public ProjectLanguageReturnObject(bool valid, Enums.SupportedLanguages lang)
            {
                ProjectIsValidLanguage = valid;
                OutputLanguageSelected = lang;
            }
            public readonly bool ProjectIsValidLanguage;
            public readonly Enums.SupportedLanguages OutputLanguageSelected;
        }

        private static ProjectLanguageReturnObject CheckProjLanguage(Project SelectedProject)
        {
            SelectedProject.GetItemInfo(out IVsHierarchy projectHierarchy, out _, out _);
            bool valid = true;
            Enums.SupportedLanguages lang;
            switch (true)
            {
                //Check Main Language Inidicators
                case true when projectHierarchy.IsProjectLanguageCSharp():
                    lang = Enums.SupportedLanguages.CSharp;
                    break;

                case true when projectHierarchy.IsProjectLanguageVB():
                    lang = Enums.SupportedLanguages.VisualBasic;
                    break;

                case true when projectHierarchy.IsProjectLanguageJSharp():
                    lang = Enums.SupportedLanguages.JSharp;
                    break;

                case true when projectHierarchy.IsProjectLanguageJS():
                    lang = Enums.SupportedLanguages.JavaScript;
                    break;

                default:
                    lang = Enums.SupportedLanguages.CSharp; //Default
                    valid = false;
                    break;
            }
            return new ProjectLanguageReturnObject(valid, lang);
        }

        #endregion GetProjectLanguage

        #region < Check Project Languages >

        /// <summary>Checks the project to see if it matches <see cref="ProjectTypes.CSHARP"/> or any other CSHARP project types</summary>
        /// <param name="hierarchy"></param>
        /// <returns></returns>
        private static bool IsProjectLanguageCSharp (this IVsHierarchy hierarchy)
        {
            return hierarchy.IsProjectOfType(
                new string[] {
                    ProjectTypes.CSHARP,
                    ProjectTypes.LEGACY_SMART_DEVICE_CSHARP,
                    ProjectTypes.SHAREPOINT_CSHARP,
                    ProjectTypes.SMART_DEVICE_CSHARP,
                    ProjectTypes.WINDOWS_CSHARP,
                    ProjectTypes.WINDOWS_PHONE_8_CSHARP,
                    ProjectTypes.WORKFLOW_CSHARP }
                );
        }

        /// <summary>Checks the project to see if it matches <see cref="ProjectTypes.VB"/> or any other VB project types</summary>
        /// <param name="hierarchy"></param>
        /// <returns></returns>
        private static bool IsProjectLanguageVB(this IVsHierarchy hierarchy)
        {
            return hierarchy.IsProjectOfType(
                new string[] {
                    ProjectTypes.VB,
                    ProjectTypes.LEGACY_SMART_DEVICE_VB,
                    ProjectTypes.SHAREPOINT_VB,
                    ProjectTypes.SMART_DEVICE_VB,
                    ProjectTypes.WINDOWS_VB,
                    ProjectTypes.WINDOWS_PHONE_8_VB,
                    ProjectTypes.WORKFLOW_VB }
                );
        }

        /// <summary>Checks the project to see if it matches <see cref="ProjectTypes.J_SHARP"/></summary>
        /// <param name="hierarchy"></param>
        /// <returns></returns>
        private static bool IsProjectLanguageJSharp(this IVsHierarchy hierarchy)
        {
            return hierarchy.IsProjectOfType( new string[] { ProjectTypes.J_SHARP } );
        }

        private static bool IsProjectLanguageJS(this IVsHierarchy hierarchy)
        {
            return hierarchy.IsProjectOfType(new string[] { ProjectTypes.NODE_JS });
        }

        /// <summary>Check what kind the project is. Modified from base routine provided by the <see cref="Community.VisualStudio.Toolkit"/></summary>
        /// <param name="hierarchy">The hierarchy instance to check.</param>
        /// <param name="GuidArr">Use the <see cref="ProjectTypes"/> list of strings to create an array of strings to check against.</param>
        /// <remarks>
        /// Ripped from this source then modified for a loop instead of checking a single guid.
        /// https://github.com/VsixCommunity/Community.VisualStudio.Toolkit/blob/9188861227a12ede01730608678cb601eac94bb3/src/toolkit/Community.VisualStudio.Toolkit.Shared/ExtensionMethods/IVsHierarchyExtensions.cs
        /// </remarks> 
        private static bool IsProjectOfType(this IVsHierarchy hierarchy, string[] GuidArr)
        {
            if (hierarchy == null)
                throw new ArgumentNullException(nameof(hierarchy));

            ThreadHelper.ThrowIfNotOnUIThread();

            if (hierarchy is IVsAggregatableProject aggregatable)
                if (ErrorHandler.Succeeded(aggregatable.GetAggregateProjectTypeGuids(out string types)))
                    foreach (string type in types.Split(';'))
                        foreach (string guid in GuidArr)
                            if (Guid.TryParse(type, out Guid identifier) && new Guid(guid).Equals(identifier))
                                return true;
            
            //GUID match not found in the array
            return false;
        }

        #endregion </ Check Project Languages >

        #region < Extract Embedded Resource >

        
        /// <inheritdoc cref="ExtractResource(System.Reflection.Assembly, string, string)"/>
        public static void ExtractResource(string resourceName, string path = null) => System.Reflection.Assembly.GetExecutingAssembly().ExtractResource(resourceName, path);

        /// <summary>
        /// Retrieves the specified [embedded] resource file and saves it to disk.  
        /// If only filename is provided then the file is saved to the default 
        /// directory, otherwise the full filepath will be used.
        /// <para>
        /// Note: if the embedded resource resides in a different assembly use that
        /// assembly instance with this extension method.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        ///       Assembly.GetExecutingAssembly().ExtractResource("Ng-setup.cmd");
        ///       OR
        ///       Assembly.GetExecutingAssembly().ExtractResource("Ng-setup.cmd", "C:\temp\MySetup.cmd");
        /// </code>
        /// </example>
        /// <param name="assembly">The assembly.</param>
        /// <param name="path">This is the output file path - should be fully qualified. </param>
        /// <param name="resourceName">Name of the resource to copy to disk.</param>
        public static void ExtractResource(this System.Reflection.Assembly assembly, string resourceName, string path = null)
        {

            //Construct the full path name for the output file
            bool isFilePath = Path.HasExtension(path); //String.IsNullOrWhiteSpace(path) ? false : 
            bool isRooted = Path.IsPathRooted(path);
            bool isDir = !String.IsNullOrWhiteSpace(path) && !isFilePath;
            string outputFile;

            switch (true)
            {
                case true when isRooted & isFilePath:
                    outputFile = path; break;

                case true when isRooted & isDir:
                    outputFile = Path.Combine(path, resourceName); break;

                case true when isFilePath:  // Treat as Relative Directory/FileName
                    outputFile = Path.Combine(Directory.GetCurrentDirectory(), path); break;
                
                case true when isDir:       // Treat as Relative Directory
                    outputFile = Path.Combine(Directory.GetCurrentDirectory(), path, resourceName); break;
                
                default:
                    outputFile = Path.Combine(Directory.GetCurrentDirectory(), resourceName); break;
            }

            // If the project name contains dashes replace with underscores since 
            // namespaces do not permit dashes (underscores will be default to).
            string InternalResourceName = $"{assembly.GetName().Name.Replace("-", "_")}.{resourceName}";

            // Pull the fully qualified resource name from the provided assembly
            using (var resource = assembly.GetManifestResourceStream(InternalResourceName))
            {
                if (resource == null)
                    throw new FileNotFoundException($"Could not find [{resourceName}] in {assembly.FullName}!");

                using (var file = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
        }

        #endregion </ Extract Embedded Resource >
    }
}