using Community.VisualStudio.Toolkit;
using System.ComponentModel;
using System;
using System.Text;

namespace XSDCustomToolVSIX
{
    internal partial class OptionsProvider
    {
        // Register the options with these attributes on your package class:
        //[ProvideOptionPage(typeof(OptionsProvider.XSD_DefaultOptions), "XSDexe_CustomTool", "XSD_Options", 0, 0, true)]
        //[ProvideProfile(typeof(OptionsProvider.XSD_DefaultOptions), "XSDexe_CustomTool", "XSD_Options", 0, 0, true)]
        public class XSD_DefaultOptions : BaseOptionPage<UserDefaultOptions> { }


        /// <inheritdoc cref="BaseOptionModel{T}.Load"/>
        public static UserDefaultOptions GetUserDefaults() { UserDefaultOptions opts = new UserDefaultOptions(); opts.Load(); return opts; }

        /// <inheritdoc cref="XSD_Instance.GetFileOptions(string)" />
        //public static XSD_Instance GetFileOptions(string wszInputFilePath) => XSD_Instance.(wszInputFilePath);

        public static string XSD_Path { get; } = XSD_Instance.FindXSD();

    }

    public class UserDefaultOptions : BaseOptionModel<UserDefaultOptions>
    {

        #region < CustomTool Options >

        //[Category("CustomTool Options")]
        //[DisplayName("Generate Parameter File")]
        //[Description("Output a customizable parameter file that follows the requirements for XSD.exe. This will be used to customize the output of the custom tool that generates the classes." +
        //    "If set to false, then the parameter file will not be used, and the options set within this page will be used instead. (If a parameter file already exists, it will be detected and used, but new ones will not be output.)")]
        //[DefaultValue(true)]
        //public bool GenerateParameterFiles{ get; set; } = true;

        [Category("Helper Class Options")]
        [DisplayName("Generate Helper Class")]
        [Description("Generate a helper class to more easily work with the class file output by XSD.exe. " +
            "\nThis helper class by default has several constructors and a method for Serializing(loading) / Deserializing(saving) an xml file into/from of the class." +
            "\nNOTE: This tool will only generate a helper class for the class output of xsd.exe, not the dataset output." +
            "\nNOTE: Currently only C# is supported for generating a helper class." )]
        [DefaultValue(true)]
        public bool GenerateHelperClass{ get; set; } = true;

        //[Category("Helper Class Options")]
        //[DisplayName("Regenerate Helper Class Automatically")]
        //[Description("The helper class file is meant to be a class that the end developer can customize to work with the classes output by XSD.exe. " +
        //    "Setting this to TRUE will cause the file to be overwritten whenever the CustomTool is run (when the xsd file is updated)" +
        //    "\nDefault Functionality (this set to false) will not perform any changes to the helper class file if it already exists on disk.")]
        //[DefaultValue(false)]
        //public bool OverWriteHelperClass { get; set; } = false;

        //[Category("Helper Class Options")]
        //[DisplayName("Create Nested Classes")]
        //[Description("Experimental feature: When set true, this will loop through all discovered classes found in the XSD.exe output file and generate nested classes within the helper class. ")]
        //[DefaultValue(false)]
        //public bool GenerateNestedClasses { get; set; } = false;

        [Category("LINQ Class Options")]
        [DisplayName("Generate LINQ Class")]
        [Description("Generate a class that houses an XDocument that is accessed using System.XML.LINQ" +
            "\nNOTE: Currently only C# is supported for generating a helper class.")]
        [DefaultValue(true)]
        public bool GenerateLinqClass { get; set; } = true;

        #endregion </ CustomTool Options >

        #region < XSD.exe Options >

        ///// <summary> Specify the output programming language. Default is C# (C-Sharp)</summary>
        //[Category("XSD.exe Options")]
        //[DisplayName("Default Output Language")]
        //[Description("This extension checks your project type's primary language to determine the best output language. \n"  +
        //    "If a match is not found, then this output language will be used.")]
        //[DefaultValue(Enums.SupportedLanguages.CSharp)]
        //public Enums.SupportedLanguages OutputLanguage { get; set; } = Enums.SupportedLanguages.CSharp;


        /// <summary> </summary>
        [Category("XSD.exe Options")]
        [DisplayName("Default Generation Type")]
        [Description("XSD.exe outputs either DataSets or Classes. Select your default choice on what to generate here. (Can be specified per using the _Parameter.xml after the first run against an XSD file). \n \n" +
            "This extension will default to outputting Classes, because the MSDataSetGenerator CustomTool provided by Microsoft as the default CustomTool for XSD files generated DataSets.")]
        [DefaultValue(Enums.Generate.CLASSES)]
        public Enums.Generate GenerateWhat { get; set; } = Enums.Generate.CLASSES;

        /// <summary> Toggles the 'NoLogo' option </summary>
        [Category("XSD.exe Options")]
        [DisplayName("SuppressBanner")]
        [Description("XSD.exe provides a comment at the top of the output file stating it was autogenerated. Set TRUE to disable writing that comment.")]
        [DefaultValue(false)]
        public bool SuppressBanner { get; set; } = false;

        /// <summary> Specify the runtime namespace for the generate types. If not set, the default namespace is "Schemas" </summary>
        [Category("XSD.exe Options")]
        [DisplayName("Default NameSpace")]
        [Description("The NameSpace option should be set for each xsd file that is assigned this CustomTool. If one is not assigned, it will use this NameSpace instead. Leave blank to allow VisualStudio / XSD.exe to generate it for you.")]
        [DefaultValue("")]
        public string DefaultNameSpace { get; set; }

        #endregion </ XSD.exe Options >

        #region < DataSet Options >

        /// <summary> 
        /// Specify that the generated DataSet can be quieried against using LINQ-to-DataSet. 
        /// This option is automatically used when generated a DataSet.
        /// </summary>
        [Category("DataSet Options")]
        [DisplayName("EnableLinqDataSet")]
        [Description("Specify that the generated DataSet can be quieried against using LINQ-to-DataSet. This option is automatically used when generated a DataSet.")]
        [DefaultValue(false)]
        public bool EnableLinqDataSet { get; set; } = false;

        #endregion </ DataSet Options >

        #region < Class Options >

        /// <summary> Generate Field values instead of Property values for the output class(es) </summary>  
        [Category("Class Options")]
        [DisplayName("GenerateFieldsInsteadOfProperties")]
        [Description("XSD.exe defaults to generating Class Properties. Set this to TRUE to generate public Fields instead.")]
        [DefaultValue(false)]
        public bool GenerateFieldsInsteadOfProperties { get; set; } = false;

        /// <summary> If TRUE, implements the INotifiyPropertyChanged interface on all generated types to enable data binding. </summary>
        [Category("Class Options")]
        [DisplayName("EnableDataBinding")]
        [Description("If TRUE, implements the INotifiyPropertyChanged interface on all generated types to enable data binding.")]
        [DefaultValue(true)]
        public bool EnableDataBinding { get; set; } = true;

        /// <summary> If TRUE: Generate explicit order identifiers on all particle members. </summary>
        [Category("Class Options")]
        [DisplayName("Order")]
        [Description("Generate explicit order identifiers on all particle members.")]
        [DefaultValue(false)]
        public bool Order { get; set; } = false;

        #endregion </ Class Options >

    }
}
