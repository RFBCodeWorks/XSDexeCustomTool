using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;

namespace XSDCustomToolVSIX
{

    internal static class CodeTypeMemberExtensions
    {
        /// <summary> Get a local reference to this object </summary>
        /// <inheritdoc cref="System.CodeDom.CodeVariableReferenceExpression.CodeVariableReferenceExpression(string)"/>
        public static CodeVariableReferenceExpression GetLocalReference(this CodeTypeMember prop) => new CodeVariableReferenceExpression(prop.Name);

        /// <inheritdoc cref="GetCodeMethodReference(string, CodeTypeReference[])"/>
        public static CodeMethodReferenceExpression GetCodeMethodReference(this CodeTypeMember prop, string methodName) => new CodeMethodReferenceExpression(prop.GetLocalReference(), methodName);

        /// <summary> Get a reference to a method contained within this object </summary>
        /// <inheritdoc cref="System.CodeDom.CodeMethodReferenceExpression.CodeMethodReferenceExpression(CodeExpression, string, CodeTypeReference[])"/>
        public static CodeMethodReferenceExpression GetCodeMethodReference(this CodeTypeMember prop, string methodName, params CodeTypeReference[] typeParameters) => new CodeMethodReferenceExpression(prop.GetLocalReference(), methodName, typeParameters);

        /// <summary> Get a reference to a property of this object </summary>
        /// <inheritdoc cref="System.CodeDom.CodePropertyReferenceExpression.CodePropertyReferenceExpression(CodeExpression, string)"/>
        public static CodePropertyReferenceExpression GetCodePropertyReference(this CodeTypeMember prop, string propertyName) => new CodePropertyReferenceExpression(prop.GetLocalReference(), propertyName);

        /// <inheritdoc cref="CodeMethodInvokeExpression.CodeMethodInvokeExpression(CodeExpression, string, CodeExpression[])"/>
        public static CodeMethodInvokeExpression GetCodeMethodInvokeExpression(this CodeTypeMember prop, string methodName, params CodeExpression[] parameters) => new CodeMethodInvokeExpression(GetCodeMethodReference(prop, methodName), parameters);

    }

}
