using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSDCustomToolVSIX.BaseClasses;
using XSDCustomToolVSIX;
using XSDCustomToolVSIX.Interfaces;
using XSDCustomToolVSIX.Language_Specific_Overrides;

namespace XSDCustomToolVSIX.BaseClasses
{
    /// <summary>
    /// Class that reads in an XSD file directly, then generates a class using LINQ style properties.
    /// </summary>
    internal class LinqClassGenerator_Base : CodeGenerator_Base, ILinqClassGenerator
    {
        #region < Class Factory >
        
        protected LinqClassGenerator_Base(IParsedFile parsefFile) : base(parsefFile) { }

        internal static ILinqClassGenerator Factory(IParsedFile parsedFile)
        {
            switch (parsedFile.xSD_Instance.XSDexeOptions.Language)
            {
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.CS:
                    return new LinqOverrides_CSharp(parsedFile);
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VB:
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VJS:
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.JS:
                default:
                    return new LinqClassGenerator_Base(parsedFile);
            }
        }

        #endregion

        

        #region < Properties >

        public override FileInfo FileOnDisk => new FileInfo(ParsedFile.xSD_Instance.InputFile.FullName.Replace(".xsd", $"_LINQ.{ParsedFile.CodeDomObjectProvider.FileExtension}"));
        protected virtual CodeTypeReference XDocType => new CodeTypeReference(typeof(System.Xml.Linq.XDocument));
        protected virtual CodeTypeReference XDocElementType => new CodeTypeReference(typeof(System.Xml.Linq.XElement));

        #endregion

        #region < Methods >

        public override void Generate()
        {
            //Clone the structor that XSD.exe built, stripping away the attributes as needed
            CodeCompileUnit OutputFile = null;
            CodeNamespace PNS = ParsedFile.TargetNameSpace;
            if (PNS == null) return;
            OutputFile = new CodeCompileUnit();

            OutputFile = new CodeCompileUnit();
            CodeNamespace GlobalNameSpace = new CodeNamespace();
            CodeNamespace @namespace = new CodeNamespace(PNS.Name);
            OutputFile.Namespaces.Add(GlobalNameSpace);
            OutputFile.Namespaces.Add(@namespace);
            GlobalNameSpace.Imports.AddRange(PNS.Imports.ToArray());
            GlobalNameSpace.Imports.AddRange(GetAdditionalNameSpaceImports().ToArray());
            @namespace.Comments.AddRange(GetComment_AutoGen());

           

            //Loop through the Discovered Classes, generating classes that utilize LINQ
            foreach (DiscoveredClass dClass in ParsedFile.DiscoveredClasses.Where((DiscoveredClass d) => d.ParsedClass.IsClass && d.ParsedClass.IsPartial))
            {
                CodeTypeDeclaration InClass = dClass.ParsedClass;
                CodeTypeDeclaration @class = new CodeTypeDeclaration()
                {
                    IsPartial = true,
                    Name = InClass.Name,
                    IsClass = InClass.IsClass,
                    IsEnum = InClass.IsEnum,
                    IsInterface = InClass.IsInterface,
                    IsStruct = InClass.IsStruct,
                    Attributes = InClass.Attributes,
                    TypeAttributes = InClass.TypeAttributes
                };

                // for primary class only
                if (dClass == ParsedFile.TopLevelClass)
                {
                    // XML Doc Ref
                    var xmlDoc = CodeProvider.CreateStandard_CodeMemberProperty("XmlFile", XDocType, MemberAttributes.Private, true, "XML File this object represents");
                    @class.Members.Add(xmlDoc);
                    //First Node Ref
                    @class.Members.Add(GenerateLinqProperty("TopLevelNode", XDocElementType, MemberAttributes.Family, ParsedFile.TopLevelClass.Name, xmlDoc));
                }

                Generate_OutputClassComments(@class, dClass);
                Generate_OutputClassConstructors(@class, dClass);

                // Transform the members to LINQ
                CodeTypeMemberCollection members = GenerateLinqClassPrivateMembers(dClass);
                @class.Members.AddRange(members);
                @namespace.Types.Add(@class);
            }
        //Save:
            base.Save(OutputFile, true);
        }

        /// <summary>Generates the comment header of the file that says the file was automatically generated and when it will be overwritten.</summary>
        /// <returns>There are 3 Environment.NewLine keys inserted at the end of this comment.</returns>
        protected override CodeCommentStatementCollection GetComment_AutoGen()
        {
            CodeCommentStatementCollection CommentBlock = new CodeCommentStatementCollection();
            CommentBlock.Add("------------------------------------------------------------------------------");
            CommentBlock.Add($"<auto-generated>");
            CommentBlock.Add($"   This code was generated by XSDCustomTool VisualStudio Extension.");
            CommentBlock.Add($"   This file is only generated if it is missing, so it is safe to modify this file as needed.");
            CommentBlock.Add($"   If the file is renamed or deleted, then it will be regenerated the next time the custom tool is run.");
            CommentBlock.Add($"   This file is meant to load and interact with an XML file using LINQ.");
            CommentBlock.Add($"</auto-generated>");
            CommentBlock.Add($"------------------------------------------------------------------------------");
            CommentBlock.Add($"{NL}");
            return CommentBlock;
        }

        protected virtual CodeNamespaceImportCollection GetAdditionalNameSpaceImports()
        {
            CodeNamespaceImportCollection collection = new CodeNamespaceImportCollection();
            collection.Add(new CodeNamespaceImport("System"));
            collection.Add(new CodeNamespaceImport("System.IO"));
            collection.Add(new CodeNamespaceImport("System.Linq"));
            collection.Add(new CodeNamespaceImport("System.Xml"));
            collection.Add(new CodeNamespaceImport("System.Xml.Linq"));
            collection.Add(new CodeNamespaceImport("System.Xml.Schema"));
            collection.Add(new CodeNamespaceImport("System.Xml.Serialization"));
            collection.Add(new CodeNamespaceImport("System.Collections.Generic"));
            return collection;
        }

        /// <summary>
        /// Add  Summary comment to output class
        /// </summary>
        /// <param name="class">Output Class to add comments to</param>
        /// <param name="dClass">Parsed generated by XSD.exe</param>
        protected virtual void Generate_OutputClassComments(CodeTypeDeclaration @class, DiscoveredClass dClass)
        {
            @class.Comments.Add("<summary>  </summary>", true);
            @class.Comments.Add("<remarks>", true);
            @class.Comments.Add($"Partial Class {dClass.Name} generated by XSD.exe", true);
            @class.Comments.Add("</remarks>", true);
        }

        #region  < Constructors Region>

        /// <summary>
        /// Generate the Constructors for the output class.
        /// </summary>
        /// <param name="class">Output Class to add comments to</param>
        /// <param name="dClass">Parsed Class generated by XSD.exe</param>
        /// <remarks>https://docs.microsoft.com/en-us/dotnet/api/system.codedom.codeconstructor?view=windowsdesktop-5.0</remarks>
        protected virtual void Generate_OutputClassConstructors(CodeTypeDeclaration @class, DiscoveredClass dClass)
        {
            //if (dClass.HasConstructor)
            //{
            //    CodeTypeMember mbr = new CodeTypeMember();
            //    mbr.Comments.Add("XSD.exe has already generated a parameterless constructor for this class.");
            //    @class.Members.Add(mbr);
            //}
            //else
            //{
            //    CodeConstructor Cstr = new CodeConstructor();
            //    Cstr.Comments.Add("<summary> Parameterless Constructor </summary>", true);
            //    Cstr.Attributes = MemberAttributes.Public;
            //    Cstr.Statements.Add(new CodeCommentStatement("TO DO: assign values for all the properties"));
            //    foreach (DiscoveredProperty dProp in dClass.ClassProperties)
            //    {
            //        CodeAssignStatement statement = dProp.GetSupplementInitializer();
            //        if (statement != null) Cstr.Statements.Add(statement);
            //    }
            //    @class.Members.Add(Cstr);
            //}
        }

        #endregion </Constructors Region>

        #region < Generate New Members >

        /// <summary>
        /// Generates Private Members for the Linq Class, such as the XDocument XmlFile {get;} property.
        /// </summary>
        /// <returns></returns>
        private CodeTypeMemberCollection GenerateLinqClassPrivateMembers(DiscoveredClass @class)
        {
            var cls = new CodeTypeDeclaration(@class.Name);
            foreach (DiscoveredProperty prop in @class.ClassProperties)
            {
                cls.Members.Add(GenerateLinqProperty(prop, MemberAttributes.Public, prop.ParentClass.GetCodeTypeMember()));
            }
            return cls.Members;
        }

        protected virtual CodeTypeMember GenerateLinqProperty(DiscoveredProperty dProp, MemberAttributes attributes, CodeTypeMember parentReference, params string[] SummaryComments)
        => GenerateLinqProperty(dProp.Name, dProp.Type, attributes, dProp.Name, parentReference,dProp.IsArrayType, SummaryComments);

        protected virtual CodeTypeMember GenerateLinqProperty(string memberName, CodeTypeReference returnType, MemberAttributes attributes, string elementname, CodeTypeMember parentReference, bool IsArrayType = false, params string[] SummaryComments)
        {
            var prop = new CodeMemberProperty();
            prop.Attributes = attributes;
            prop.Name = memberName;
            prop.Type = XDocElementType;
            prop.Comments.AddRange(CodeProvider.GenerateComment_Summary(SummaryComments));
            prop.SetStatements.Clear();
            prop.HasSet = false;
            prop.HasGet = true;
            prop.GetStatements.Add(new CodeMethodReturnStatement(GenerateElementRequest(parentReference, elementname, IsArrayType)));
            return prop;
        }

        protected virtual CodeExpression GenerateElementRequest(CodeTypeMember parent, string elementname, bool IsArrayType)
        {
            if (IsArrayType)
            {
                return parent.GetCodeMethodInvokeExpression("Element", new CodePrimitiveExpression(elementname));
            }
            else
            {
                return parent.GetCodeMethodInvokeExpression("Element", new CodePrimitiveExpression(elementname));
            }
        }

        #endregion

        #region < Transform Existing Members to utilize LINQ >

        #endregion

        #endregion </ Methods >

    }
}
