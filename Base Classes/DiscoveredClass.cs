﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace XSDCustomToolVSIX.BaseClasses
{
    /// <summary> This represents a class that was discovered when evaluting the output class file from XSD.exe </summary>
    internal class DiscoveredClass
    {
        
        #region </ Construction >

        private DiscoveredClass() { }

        public DiscoveredClass(CodeTypeDeclaration ClassDeclaration, ParsedFile GeneratedFile)
        {
            this.ParsedClass = ClassDeclaration;
            this.ParsedFile = GeneratedFile;

            List<CodeMemberField> Fields = ParsedClass.Members.OfType<CodeMemberField>().ToList();
            bool HasProperties = ParsedClass.Members.OfType<CodeMemberProperty>().Count() > 0;
            foreach (CodeTypeMember member in ParsedClass.Members)
            {
                switch (true)
                {
                    case true when member.Attributes == MemberAttributes.Private: break; //Ignore

                    case true when member.GetType() == typeof(CodeMemberProperty):
                        //Add the Property to the ClassProperty list
                        CodeMemberProperty prop = (CodeMemberProperty)member;
                        ClassProperties.Add(ParsedFile.CodeDomObjectProvider.DiscoveredPropertyGenerator(prop, TryGetBackingField(prop), this));
                        break;
                    case true when !HasProperties && member.GetType() == typeof(CodeMemberField):
                        //Add the Field to the ClassProperty list, since it should be a public field
                        ClassProperties.Add(ParsedFile.CodeDomObjectProvider.DiscoveredPropertyGenerator(null, (CodeMemberField)member, this));
                        break;
                    case true when member.GetType() == typeof(CodeMemberMethod):
                        //Currently Ignored
                        break;
                    case true when member.GetType() == typeof(CodeMemberEvent):
                        //Currently Ignored
                        break;
                    default: //Ignore (shouldn't occur)
                        break;
                }
            }
        }

        /// <summary>
        /// Check the CodeMemberProperty 
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        private CodeMemberField TryGetBackingField(CodeMemberProperty prop)
        {
            foreach (CodeMemberField fld in this.ParsedClass.Members.OfType<CodeMemberField>())
            {
                if (fld.Name.ToLower() == prop.Name.ToLower()+"field")
                    return fld;
            }
            return null;
        }

        #endregion </ Construction >

        #region < Properties >

        /// <summary>Reference to the ParseFile object this DiscoveredClass was generated from</summary>
        internal ParsedFile ParsedFile { get; }

        /// <inheritdoc cref="CodeTypeDeclaration" />
        internal protected CodeTypeDeclaration ParsedClass { get; }

        /// <summary> This is the name of the class (the 'type' of the class.)</summary>
        public string ClassName => ParsedClass.Name;

        /// <summary> Any properties discovered while parsing the class will be listed here. </summary>
        public List<DiscoveredProperty> ClassProperties { get; } = new List<DiscoveredProperty>();

        /// <summary>If an autogenerated constructor was found during parsing, this value will be true</summary>
        internal bool HasConstructor => HasInstanceConstructor || HasStaticConstructor;

        private bool HasInstanceConstructor => ParsedClass.Members.OfType<CodeConstructor>().Count() > 0;
        private bool HasStaticConstructor => ParsedClass.Members.OfType<CodeTypeConstructor>().Count() > 0;

        /// <summary> This is the name of the property within the Helper class. </summary>
        /// <returns>Base property returns the <see cref="DiscoveredClass.ClassName"/></returns>
        public virtual string HelperClass_PropertyName => this.ClassName;

        #endregion </ Properties >

        #region < CodeDom >

        /// <summary>Returns new CodeTypeReference object for this class</summary>
        /// <returns>new CodeTypeReference(this.ClassName)</returns>
        internal virtual CodeTypeReference GetCodeTypeReference() => new CodeTypeReference(this.ClassName);

        /// <summary>Returns a CodeMemberProperty Object for the HelperClass Property <br/>
        /// Overrides may return a CodeSnippet instead. </summary>
        internal virtual CodeTypeMemberCollection GetHelperClassProperty(bool isPublic = true)
            => ParsedFile.CodeDomObjectProvider.CreateNew_CodeMemberProperty(
                name: this.HelperClass_PropertyName,
                type: GetCodeTypeReference(),
                attributes: MemberAttributes.Public,
                comments: new CodeCommentStatementCollection { GetHelperClassPropertySummary() },
                hasSet: true
                );


        /// <summary>
        /// This is the text to wrap within the &lt;summary&gt; brackets
        /// </summary>
        protected virtual string GetHelperClassPropertySummaryText() => $"This Property Represents the <see cref=\"{ClassName}\"/> XML Object Class";

        /// <summary>
        /// Wrap the result of <see cref="GetHelperClassPropertySummaryText"/> in &lt;summary&gt; brackets and return as a new <see cref="CodeCommentStatement"/>
        /// </summary>
        protected CodeCommentStatement GetHelperClassPropertySummary() => new CodeCommentStatement($"<summary>{GetHelperClassPropertySummaryText()}</summary>",true);

        #endregion </ CodeDom >

        
    }
}
