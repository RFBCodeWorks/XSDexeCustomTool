using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using XSDCustomToolVSIX.BaseClasses;

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

        internal static DiscoveredEnum FindEnumByType(this IEnumerable<DiscoveredEnum> enums, string EnumTypeName) => enums.ToList().Find((DiscoveredEnum e) => e.Type == EnumTypeName);

        /// <summary>Add a new CodeSnippetStatement to the Collection</summary>
        public static void Add(this CodeStatementCollection coll, string str) => coll.Add(new CodeSnippetStatement(str));

        public static string ToLower_FirstCharOnly(this string s)
        {
            string r = s.Length > 0 ? s.Substring(0, 1).ToLower() : "";
            string r2 = s.Length > 1 ? s.Substring(1) : "";
            return String.Concat(r, r2);
        }

        public static bool AnyContains(this IEnumerable<string> s, string[] SearchTerms, bool CaseSensitive = true)
        {
            foreach (string L in s)
                if (CaseSensitive)
                {
                    foreach (string ST in SearchTerms)
                        if (L.Contains(ST)) return true;
                }
                else
                {
                    foreach (string ST in SearchTerms)
                        if (L.ToLower().Contains(ST.ToLower())) return true;
                }
            return false;
        }

        public static bool AnyContains(this IEnumerable<string> s, string SearchTerm, bool CaseSensitive = true)
        {
            foreach (string L in s)
                if (
                    (CaseSensitive && L.Contains(SearchTerm)) || 
                    (!CaseSensitive && L.ToLower().Contains(SearchTerm.ToLower()))
                    )
                    return true;
            return false;
        }
        
        public static bool Any(this IEnumerable<string> s, string Searchterm) => s.Any((string l) => l == Searchterm);
        public static bool Any(this IEnumerable<string> s, string Searchterm, bool CaseSensitive) 
            => CaseSensitive ? s.Any(Searchterm) : s.Any((string l) => l.ToLower() == Searchterm.ToLower());

        /// <summary>Check if any item in the arary is any of the following:<br/>
        /// 'protected' <br/>
        /// 'public' <br/>
        /// 'internal' <br/>
        /// 'private'</summary>
        /// <returns></returns>
        public static bool AnyProtectionLevel(this IEnumerable<string> s)
            => s.Any("public", false) || s.Any("protected") || s.Any("internal") || s.Any("private", false);
    }
}
