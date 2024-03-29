﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using XSDCustomToolVSIX.Interfaces;
using XSDCustomToolVSIX.Language_Specific_Overrides;

namespace XSDCustomToolVSIX.BaseClasses
{
    /// <summary>
    /// The Supplement File is meant to fill in the gaps that XSD.exe left out. 
    /// </summary>
    internal class CodeGenerator_SupplementFile : CodeGenerator_Base, ICodeGenerator_SupplementFile
    {

        #region < Class Factory >

        protected CodeGenerator_SupplementFile(IParsedFile parsefFile) : base(parsefFile) { }

        internal static ICodeGenerator_SupplementFile Factory(IParsedFile parsedFile)
        {
            switch (parsedFile.xSD_Instance.XSDexeOptions.Language)
            {
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.CS:
                    return new SupplementGenerator_CSharp(parsedFile);
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VB:
                    return new SupplementGenerator_VB(parsedFile);
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VJS:
                    return new SupplementGenerator_JSharp(parsedFile);
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.JS:
                    return new SupplementGenerator_JS(parsedFile);
                default:
                    return new CodeGenerator_SupplementFile(parsedFile);
            }
        }

        #endregion

        /// <summary> Location of the AutoGen_Supplement file on disk. </summary>
        public override FileInfo FileOnDisk => new FileInfo(ParsedFile.xSD_Instance.InputFile.FullName.Replace(".xsd", $"_AutoGenerated_Supplement.{ParsedFile.CodeDomObjectProvider.FileExtension}"));

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
            @namespace.Comments.AddRange(GetComment_AutoGen());
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
                CodeTypeMemberCollection @members = @class.Members;
                @namespace.Types.Add(@class);

                Generate_OutputClassComments(@class, dClass);
                Generate_OutputClassConstructors(@class, dClass);
                Generate_ShouldSerializeRegion(@class, dClass);
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
            CommentBlock.Add($"   The base file contains the Load(string), Save(string) methods, several constructors,");
            CommentBlock.Add($"   and several properties to work with the class file generated by XSD.exe.");
            CommentBlock.Add($"</auto-generated>");
            CommentBlock.Add($"------------------------------------------------------------------------------");
            CommentBlock.Add($"{NL}");
            return CommentBlock;
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
            if (dClass.HasConstructor)
            {
                CodeTypeMember mbr = new CodeTypeMember();
                mbr.Comments.Add("XSD.exe has already generated a parameterless constructor for this class.");
                @class.Members.Add(mbr);
            }
            else
            {
                CodeConstructor Cstr = new CodeConstructor();
                Cstr.Comments.Add("<summary> Parameterless Constructor </summary>", true);
                Cstr.Attributes = MemberAttributes.Public;
                Cstr.Statements.Add(new CodeCommentStatement("TO DO: assign values for all the properties"));
                foreach (DiscoveredProperty dProp in dClass.ClassProperties)
                {
                    CodeAssignStatement statement = dProp.GetSupplementInitializer();
                    if (statement != null) Cstr.Statements.Add(statement);
                }
                @class.Members.Add(Cstr);
            }
        }

        #endregion </Constructors Region>

        #region < ShouldSerialize Methods >

        /// <summary>
        /// Create the ShouldSerialize Region and add it to the Members  of  the  specified  OutputClass
        /// </summary>
        /// <param name="class">The OutputClass to add the generated region to</param>
        /// <param name="dClass">The DiscoveredClass whose Properties will be evaluated and converted to ShouldSerialize methods</param>
        protected virtual void Generate_ShouldSerializeRegion(CodeTypeDeclaration @class, DiscoveredClass dClass)
        {
            CodeTypeDeclaration currentClass = dClass.ParsedClass;
            IEnumerable<DiscoveredProperty> dProps = dClass.ClassProperties.Where((DiscoveredProperty p) => p.IsSerializable);
            if (dProps.Count() > 0)
            {
                CodeTypeMember mbr = new CodeTypeMember();
                mbr.StartDirectives.Add(StartRegion("ShouldSerializeProperty"));
                mbr.Comments.AddRange(GetComment_ShouldSerialize());
                @class.Members.Add(mbr);
            }

            foreach (DiscoveredProperty dProp in dProps)
                @class.Members.Add(ShouldSerializeProperty(dProp.Name));

            if (dProps.Count() > 0)
            {
                CodeTypeMember mbr = new CodeTypeMember();
                mbr.EndDirectives.Add(EndRegion("ShouldSerializeProperty"));
                @class.Members.Add(mbr);
            }

        }

        /// <summary>
        /// Generatea new ShouldSerialize[PropertyName] method
        /// </summary>
        /// <param name="PropName"></param>
        /// <returns></returns>
        protected virtual CodeTypeMember ShouldSerializeProperty(string PropName)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Comments.Add(GetShouldSerializerPropertyComment(PropName));
            method.Name = $"ShouldSerialize{PropName}";
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(true)));
            method.Attributes = MemberAttributes.Private;
            return method;
        }

        protected virtual CodeCommentStatement GetShouldSerializerPropertyComment(string PropName) => new CodeCommentStatement($"<summary>Determine when the <see cref=\"{PropName}\"/> property is written to disk during XML Serialization</summary>", true);

        /// <summary>Comment Block to go above the ShouldSerialize Methods</summary>
        /// <returns>There are 3 Environment.NewLine keys inserted at the end of this comment.</returns>
        protected virtual CodeCommentStatementCollection GetComment_ShouldSerialize()
        {
            CodeCommentStatementCollection CommentBlock = new CodeCommentStatementCollection();
            CommentBlock.Add(NL);
            CommentBlock.Add($"ShouldSerialize is run  by the XML Serializer against properties to determine whether to write them to disk.");
            CommentBlock.Add($"The Default functionality (without this method) is to only serialize if the value is changed from the default.");
            CommentBlock.Add($"These methods override that functionality, allowing the programmer to decide when they are serialized.");
            CommentBlock.Add($"To restore original functionality (allowing Serializer to decide for each parameter), comment out these methods.");
            CommentBlock.Add(NL);
            return CommentBlock;
        }

        #endregion </ ShouldSerialize Methods >

        #endregion </ Methods >
    }


}
