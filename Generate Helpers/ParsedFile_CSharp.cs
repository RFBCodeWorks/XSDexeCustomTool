using System;
using System.IO;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XSDCustomToolVSIX.Generate_Helpers
{
    internal class ParsedFile_CSharp : ParsedFile
    {
        internal ParsedFile_CSharp(XSD_Instance xsd) : base(xsd) { }

        protected override CodeCompileUnit ReadInClassFile()
        {
            CodeCompileUnit output = new CodeCompileUnit();
            List<string> fileText = new List<string>();
            int? OpenBracketIndex = null; int OpenBracketCount = 0; int ClosedBracketCount = 0; int i = 0;
            string className = null;

            CodeNamespace @namespace = new CodeNamespace();
            output.Namespaces.Add(@namespace);
            CodeTypeDeclaration @declaration = null;

            using (StreamReader rdr = this.xSD_Instance.OutputFile.OpenText())
            {
                while (!rdr.EndOfStream)
                {    
                    string ln = rdr.ReadLine();
                    string[] lnSplit = ln.Trim().Split(' ');
                    fileText.Add(ln);
                    if (String.IsNullOrWhiteSpace(ln))
                    { }
                    else if (@declaration == null && ln.Contains("using"))
                    {
                        // Using System.IO; => System.IO
                        @namespace.Imports.Add(new CodeNamespaceImport(ln.Replace("using", "").Replace(";", "").Trim()));
                    }
                    else if (lnSplit[0] == "namespace")
                    {
                        @namespace = new CodeNamespace(lnSplit[1]);
                        output.Namespaces.Add(@namespace);
                    }
                    if (ln.Contains("partial class"))
                    {
                        //Assume no nested classes since this should be output of XSD.exe, which puts all classes at root of namespace
                        i = lnSplit.ToList().IndexOf("class");
                        @declaration = new CodeTypeDeclaration(lnSplit[i + 1])
                        { Attributes = lnSplit[0] == "private" ? MemberAttributes.Private : MemberAttributes.Public,
                          IsClass= true
                        };
                        declaration.Comments.AddRange(GetComments(fileText.ToArray()));
                        declaration.CustomAttributes.AddRange(GetAttributes(fileText.ToArray()));
                        @namespace.Types.Add(declaration);
                        OpenBracketCount = 0;
                        ClosedBracketCount = 0;
                    }
                    else if (className != null && ln.Contains("public") && !(ln.Contains("void") | ln.Contains("event")))
                    {
                        //This is going to be an auto-generated Property
                        string[] arr = ln.Split(' ');
                        int expectedlocation = arr.ToList().IndexOf("public");
                        string PropType = arr[expectedlocation + 1];
                        string PropName = arr[expectedlocation + 2];
                        bool IsSerializableProperty = !classText.Last().Contains("XmlIgnoreAttribute");
                        bool IsGeneratedClassProperty = PropType.Contains(className) | DiscoverClassNames.Contains(PropType); //Indicates this is a class property of another auto-generated class.
                        if (IsSerializableProperty)
                            ClassProperties.Add(new ClassProperty(PropName, PropType, IsGeneratedClassProperty, IsSerializableProperty));
                    }

                    if (className != null) classText.Add(ln);
                    OpenBracketCount += ln.Count(c => c == '{'); // the first instance of should be on the start of the class.
                    ClosedBracketCount += ln.Count(c => c == '}');
                    if (OpenBracketCount == ClosedBracketCount & className != null)
                    {
                        // this should be the end of the class in question
                        SourceClass_CSharp @class = new SourceClass_CSharp(className, classText.ToArray());
                        @class.InnerClasses.AddRange(nestedClasses?.ToList());
                        @class.ClassProperties.AddRange(ClassProperties);
                        DiscoveredClasses.Add(@class);
                        DiscoverClassNames.Add(@class.ClassName);
                        className = null;
                        ClassProperties = new List<ClassProperty>();
                    }
                    else if (OpenBracketCount < ClosedBracketCount) // hit more than closed brackets than open brackets -> end of file.
                        return DiscoveredClasses.ToArray();


                } 
            }

                return output;
        }

        private int FindLastIndex(string[] FileText, string searchterm)
        {
            for (int i = FileText.Length - 1; i > 0; i--)
            {
                if (FileText[i].Trim().StartsWith(searchterm))
                    return i;
            }
            return 0;
        }


        private CodeCommentStatementCollection GetComments(string[] FileText)
        {
            CodeCommentStatementCollection cll = new CodeCommentStatementCollection();
            for (int  i = FindLastIndex(FileText,"//"); i > 0 && FileText[i].Trim().StartsWith("//") ; i--)
            {
                string ln = FileText[i].Trim();
                if (ln.StartsWith("///"))
                    cll.Add(new CodeCommentStatement(ln.Substring(3).Trim()));
                else if (ln.StartsWith("//"))
                    cll.Add(new CodeCommentStatement(ln.Substring(2).Trim()));
            }
            return cll;
        }

        private CodeAttributeDeclarationCollection GetAttributes(string[] FileText)
        {
            CodeAttributeDeclarationCollection cll = new CodeAttributeDeclarationCollection();
            for (int i = FindLastIndex(FileText, "["); i > 0 && FileText[i].Trim().StartsWith("["); i--)
            {
                cll.Add(GetAttributeFromLine(FileText[i].Trim()));
            }
            return cll;
        }

        private CodeAttributeDeclaration GetAttributeFromLine(string line)
        {
            //Ignore the wrapping brackets
            int i = line.IndexOf("(");
            string name = line.Trim().Substring(1, i - 1);
            string[] args = line.Substring(i, line.IndexOf(')', i)).Split(',');
            List<CodeAttributeArgument> arglist = new List<CodeAttributeArgument>();
            foreach (string  a in args)
            {
                CodeExpression ar;
                if (a.Contains("="))
                {
                    ar = null;
                    //This should be able to create statemetns like this: Namespace="", IsNullable=false
                    //But I don't know how. Plus, not superimportant forwhat we are doing.
                    //string[] asp = a.Split('=');
                    //ar = new 
                    //    parameterName: new CodeTypeReferenceExpression(asp[0]),
                    //    right: new CodePrimitiveExpression(asp[1])
                    //    ));
                }
                else
                    ar = new CodePrimitiveExpression(a);
                if (ar != null) arglist.Add(new CodeAttributeArgument(ar));
            }
            return new CodeAttributeDeclaration(name, arglist.ToArray());
        }

    }

}
