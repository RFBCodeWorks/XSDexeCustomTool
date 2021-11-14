using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;

namespace XSDCustomToolVSIX.Generate_Helpers
{
    /// <summary>
    /// Contains overrideable methods for creating various objects.  <br/>
    /// These methods are called by all the base classes, so that CodeSnippets may be used in place of the CodeDom objects. <br/>
    /// For example, if you want a one-line CodeMemberProperty as the standard, you have to override the <see cref="CreateNew_CodeMemberProperty"/> call with a CodeSnippet.
    /// </summary>
    internal class ObjectProvider
    {

        /// <summary>Override this method to create a derived CodeGenerator_HelperClass instead of the base CodeGenerator_HelperClass </summary>
        public virtual CodeGenerator_HelperClass HelperClassGenerator(ParsedFile parsedfile) => new CodeGenerator_HelperClass(parsedfile);

        /// <summary>Override this method to create a derived CodeGenerator_SupplementFile instead of the base CodeGenerator_SupplementFile </summary>
        public virtual CodeGenerator_SupplementFile SupplementFileGenerator(ParsedFile parsedfile) => new CodeGenerator_SupplementFile(parsedfile);

        /// <summary>Override this method to create a derived DiscoveredEnum instead of the base DiscoveredEnum </summary>
        public virtual DiscoveredEnum DiscoveredEnumGenerator(CodeTypeDeclaration type, ParsedFile parsedfile) => new DiscoveredEnum(type, parsedfile);

        /// <summary>Override this method to create a derived DiscoveredClass instead of the base DiscoveredClass </summary>
        public virtual DiscoveredClass DiscoveredClassGenerator(CodeTypeDeclaration type, ParsedFile parsedfile) => new DiscoveredClass(type, parsedfile);

        /// <summary>Override this method to create a derived DiscoveredProperty instead of the base DiscoveredProperty </summary>
        public virtual DiscoveredProperty DiscoveredPropertyGenerator(CodeMemberProperty Prop, CodeMemberField backingField, DiscoveredClass parentClass) => new DiscoveredProperty(Prop, backingField, parentClass);

        public CodeCommentStatementCollection EmptySummaryTag = new CodeCommentStatementCollection { new CodeCommentStatement("<summary>  </summary>", true) };

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

        /// <remarks>Overridable methods can return a CodeSnippet Collection</remarks>
        /// <returns>Base Method calls <see cref="CreateStandard_CodeMemberProperty(string, CodeTypeReference, bool, string, bool)"/>, then wraps it into a Collection.</returns>
        /// <inheritdoc cref="CreateStandard_CodeMemberProperty(string, CodeTypeReference, bool, string, bool)"/>
        public virtual CodeTypeMemberCollection CreateNew_CodeMemberProperty(string name, CodeTypeReference type, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null)
            => new CodeTypeMemberCollection{CreateStandard_CodeMemberProperty(name, type, attributes, hasSet, comments) };

        /// <inheritdoc cref="CreateNew_CodeMemberProperty(string, CodeTypeReference, bool, string, bool)"/>
        public CodeTypeMemberCollection CreateNew_CodeMemberProperty(string name, string type, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null)
            => CreateNew_CodeMemberProperty(name, new CodeTypeReference(type), attributes, hasSet, comments);
        
        /// <summary>
        /// Create a new <see cref="CodeMemberProperty"/> object with the supplied properties.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="type">The Type of property (object type)</param>
        /// <param name="attributes"><inheritdoc cref="MemberAttributes"/></param>
        /// <param name="comments">Provide a collection of comments for the property. If left null, uses <see cref="EmptySummaryTag"/></param>
        /// <param name="hasSet"><inheritdoc cref="CodeMemberProperty.HasSet"/></param>
        /// <returns></returns>
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

        /// <remarks>Overridable methods can return a CodeSnippet Collection</remarks>
        /// <returns>Base Method calls <see cref="CreateStandard_CodeMemberProperty(string, CodeTypeReference, CodeMemberField, bool, string, bool, bool)"/>, then wraps it into a Collection.</returns>
        /// <inheritdoc cref="CreateStandard_CodeMemberProperty(string, CodeTypeReference, CodeMemberField, bool, string, bool, bool)"/>
        public virtual CodeTypeMemberCollection CreateNew_CodeMemberProperty(string name, CodeTypeReference type, CodeMemberField backingField, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null)
            => new CodeTypeMemberCollection { CreateStandard_CodeMemberProperty(name, type, backingField, attributes, hasSet, comments) };

        /// <summary>
        /// Create a new <see cref="CodeMemberProperty"/> object with the supplied properties, with an integreated backing field.
        /// </summary>
        /// <param name="backingField">Field for the Get/Set to interact with</param>
        /// <inheritdoc cref="CreateStandard_CodeMemberProperty(string, CodeTypeReference, bool, string, bool, bool)"/>
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

        /// <summary>
        /// Generatea new ShouldSerialize[PropertyName] method
        /// </summary>
        /// <param name="PropName"></param>
        /// <returns></returns>
        public CodeMemberMethod CreateStandard_CodeMemberMethod(string methodName, CodeTypeReference returnType, MemberAttributes attributes, CodeCommentStatementCollection comments, CodeStatementCollection statements, CodeParameterDeclarationExpressionCollection parameters = null)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Comments.AddRange(comments);
            method.Name = methodName;
            method.ReturnType = returnType;
            method.Attributes = attributes;
            method.Statements.AddRange(statements);
            if (parameters != null ) method.Parameters.AddRange(parameters);
            return method;
        }

        public CodeMemberMethod CreateStandard_CodeMemberMethod(string methodName, CodeTypeReference returnType, MemberAttributes attributes, CodeCommentStatement comment, CodeStatementCollection statements)
            => CreateStandard_CodeMemberMethod(methodName, returnType, attributes, new CodeCommentStatementCollection { comment }, statements);

        public CodeCommentStatement GetMethodParamComment(string paramName, string description)
            => new CodeCommentStatement($"<param name=\"{paramName}\">{description}</param>");

        public CodeCommentStatement GetMethodParamComment(CodeParameterDeclarationExpression param, string description)
            => new CodeCommentStatement($"<param name=\"{param.Name}\">{description}</param>");

        #endregion </ Method Generation >

    }
}
