using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using XSDCustomToolVSIX.Interfaces;
using XSDCustomToolVSIX.Language_Specific_Overrides;
using System.CodeDom.Compiler;

namespace XSDCustomToolVSIX.BaseClasses
{
    /// <summary>
    /// Contains overrideable methods for creating various objects.  <br/>
    /// These methods are called by all the base classes, so that CodeSnippets may be used in place of the CodeDom objects. <br/>
    /// For example, if you want a one-line CodeMemberProperty as the standard, you have to override the <see cref="CreateNew_CodeMemberProperty"/> call with a CodeSnippet.
    /// </summary>
    internal abstract class CodeDomObjectProvider : ICodeDomObjectProvider
    {

        #region < Class Factory >

        private readonly static CodeDomObjectProvider_CSharp CSharpProvider = new CodeDomObjectProvider_CSharp();
        private readonly static CodeDomObjectProvider_VB VBProvider = new CodeDomObjectProvider_VB();
        private readonly static CodeDomObjectProvider_JS JSProvider = new CodeDomObjectProvider_JS();
        private readonly static CodeDomObjectProvider_JSharp JSharpProvider = new CodeDomObjectProvider_JSharp();
        //private readonly static CodeDomObjectProvider DefaultProvider = new CodeDomObjectProvider();

        internal static ICodeDomObjectProvider Factory(XSDCustomTool_ParametersXSDexeOptionsLanguage lang)
        {
            switch (lang)
            {
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.CS: return CSharpProvider;
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VB: return VBProvider;
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.JS: return JSProvider;
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VJS: return JSharpProvider;
                default: throw new NotImplementedException("Unknown Language Type - Must use language that specifies System.CodeDom.Compiler.CodeDomProvider");  // return DefaultProvider;
            }
        }

        #endregion

        #region < Abstract Properties >

        public abstract XSDCustomTool_ParametersXSDexeOptionsLanguage Language { get; }
        public abstract System.CodeDom.Compiler.CodeDomProvider CodeDomProvider { get; }

        public virtual System.CodeDom.Compiler.CodeGeneratorOptions CodeGeneratorOptions { get; } = new System.CodeDom.Compiler.CodeGeneratorOptions { BlankLinesBetweenMembers = true };

        public string FileExtension => CodeDomProvider.FileExtension;

        #endregion

        public readonly CodeCommentStatementCollection EmptySummaryTag = new CodeCommentStatementCollection { new CodeCommentStatement("<summary>  </summary>", true) };

        public virtual CodeCommentStatement UnableToParseComment => new CodeCommentStatement("Unable to Generate this file -- Language Parser not implemented");

        public virtual string ConvertMemberAttributesToString(MemberAttributes attr)
        {
            string ret = String.Empty;
            if (attr == MemberAttributes.New) ret += "new ";
            if (attr == MemberAttributes.Static) ret += "static ";
            if (attr == MemberAttributes.Const) ret += "const ";
            switch (attr)
            {
                case MemberAttributes.Public: ret += "public"; break;
                case MemberAttributes.Private: ret += "private"; break;
            }
            switch (attr)
            {
                case MemberAttributes.Static: ret += " static"; break;
                case MemberAttributes.Abstract: ret += " abstract"; break;
                case MemberAttributes.Override: ret += " override"; break;
                case MemberAttributes.Final: ret += " final"; break;
            }
            return ret;
        }

        #region < MemberProperty Generation >

        #region No Backing Field


        public virtual CodeTypeMemberCollection CreateNew_CodeMemberProperty(string name, CodeTypeReference type, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null)
            => new CodeTypeMemberCollection { CreateStandard_CodeMemberProperty(name, type, attributes, hasSet, comments) };


        public CodeTypeMemberCollection CreateNew_CodeMemberProperty(string name, string type, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null)
            => CreateNew_CodeMemberProperty(name, new CodeTypeReference(type), attributes, hasSet, comments);


        public CodeMemberProperty CreateStandard_CodeMemberProperty(string name, CodeTypeReference type, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null)
        {
            CodeMemberProperty tmp = new CodeMemberProperty();
            tmp.Attributes = attributes;
            tmp.Comments.AddRange(comments ?? EmptySummaryTag);
            tmp.HasGet = true;
            tmp.HasSet = hasSet;
            tmp.Type = type;
            tmp.Name = name;
            return tmp;
        }

        #endregion

        #region With Backing Field

        public virtual CodeTypeMemberCollection CreateNew_CodeMemberProperty(string name, CodeTypeReference type, CodeMemberField backingField, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null)
            => new CodeTypeMemberCollection { CreateStandard_CodeMemberProperty(name, type, backingField, attributes, hasSet, comments) };

        public CodeMemberProperty CreateStandard_CodeMemberProperty(string name, CodeTypeReference type, CodeMemberField backingField, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null)
        {
            CodeMemberProperty tmp = new CodeMemberProperty();
            tmp.Attributes = attributes;
            tmp.Comments.AddRange(comments ?? EmptySummaryTag);
            tmp.HasGet = true;
            tmp.HasSet = hasSet;
            tmp.GetStatements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), backingField.Name)));
            if (hasSet) tmp.SetStatements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), backingField.Name), new CodePropertySetValueReferenceExpression()));
            tmp.Type = type;
            tmp.Name = name;
            return tmp;
        }

        #endregion

        #endregion </ MemberProperty Generation >

        #region < Method Generation >

        public CodeMemberMethod CreateStandard_CodeMemberMethod(string methodName, CodeTypeReference returnType, MemberAttributes attributes, CodeCommentStatementCollection comments, CodeStatementCollection statements, CodeParameterDeclarationExpressionCollection parameters = null)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Comments.AddRange(comments);
            method.Name = methodName;
            method.ReturnType = returnType;
            method.Attributes = attributes;
            method.Statements.AddRange(statements);
            if (parameters != null) method.Parameters.AddRange(parameters);
            return method;
        }

        /// <inheritdoc cref="CreateStandard_CodeMemberMethod(string, CodeTypeReference, MemberAttributes, CodeCommentStatementCollection, CodeStatementCollection, CodeParameterDeclarationExpressionCollection)"/>
        public CodeMemberMethod CreateStandard_CodeMemberMethod(string methodName, CodeTypeReference returnType, MemberAttributes attributes, CodeCommentStatement comment, CodeStatementCollection statements)
            => CreateStandard_CodeMemberMethod(methodName, returnType, attributes, new CodeCommentStatementCollection { comment }, statements);


        public CodeCommentStatement GetMethodParamComment(CodeParameterDeclarationExpression param, string description) => GetMethodParamComment(param.Name, description);

        public CodeCommentStatement GetMethodParamComment(string paramName, string description) => new CodeCommentStatement($"<param name=\"{paramName}\">{description}</param>");

        #endregion </ Method Generation >

    }
}
