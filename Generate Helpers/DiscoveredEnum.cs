using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;

namespace XSDCustomToolVSIX.Generate_Helpers
{
    /// <summary>
    /// Wrapperfor CodeTypeDeclaration where IsEnum is TRUE
    /// </summary>
    internal class DiscoveredEnum
    {
        private DiscoveredEnum() { }
        internal DiscoveredEnum(CodeTypeDeclaration EnumDeclaration)
        {
            foreach (CodeTypeMember member in EnumDeclaration.Members)
                EnumValues.Add(new EnumValue(EnumDeclaration, member));
        }

        /// <inheritdoc cref="CodeTypeDeclaration" />
        protected CodeTypeDeclaration ParsedEnum { get; }

        List<EnumValue> EnumValues { get; } = new List<EnumValue>();

        internal string Type => ParsedEnum.Name;

        /// <summary> Get the first Enum in the list </summary>
        /// <returns></returns>
        internal EnumValue GetFirst() => EnumValues[0];

        /// <summary> Get the first Enum in the list and return its Assignment Expression </summary>
        /// <returns></returns>
        internal CodePropertyReferenceExpression GetDefaultAssignmentExpression => EnumValues[0].GetAssignmentExpression();

        /// <summary>
        /// <inheritdoc cref="List{T}.Find(Predicate{T})"/>
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns></returns>
        internal EnumValue FindEnumByName(string name) => EnumValues.Find((EnumValue e) => e.Name == name);


        /// <summary>
        /// Wrapper for CodeTypeMembers within an Enum 
        /// </summary>
        internal class EnumValue
        {
            CodeTypeMember EnumValueField;
            CodeTypeDeclaration ParentEnumType;

            private EnumValue() { }
            
            internal EnumValue(string name) { EnumValueField = new CodeTypeMember() { Name = name }; }
            internal EnumValue(CodeTypeDeclaration parentEnum, CodeTypeMember mbr) 
            { 
                this.EnumValueField = mbr;
                this.ParentEnumType = parentEnum;
            }

            internal string EnumType => ParentEnumType.Name;
            internal CodeCommentStatementCollection Comments => EnumValueField.Comments;
            internal string Name => EnumValueField.Name;

            /// <summary>
            /// Create a <see cref="CodePropertySetValueReferenceExpression"/> for the Right side of a <see cref="CodeAssignStatement"/>
            /// </summary>
            /// <returns></returns>
            internal CodePropertyReferenceExpression GetAssignmentExpression() =>
                new CodePropertyReferenceExpression(ParentEnumType.GetCodeTypeReferenceExpression(), this.Name);
            
            /// <summary>
            /// Assign the EnumValue to a Type if it is not already Assigned tot that Type
            /// </summary>
            /// <param name="ParentEnumType"></param>
            internal void AssignToEnumType(CodeTypeDeclaration @ParentEnumType)
            {
                if (this.ParentEnumType is null)
                {
                    this.ParentEnumType = @ParentEnumType;
                    ParentEnumType.Members.Add(this.EnumValueField);
                }
            }
            
            /// <summary>
            /// Assign the EnumValue to a Type if it is not already Assigned tot that Type
            /// </summary>
            /// <param name="ParentEnumType"></param>
            internal void AssignToEnumType(DiscoveredEnum discoveredEnum)
            {
                if (this.ParentEnumType is null)
                {
                    ParentEnumType = discoveredEnum.ParsedEnum;
                    ParentEnumType.Members.Add(this.EnumValueField);
                }
            }
        }
    }
}
