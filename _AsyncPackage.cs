using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.Win32;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace XSDCustomToolVSIX
{

    /*
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    /// */
    

    /// <summary>
    /// This custom tool is a wrapper class to run XSD.exe and generate a Class object instead of a DataTable object. <br/>
    /// This was made because XSD files when imported to VS automatically generate DataSets, which can conflict with the classes XSD.exe exports if under the same namespace. <br/>
    /// So instead of outputting the dataset automatically and the class manually, this will simply generate the class file automatically.
    /// </summary>
    [ProvideObject(typeof(XSDexe_CustomToolGenerator))]
    [Guid(XSDexe_CustomToolGenerator.PackageGuidString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("XSDexe_CustomTool",
        "Custom Tool to run XSD.exe to generate Classes or DataSets when an XSD file is saved. Replace \'MSDataSetGenerator\' in the XSD file's CustomTool property to use this tool. ",
        "1.0")]
    [ProvideOptionPage(typeof(OptionsProvider.XSD_DefaultOptions), "XSDexe_CustomTool", "XSD_Options", 0, 0, true)]
    [ProvideProfile(typeof(OptionsProvider.XSD_DefaultOptions), "XSDexe_CustomTool", "XSD_Options", 0, 0, true)]
    public sealed class XSDexe_CustomToolGenerator: AsyncPackage
    {
        /// <summary> XSDexe_CustomTool GUID string. </summary>
        public const string PackageGuidString = "a25f6c79-bd77-40cd-99c9-353d647380e2";
        public const string RegKeyGuidString = "{" + PackageGuidString + "}";

        /// <summary>
        /// Initializes a new instance of the <see cref="XSDexe_CustomToolGenerator"/> class.
        /// </summary>
        public XSDexe_CustomToolGenerator()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
            
        }

        #region < AsyncPackage Package Members >

        protected override WindowPane InstantiateToolWindow(Type toolWindowType)
        {
            return base.InstantiateToolWindow(toolWindowType);
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }

        #endregion
    }
}
