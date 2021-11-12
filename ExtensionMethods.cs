using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using XSDCustomToolVSIX.Generate_Helpers;

namespace XSDCustomToolVSIX
{
    public static class ExtensionMethods
    {
        /// <inheritdoc cref="Add(CodeCommentStatementCollection, string, bool)"/>
        public static int Add(this CodeCommentStatementCollection Collection, string text)
            => Collection.Add(new CodeCommentStatement(text));

        /// <inheritdoc cref="CodeCommentStatementCollection.Add(CodeCommentStatement)"/>
        /// <inheritdoc cref="CodeComment.CodeComment(string, bool)"/>
        public static int Add(this CodeCommentStatementCollection Collection, string text, bool docComment)
            => Collection.Add(new CodeCommentStatement(text, docComment));

        public static CodeNamespaceImport[] ToArray(this CodeNamespaceImportCollection collection)
        {
            List<CodeNamespaceImport> arr = new List<CodeNamespaceImport>();
            foreach (CodeNamespaceImport c in collection)
                arr.Add(c);
            return arr.ToArray();
        }

        public static CodeTypeDeclaration[] ToArray(this CodeTypeDeclarationCollection collection)
        {
            CodeTypeDeclaration[] arr = new CodeTypeDeclaration[collection.Count];
            collection.CopyTo(arr, 0);
            return arr;
        }

        public static bool Any(this CodeTypeDeclarationCollection collection, Func<CodeTypeDeclaration,bool> Predicate) => collection.ToArray().Any(Predicate);

        public static IEnumerable<CodeTypeDeclaration> Where(this CodeTypeDeclarationCollection collection, Func<CodeTypeDeclaration, bool> Predicate)
            => collection.ToArray().Where(Predicate);

        public static CodeTypeReferenceExpression GetCodeTypeReferenceExpression(this CodeTypeDeclaration dec) => new CodeTypeReferenceExpression(dec.Name);

        internal static DiscoveredEnum FindEnumByType(this IEnumerable<DiscoveredEnum> enums, string EnumTypeName) => enums.Single((DiscoveredEnum e) => e.Type == EnumTypeName);

        public static string ToLower_FirstCharOnly(this string s)
        {
            string r = s.Length > 0 ? s.Substring(0, 1).ToLower() : "";
            string r2 = s.Length > 1 ? s.Substring(1) : "";
            return String.Concat(r, r2);
        }

    }
}
