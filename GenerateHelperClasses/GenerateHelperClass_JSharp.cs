using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XSDCustomToolVSIX
{
    /// <summary>
    /// This class is framed in, but not implemented as I don't know the proper output syntax.
    /// </summary>
    class GenerateHelperClass_JSharp : GenerateHelperClass_Base
    {
        public GenerateHelperClass_JSharp(XSD_Instance xsdSettings) : base(xsdSettings)
        { throw new NotImplementedException("J# Output Helper Not Implemented!"); }

        public override string OutputClassName => base.OutputClassName;

        protected override string CommentIndicator => "#";

        public override FileInfo FileOnDisk => new FileInfo(base.xSD_Instance.InputFile.FullName.Replace(".xsd", "_HelperClass.jsl"));

        protected override string GenerateConstructors(int BaseIndentLevel)
        {
            return String.Empty;
            //string NoArgs = String.Concat(
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// <summary> Construct a new instance of the {OutputClassName} object. </summary>", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}Public {OutputClassName}()", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}{{", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel + 1)} // TO DO: assign values for all the properties", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}}}", Environment.NewLine
            //    );

            //string FilePathArg = String.Concat(
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// <summary> Construct a new instance of the {OutputClassName} object by Deserializing an XML file. </summary>", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// <param name=\"FilePath\"> This XML file to read into the class object </param>", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}Public {OutputClassName}(string FilePath)", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}{{", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel + 1)} {TopLevelClass.HelperClass_PropertyName} = Load(FilePath);", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}}}", Environment.NewLine
            //    );

            //string DeserializedXML = String.Concat(
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// <summary> Construct a new instance of the {OutputClassName} object from an existing  {TopLevelClass.ClassName} object. </summary>", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// <param name=\"{TopLevelClass.HelperClass_PropertyName.ToLower()}\"> A pre-existing {TopLevelClass.ClassName} object.</param>", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}Public {OutputClassName}({TopLevelClass.ClassName} {TopLevelClass.HelperClass_PropertyName.ToLower()})", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}{{", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel + 1)} {TopLevelClass.HelperClass_PropertyName} = Load(FilePath);", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}}}", Environment.NewLine
            //    );

            //return String.Concat(NoArgs, FilePathArg, DeserializedXML);
        }

        /// <summary> Generate a Load(string) method to deserialize an XML file into this helper class. </summary>
        /// <returns></returns>
        protected override string GetClassLoaderMethod(int BaseIndentLevel)
        {
            return String.Empty;
            //string Comments = String.Concat(
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// <summary>", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// Load a file path and produce a Deserialized <typeparamref name=\"{TopLevelClass.ClassName}\"/> Object", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// </summary>", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// <param name=\"FilePath\"> This XML file to read into the class object </param>", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// <returns> A new <typeparamref name=\"{TopLevelClass.ClassName}\"/> object </returns>", Environment.NewLine
            //    );

            //string Method = String.Concat(
            //    $"{VSTools.TabIndent(BaseIndentLevel)}public static {TopLevelClass.ClassName} Load(string FilePath) {{", Environment.NewLine,
            //        $"{VSTools.TabIndent(BaseIndentLevel + 1)}{TopLevelClass.ClassName} retObj = null;", Environment.NewLine,
            //        $"{VSTools.TabIndent(BaseIndentLevel + 1)}try {{", Environment.NewLine,
            //            $"{VSTools.TabIndent(BaseIndentLevel + 2)}using (Stream stream = File.Open(FilePath, FileMode.Open)) {{", Environment.NewLine,
            //                $"{VSTools.TabIndent(BaseIndentLevel + 3)}XmlSerializer serializer = new XmlSerializer(typeof({TopLevelClass.ClassName}));", Environment.NewLine,
            //                $"{VSTools.TabIndent(BaseIndentLevel + 3)}retObj = ({TopLevelClass.ClassName})serializer.Deserialize(stream);", Environment.NewLine,
            //            $"{VSTools.TabIndent(BaseIndentLevel + 2)}}}", Environment.NewLine,
            //        $"{VSTools.TabIndent(BaseIndentLevel + 1)}}} catch (Exception E) {{", Environment.NewLine,
            //            $"{VSTools.TabIndent(BaseIndentLevel + 2)}throw new NotImplementedException(\"Catch Statement Not Implemented. See Inner Error.\", E);", Environment.NewLine,
            //        $"{VSTools.TabIndent(BaseIndentLevel + 1)}}}", Environment.NewLine,
            //        $"{VSTools.TabIndent(BaseIndentLevel + 1)}return retObj;", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}}}", Environment.NewLine
            //    );
            //return String.Concat(Comments, Method);
        }

        /// <summary> Generate a Save(string) method to serialize an XML file from this class. </summary>
        /// <returns></returns>
        protected override string GetClassSaverMethod(int BaseIndentLevel)
        {
            return String.Empty;
            //string Comments = String.Concat(
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// <summary>", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// This method will take the {TopLevelClass.ClassName} object, create an XML serializer for it, and write the XML to the < paramref name = \"FilePath\" />", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// </ summary > ", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// <param name=\"FilePath\"> Destination file path to save the file into. </param>", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}/// <returns> A new <typeparamref name=\"{TopLevelClass.ClassName}\"/> object </returns>", Environment.NewLine
            //    );

            //string Method = String.Concat(
            //    $"{VSTools.TabIndent(BaseIndentLevel)}public void SaveXMLFile(string FilePath) {{", Environment.NewLine,
            //        $"{VSTools.TabIndent(BaseIndentLevel + 1)}try {{", Environment.NewLine,
            //            $"{VSTools.TabIndent(BaseIndentLevel + 2)}Directory.CreateDirectory(new FileInfo(FilePath).DirectoryName);", Environment.NewLine,
            //            $"{VSTools.TabIndent(BaseIndentLevel + 2)}using (Stream stream = File.Open(FilePath, FileMode.Create)) {{", Environment.NewLine,
            //                $"{VSTools.TabIndent(BaseIndentLevel + 3)}XmlSerializer serializer = new XmlSerializer(typeof({TopLevelClass.ClassName}));", Environment.NewLine,
            //                $"{VSTools.TabIndent(BaseIndentLevel + 3)}serializer.Serialize(stream, this.{TopLevelClass.HelperClass_PropertyName});", Environment.NewLine,
            //                $"{VSTools.TabIndent(BaseIndentLevel + 3)}stream.Flush();", Environment.NewLine,
            //            $"{VSTools.TabIndent(BaseIndentLevel + 2)}}}", Environment.NewLine,
            //        $"{VSTools.TabIndent(BaseIndentLevel + 1)}}} catch (Exception E) {{", Environment.NewLine,
            //            $"{VSTools.TabIndent(BaseIndentLevel + 2)}throw new NotImplementedException(\"Catch Statement Not Implemented. See Inner Error.\", E);", Environment.NewLine,
            //        $"{VSTools.TabIndent(BaseIndentLevel + 1)}}}", Environment.NewLine,
            //    $"{VSTools.TabIndent(BaseIndentLevel)}}}", Environment.NewLine
            //    );
            //return String.Concat(Comments, Method);
        }

        /// <summary>Generate the Class Tree of nested helper classes within this helper class</summary>
        /// <param name="BaseIndentLevel"></param>
        /// <returns></returns>
        protected override string GenerateClassTree(int BaseIndentLevel) => TopLevelClass.BuildClassTree(BaseIndentLevel);

        public override void Generate()
        {
            throw new NotImplementedException();
        }

        protected override SourceClass[] ParseLoop(string[] txt, int StartIndex)
        {
            throw new NotImplementedException();
        }
    }
    
    class SourceClass_JSharp : SourceClass
    {
        /// <inheritdoc cref="SourceClass.SourceClass(string)"/>
        public SourceClass_JSharp(string className, string[] classText) : base(className, classText) { }

        /// <inheritdoc cref="SourceClass.SourceClass(string, bool)"/>
        public SourceClass_JSharp(string className, string[] classText, bool isTopLevelNode) : base(className, classText, isTopLevelNode) { }

        public override Enums.SupportedLanguages ClassLanguage => Enums.SupportedLanguages.CSharp;

        /// <returns>"{Public/Private} {ClassName} {PropertyName} {get;set;}"</returns>
        /// <inheritdoc cref="GetPropertyString(int, bool)"/>
        public override string GetPropertyString(int IndentLevel, bool IsPublic = true)
            => throw new NotImplementedException(); //=> $"{(IsPublic ? "public" : "private")} {VSTools.TabIndent(IndentLevel)}{ClassName} {HelperClass_PropertyName} {{ get; {(IsPublic ? "private " : "")} set; }}";

        /// <inheritdoc cref="GetConstructors(int)"/>
        public override string GetConstructors(int IndentLevel)
            => throw new NotImplementedException(); //=> $"{VSTools.TabIndent(IndentLevel)}public {this.ClassName}() {{}}";

        /// <inheritdoc cref="BuildClassTree(int)"/>
        public override string BuildClassTree(int IndentLevel)
            => throw new NotImplementedException(); 
        /*
        {
            string properties = String.Empty;
            string nested = String.Empty;

            foreach (SourceClass IC in InnerClasses)
                properties = String.Concat(properties, IC.GetPropertyString(IndentLevel + 1), Environment.NewLine);

            foreach (SourceClass IC in InnerClasses)
                nested = String.Concat(nested, IC.BuildClassTree(IndentLevel + 1), Environment.NewLine);

            string thisclass = String.Concat(
                $"{VSTools.TabIndent(IndentLevel)}#region < {this.ClassName} >", Environment.NewLine,
                this.GetConstructors(IndentLevel + 1), Environment.NewLine,

                $"{VSTools.TabIndent(IndentLevel)}#region < Nested Class Objects Properties >", Environment.NewLine,
                properties, Environment.NewLine,
                $"{VSTools.TabIndent(IndentLevel)}#endregion </ Nested Class Objects Properties >", Environment.NewLine, Environment.NewLine,

                $"{VSTools.TabIndent(IndentLevel)}#region < Nested Classes >", Environment.NewLine,
                nested, Environment.NewLine,
                $"{VSTools.TabIndent(IndentLevel)}#endregion </ Nested Classes >", Environment.NewLine,

                $"{VSTools.TabIndent(IndentLevel)}#endregion </ {this.ClassName} >"
                );

            return thisclass;
        }
        */


    }
}
