using System.CodeDom;

namespace XSDCustomToolVSIX.Interfaces
{    
    /// <summary>
    /// Interface for the Language-specific CodeDomObjectProvider methods
    /// </summary>
    internal interface ICodeDomObjectProvider
    {

        /// <summary>
        /// CodeDomeProvider that provides the ability to Generate and Parse code for some language
        /// </summary>
        System.CodeDom.Compiler.CodeDomProvider CodeDomProvider { get; }

        /// <summary>
        /// Provides Default Options for when writing to a CodeDomCompileUnit to a file
        /// </summary>
        System.CodeDom.Compiler.CodeGeneratorOptions CodeGeneratorOptions { get; }

        /// <summary>
        /// Provide a Comment Statement declaring that the file was unable to be parsed.
        /// </summary>
        CodeCommentStatement UnableToParseComment { get; }

        /// <summary>
        /// Specifies the Language in use
        /// </summary>
        XSDCustomTool_ParametersXSDexeOptionsLanguage Language { get; }

        /// <summary>
        /// <inheritdoc cref="System.CodeDom.Compiler.CodeDomProvider.FileExtension"/>
        /// </summary>
        string FileExtension { get; }

        /// <summary>
        /// Convert a <see cref="MemberAttributes"/> object to its language-specific string representation
        /// </summary>
        /// <param name="attr">MemberAttribute Object to convert</param>
        /// <returns>language-specific string representation of the supplied <paramref name="attr"/></returns>
        string ConvertMemberAttributesToString(MemberAttributes attr);

        #region < Member Collection >

        /// <remarks>Overridable methods can return a CodeSnippet Collection</remarks>
        /// <returns>Base Method calls <see cref="CreateStandard_CodeMemberProperty(string, CodeTypeReference, bool, string, bool)"/>, then wraps it into a Collection.</returns>
        /// <inheritdoc cref="CreateStandard_CodeMemberProperty(string, CodeTypeReference, bool, string, bool)"/>
        CodeTypeMemberCollection CreateNew_CodeMemberProperty(string name, CodeTypeReference type, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null);

        /// <remarks>Overridable methods can return a CodeSnippet Collection</remarks>
        /// <returns>Base Method calls <see cref="CreateStandard_CodeMemberProperty(string, CodeTypeReference, CodeMemberField, bool, string, bool, bool)"/>, then wraps it into a Collection.</returns>
        /// <inheritdoc cref="CreateStandard_CodeMemberProperty(string, CodeTypeReference, CodeMemberField, MemberAttributes, bool, CodeCommentStatementCollection)"/>
        CodeTypeMemberCollection CreateNew_CodeMemberProperty(string name, CodeTypeReference type, CodeMemberField backingField, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null);

        /// <summary>Converts <paramref name="type"/> into a <see cref="CodeTypeReference"/> then calls the overload to generate the return collection.</summary>
        /// <inheritdoc cref="CreateStandard_CodeMemberProperty(string, CodeTypeReference, CodeMemberField, MemberAttributes, bool, CodeCommentStatementCollection)"/>
        CodeTypeMemberCollection CreateNew_CodeMemberProperty(string name, string type, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null);

        #endregion

        #region < Property Generation >

        /// <summary> Create a new <see cref="CodeMemberProperty"/> object with the supplied properties. </summary>
        /// <inheritdoc cref="CreateStandard_CodeMemberProperty(string, CodeTypeReference, CodeMemberField, MemberAttributes, bool, CodeCommentStatementCollection)"/>
        CodeMemberProperty CreateStandard_CodeMemberProperty(string name, CodeTypeReference type, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null);

        /// <summary>
        /// Create a new <see cref="CodeMemberProperty"/> object with the supplied properties, with an integreated backing field.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="type">The Type of property (object type)</param>
        /// <param name="attributes"><inheritdoc cref="MemberAttributes"/></param>
        /// <param name="comments">Provide a collection of comments for the property. If left null, uses <see cref="EmptySummaryTag"/></param>
        /// <param name="hasSet"><inheritdoc cref="CodeMemberProperty.HasSet" path="/summary"/></param>
        /// <param name="backingField">Field for the Get/Set to interact with</param>
        /// <returns></returns>
        CodeMemberProperty CreateStandard_CodeMemberProperty(string name, CodeTypeReference type, CodeMemberField backingField, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null);

        #endregion

        #region < Method Generation >

        /// <summary>
        /// Create a new Method using the supplied parameters
        /// </summary>
        /// <param name="methodName">Method Name</param>
        /// <param name="returnType">Method Return Type</param>
        /// <param name="attributes">Method Attributes</param>
        /// <param name="comments">Method Comments</param>
        /// <param name="statements">Lines to execute within the method</param>
        /// <param name="parameters">Parameters the method accepts</param>
        /// <returns>new <see cref="CodeMemberMethod"/> object</returns>
        CodeMemberMethod CreateStandard_CodeMemberMethod(string methodName, CodeTypeReference returnType, MemberAttributes attributes, CodeCommentStatementCollection comments, CodeStatementCollection statements, CodeParameterDeclarationExpressionCollection parameters = null);

        /// <param name="comment">Method Comment</param>
        /// <inheritdoc cref="CreateStandard_CodeMemberMethod(string, CodeTypeReference, MemberAttributes, CodeCommentStatementCollection, CodeStatementCollection, CodeParameterDeclarationExpressionCollection)"/>
        CodeMemberMethod CreateStandard_CodeMemberMethod(string methodName, CodeTypeReference returnType, MemberAttributes attributes, CodeCommentStatement comment, CodeStatementCollection statements);

        #endregion

        #region < Method Parameter Comments  >

        /// <summary>
        /// Generate a new CodeCommentStatement that describes a parameter for a method
        /// </summary>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="description">Description of the parameter</param>
        /// <returns>Parameter Comment for a Method</returns>
        CodeCommentStatement GetMethodParamComment(string paramName, string description);

        /// <param name="param">Parameter Declaration Object</param>
        /// <inheritdoc cref="GetMethodParamComment(string, string)"/>
        CodeCommentStatement GetMethodParamComment(CodeParameterDeclarationExpression param, string description);

        #endregion

    }
}