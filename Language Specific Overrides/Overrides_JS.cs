using System;
using System.IO;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XSDCustomToolVSIX.Generate_Helpers
{
    internal class ObjectProvider_JS : ObjectProvider
    {
        public override CodeGenerator_HelperClass HelperClassGenerator(ParsedFile parsedfile) => new HelperClassGenerator_JS(parsedfile);
        public override CodeGenerator_SupplementFile SupplementFileGenerator(ParsedFile parsedfile) => new SupplementGenerator_JS(parsedfile);
        public override DiscoveredEnum DiscoveredEnumGenerator(CodeTypeDeclaration type, ParsedFile parsedfile) => new DiscoveredEnum(type, parsedfile);
        public override DiscoveredClass DiscoveredClassGenerator(CodeTypeDeclaration type, ParsedFile parsedfile) => new DiscoveredClass(type, parsedfile);
        public override DiscoveredProperty DiscoveredPropertyGenerator(CodeMemberProperty Prop, CodeMemberField backingField, DiscoveredClass parentClass) => new DiscoveredProperty(Prop, backingField, parentClass);
        public override CodeCommentStatement UnableToParseComment => new CodeCommentStatement("Unable to automatically generate this file -- JavaScript Language Parser not implemented yet.");

        public override CodeTypeMemberCollection CreateNew_CodeMemberProperty(string name, CodeTypeReference type, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null)
        {
            //CodeSnippetTypeMember mbr; //cannot have property with SetOnly method
            //string attr = ConvertMemberAttributesToString(attributes);
            //if (hasSet)
            //    mbr = new CodeSnippetTypeMember($"        {attr} {type.BaseType} {name} {{ get; set; }}");
            //else
            //    mbr = new CodeSnippetTypeMember($"        {attr} {type.BaseType} {name} {{ get; }}");
            //mbr.Comments.AddRange(comments ?? EmptySummaryTag);
            //return new CodeTypeMemberCollection { mbr };
            return base.CreateNew_CodeMemberProperty(name, type, attributes, hasSet, comments);
        }

        public override CodeTypeMemberCollection CreateNew_CodeMemberProperty(string name, CodeTypeReference type, CodeMemberField backingField, MemberAttributes attributes = MemberAttributes.Public, bool hasSet = true, CodeCommentStatementCollection comments = null)
        {
            //CodeTypeMemberCollection cll = new CodeTypeMemberCollection();
            //CodeSnippetTypeMember mbr;
            //string attr = ConvertMemberAttributesToString(attributes);
            //if (!hasSet)
            //{
            //    mbr = new CodeSnippetTypeMember($"        {attr} {type.BaseType} {name} => this.{backingField.Name};");
            //    mbr.Comments.AddRange(comments ?? EmptySummaryTag);
            //    cll.Add(mbr);
            //}
            //else
            //{
            //    mbr = new CodeSnippetTypeMember($"        {attr} {type.BaseType} {name}");
            //    mbr.Comments.AddRange(comments ?? EmptySummaryTag);
            //    cll.Add(mbr);
            //    cll.Add(new CodeSnippetTypeMember($"        {{"));
            //    cll.Add(new CodeSnippetTypeMember($"            get => this.{backingField.Name};"));
            //    cll.Add(new CodeSnippetTypeMember($"            set => this.{backingField.Name} = value;"));
            //    cll.Add(new CodeSnippetTypeMember($"        }}"));
            //}
            //return cll;
            return base.CreateNew_CodeMemberProperty(name, type, backingField, attributes, hasSet, comments);
        }

    }

    internal class SupplementGenerator_JS : CodeGenerator_SupplementFile
    {
        internal SupplementGenerator_JS(ParsedFile ParsedFile) : base(ParsedFile) { }

        protected override CodeTypeMember ShouldSerializeProperty(string PropName)
        {
            return base.ShouldSerializeProperty(PropName);

            //CodeSnippetTypeMember mbr = new CodeSnippetTypeMember($"        private bool ShouldSerialize{PropName}() => true;");
            //mbr.Comments.Add(GetShouldSerializerPropertyComment(PropName));
            //return mbr;
        }

    }

    internal class HelperClassGenerator_JS : CodeGenerator_HelperClass
    {
        internal HelperClassGenerator_JS(ParsedFile parsedFile) : base(parsedFile) { }

        protected override CodeStatementCollection GetClassLoaderMethodStatements()
        {
            CodeStatementCollection tmp = new CodeStatementCollection();
            //MiscData retObj = null;
            //try
            //{
            //    using (Stream stream = File.Open(FilePath, FileMode.Open))
            //    {
            //        XmlSerializer serializer = new XmlSerializer(typeof(MiscData));
            //        retObj = (MiscData)serializer.Deserialize(stream);
            //    }
            //}
            //catch (Exception E)
            //{
            //    throw new NotImplementedException("Catch Statement Not Implemented. See Inner Error.", E);
            //}
            //return retObj;

            //string tab = "    ";
            //string baseTab = $"{tab}{tab}{tab}";
            //tmp.Add($"{baseTab}{TopLevelClass.ClassName} retObj = null;");
            //tmp.Add($"{baseTab}try");
            //tmp.Add($"{baseTab}{{");
            //tmp.Add($"{baseTab}{tab}using (Stream stream = File.Open(FilePath, FileMode.Open))");
            //tmp.Add($"{baseTab}{tab}{{");
            //tmp.Add($"{baseTab}{tab}{tab}XmlSerializer serializer = new XmlSerializer(typeof({TopLevelClass.ClassName}));");
            //tmp.Add($"{baseTab}{tab}{tab}retObj = ({TopLevelClass.ClassName})serializer.Deserialize(stream);");
            //tmp.Add($"{baseTab}{tab}}}");
            //tmp.Add($"{baseTab}}}");
            //tmp.Add($"{baseTab}catch (Exception E)");
            //tmp.Add($"{baseTab}{{");
            //tmp.Add($"{baseTab}{tab} throw E;");
            //tmp.Add($"{baseTab}}}");
            //tmp.Add($"{baseTab}return retObj;");

            //return tmp;

            return base.GetClassLoaderMethodStatements();
        }

        protected override CodeStatementCollection GetClassSaverMethodStatements()
        {
            CodeStatementCollection tmp = new CodeStatementCollection();
            //try
            //{
            //    Directory.CreateDirectory(new FileInfo(FilePath).DirectoryName);
            //    using (Stream stream = File.Open(FilePath, FileMode.Create))
            //    {
            //        XmlSerializer serializer = new XmlSerializer(typeof(MiscData));
            //        serializer.Serialize(stream, this.MiscData_Property);
            //        stream.Flush();
            //    }
            //}
            //catch (Exception E)
            //{
            //    throw new NotImplementedException("Catch Statement Not Implemented. See Inner Error.", E);
            //}

            //string tab = "    ";
            //string baseTab = $"{tab}{tab}{tab}";
            //tmp.Add($"{baseTab}try");
            //tmp.Add($"{baseTab}{{");
            //tmp.Add($"{baseTab}{tab}Directory.CreateDirectory(new FileInfo(FilePath).DirectoryName);");
            //tmp.Add($"{baseTab}{tab}using (Stream stream = File.Open(FilePath, FileMode.Create))");
            //tmp.Add($"{baseTab}{tab}{{");
            //tmp.Add($"{baseTab}{tab}{tab}XmlSerializer serializer = new XmlSerializer(typeof({TopLevelClass.ClassName}));");
            //tmp.Add($"{baseTab}{tab}{tab}serializer.Serialize(stream, this.{TopLevelClass.HelperClass_PropertyName});");
            //tmp.Add($"{baseTab}{tab}{tab}stream.Flush();");
            //tmp.Add($"{baseTab}{tab}}}");
            //tmp.Add($"{baseTab}}}");
            //tmp.Add($"{baseTab}catch (Exception E)");
            //tmp.Add($"{baseTab}{{");
            //tmp.Add($"{baseTab}{tab} throw E;");
            //tmp.Add($"{baseTab}}}");

            //return tmp; 
            return base.GetClassSaverMethodStatements();
        }
    }

    internal class ParsedFile_JS : ParsedFile
    {
        internal ParsedFile_JS(XSD_Instance xsd) : base(xsd) { }

        #region < ReadInClassFile >

        protected override CodeCompileUnit ReadInClassFile()
        {
            return null;

            string[] FileText = ReadFileToStringArray();
            int i = 0;
            CodeCompileUnit output = new CodeCompileUnit();
            CodeNamespace @namespace = new CodeNamespace(); // Empty Namespace -> WIll be remade once parsed
            CodeNamespace GlobalNameSpace = new CodeNamespace();
            output.Namespaces.Add(GlobalNameSpace);
            try
            {
                //Read the FileText Into NameSpaces, Classes, and Enums
                for (i = 0; i < FileText.Length - 1; i++)
                {
                    string ln = FileText[i];
                    string[] lnSplit = ln.Trim().Split(' ');
                    if (String.IsNullOrWhiteSpace(ln))
                    { }
                    else if (ln.Contains("using"))
                    {
                        // Using System.IO; => System.IO
                        GlobalNameSpace.Imports.Add(new CodeNamespaceImport(ln.Replace("using", "").Replace(";", "").Trim()));
                    }
                    else if (lnSplit[0] == "namespace")
                    {
                        @namespace = new CodeNamespace(lnSplit[1]);
                        output.Namespaces.Add(@namespace);
                    }
                    else if (ln.ToLower().Contains("enum"))
                    {
                        EnumText @enum = new EnumText();
                        @enum.Text = ReadBracketGroup(FileText, i, out int ResumeIndex);
                        @namespace.Types.Add(@enum.ProcessEnumText());
                        i = ResumeIndex;
                    }
                    else if (lnSplit.Any("class", false))
                    {
                        ClassText @class = new ClassText();
                        @class.Text = ReadBracketGroup(FileText, i, out int ResumeIndex);
                        @namespace.Types.Add(@class.ProcessClassText());
                        i = ResumeIndex;
                    }
                }
            }
            catch (Exception E)
            {
                throw E;
            }

            return output;
        }

        /// <summary>
        /// Parse the TextToRead, cuonting the Open and Close Brackets. End once they match up. <br/>
        /// This works because each open bracket requires a close bracket. <br/>
        /// Returned DeclarationText will include attributes and summary text if it was in the TextToRead, even if the StartIndex was after that attribute/comment
        /// </summary>
        /// <param name="TextToRead"></param>
        /// <param name="StartIndex"></param>
        /// <param name="EndIndex"></param>
        private static string[] ReadBracketGroup(in string[] TextToRead, in int StartIndex, out int EndIndex)
        {
            int OpenBracketCount = 0; int ClosedBracketCount = 0;
            bool HasPreText = false;
            List<string> ReadLines = new List<string>();
            List<string> Dec = new List<string>();
            for (int i = StartIndex; i < TextToRead.Length; i++)
            {
                string ln = TextToRead[i];
                ReadLines.Add(ln);
                if (!ln.Contains("//"))
                {
                    if (ln.Contains("{")) OpenBracketCount++;
                    if (ln.Contains("}")) ClosedBracketCount++;
                }
                if (OpenBracketCount > 0)
                {
                    if (!HasPreText)
                    {
                        int si = FindFirstIndex(TextToRead.ToArray(), "//", StartIndex);
                        Dec.AddRange(TextToRead.ToList().GetRange(si, StartIndex - si));
                        HasPreText = true;
                    }
                    Dec.Add(ln);
                }
                if (OpenBracketCount >= 1 && OpenBracketCount == ClosedBracketCount)
                    break;
            }
            EndIndex = StartIndex + ReadLines.Count - 1;
            return Dec.ToArray();
        }

        /// <inheritdoc cref="FindFirstIndex(string[], string, int, out int)"/>
        private static int FindFirstIndex(string[] FileText, string searchterm)
            => FindFirstIndex(FileText, searchterm, out _, -1);

        /// <inheritdoc cref="FindFirstIndex(string[], string, int, out int)"/>
        private static int FindFirstIndex(string[] FileText, string searchterm, int StartIndex = -1)
            => FindFirstIndex(FileText, searchterm, out _, StartIndex);

        /// <summary>
        /// Get the first index of a term when searching from the last towards the first of the array 
        /// </summary>
        /// <param name="FileText"></param>
        /// <param name="searchterm">Line must start with the search term</param>
        /// <param name="StartIndex">Index to start searching backwards from. If not supplied, use last item in FileText array</param>
        /// <param name="LastIndex">Result of FindLastIndex</param>
        /// <returns>
        /// Example: Search Term = // <br/>
        /// Line 0 <br/>
        /// Line 1 <br/>
        /// // Line2 <br/>
        /// // Line3 <br/>
        /// // Line4 <br/>
        /// [Attribute_Line5] <br/>
        /// [Attribute_Line6]<br/>
        /// <br/>
        /// This will return index of 2 ( index 0 and 1 do not start with the search term ) <br/>
        /// The LastIndex will be 4.
        /// </returns>
        private static int FindFirstIndex(string[] FileText, string searchterm, out int LastIndex, int StartIndex = -1)
        {
            LastIndex = FindLastIndex(FileText, searchterm, StartIndex);
            for (int i = LastIndex; i >= 0; i--)
            {
                if (!FileText[i].Trim().StartsWith(searchterm))
                    return i + 1;
            }
            return 0;
        }

        /// <summary>
        /// Searches the array from Last to First and returns the index of the first match of the search term <br/>
        /// </summary>
        /// <param name="searchterm">Line must start with the search term</param>
        /// <param name="StartIndex">Index to start searching backwards from. If not supplied, use last item in FileText array</param>
        /// <returns>
        /// Example: Search Term = // <br/>
        /// Line 0 <br/>
        /// Line 1 <br/>
        /// // Line2 <br/>
        /// // Line3 <br/>
        /// // Line4 <br/>
        /// [Attribute_Line5] <br/>
        /// [Attribute_Line6]<br/>
        /// <br/>
        /// The LastIndex will be 4.
        /// </returns>
        private static int FindLastIndex(string[] FileText, string searchterm, int StartIndex = -1)
        {
            int start = (StartIndex > 0 && StartIndex < FileText.Length - 1) ? StartIndex : FileText.Length - 1;
            for (int i = start; i >= 0; i--)
            {
                if (FileText[i].Trim().StartsWith(searchterm))
                    return i;
            }
            return 0;
        }

        private static CodeCommentStatementCollection GetComments(string[] FileText, int StartIndex = -1)
        {
            CodeCommentStatementCollection cll = new CodeCommentStatementCollection();
            for (int i = FindLastIndex(FileText, "//", StartIndex); i > 0 && FileText[i].Trim().StartsWith("//"); i--)
            {
                string ln = FileText[i].Trim();
                if (ln.StartsWith("///"))
                    cll.Add(new CodeCommentStatement(ln.Substring(3).Trim()));
                else if (ln.StartsWith("//"))
                    cll.Add(new CodeCommentStatement(ln.Substring(2).Trim()));
            }
            return cll;
        }

        private static CodeAttributeDeclarationCollection GetAttributes(string[] FileText, int StartIndex = -1)
        {
            CodeAttributeDeclarationCollection cll = new CodeAttributeDeclarationCollection();
            for (int i = FindLastIndex(FileText, "[", StartIndex); i > 0 && FileText[i].Trim().StartsWith("["); i--)
            {
                cll.Add(GetAttributeFromLine(FileText[i].Trim()));
            }
            return cll;
        }

        private static CodeAttributeDeclaration GetAttributeFromLine(string line)
        {
            //Ignore the wrapping brackets
            int i = line.IndexOf("(");
            string name = line.Trim().Substring(1, i - 1);
            string[] args = line.Substring(i + 1, line.IndexOf(')', i) - i - 1).Split(',');
            List<CodeAttributeArgument> arglist = new List<CodeAttributeArgument>();
            foreach (string a in args)
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

        protected class EnumText
        {
            public string[] Text { get; set; }

            public CodeTypeDeclaration ProcessEnumText()
            {
                CodeTypeDeclaration ret = new CodeTypeDeclaration
                {
                    IsClass = false,
                    IsEnum = true,
                    IsPartial = false,
                    Attributes = MemberAttributes.Public
                };

                foreach (string s in Text)
                {
                    string[] sS = s.Trim().Split(' ');
                    if (sS.Length > 1 && sS[1] == "enum")
                    {
                        ret.Name = sS[2];
                        ret.Comments.AddRange(GetComments(Text, Text.ToList().IndexOf(s)));
                        ret.CustomAttributes.AddRange(GetAttributes(Text, Text.ToList().IndexOf(s)));
                    }
                    else if (sS[0].Contains(",") && !sS[0].StartsWith("["))
                    {
                        ret.Members.Add(new CodeTypeMember { Name = sS[0].Replace(",", " ").Trim() });
                    }
                }
                return ret;
            }


        }

        protected class ClassText
        {
            public string[] Text { get; set; }

            public CodeTypeDeclaration ProcessClassText()
            {
                //Assume no nested classes since this should be output of XSD.exe, which puts all classes at root of namespace
                CodeTypeDeclaration ret = new CodeTypeDeclaration() { IsClass = false };
                bool classFound = false;
                for (int i = 0; i < this.Text.Length - 1; i++)
                {
                    string ln = this.Text[i];
                    string LL = ln.ToLower();
                    string[] lnSplit = ln.Trim().Split(' ');
                    if (String.IsNullOrWhiteSpace(ln) || ln.Length < 5)
                    { }
                    else if (ln.Contains("class") && ln.Contains("partial"))
                    {
                        int c = lnSplit.ToList().IndexOf("class");
                        ret = new CodeTypeDeclaration
                        {
                            Name = lnSplit[c + 1],
                            IsClass = true,
                            IsPartial = true,
                            Attributes = lnSplit[0] == "private" ? MemberAttributes.Private : MemberAttributes.Public
                        };
                        classFound = true;
                        //ret.Comments.AddRange(GetComments(@class.Text, ));
                        //CustomAttributes.AddRange(GetAttributes(fileText.ToArray()));
                    }
                    else if (LL.Contains("void") | LL.Contains("event") | LL.Contains("delegate"))
                    { }
                    else if (classFound && ln.Contains($"{ret.Name}("))
                    {
                        ///The contents here don't matter, only that it exists matters. So add empty constructors to the ret object to flag the <see cref="DiscoveredClass.HasConstructor"/> property.
                        if (lnSplit.Contains("static"))
                            ret.Members.Add(new CodeTypeConstructor());
                        else
                            ret.Members.Add(new CodeConstructor());
                    }
                    else if (lnSplit.Length >= 2 && ln.EndsWith(";"))
                    {
                        //This is a field
                        bool hasPubPriv = lnSplit[0] == "public" | lnSplit[0] == "private";
                        bool pub = hasPubPriv && lnSplit[0] == "public";
                        string fieldname = (hasPubPriv ? lnSplit[2].Replace(";", "") : lnSplit[1]).Replace(";", "");
                        string fieldType = hasPubPriv ? lnSplit[1] : lnSplit[0];
                        CodeMemberField field = new CodeMemberField(fieldType, fieldname);
                        field.Attributes = pub ? MemberAttributes.Public : MemberAttributes.Private;
                        field.Comments.AddRange(GetComments(this.Text, i));
                        field.CustomAttributes.AddRange(GetAttributes(this.Text, i));
                        ret.Members.Add(field);
                    }
                    else if (ln.Contains("public") && !(ln.Contains("void") | ln.Contains("event") && ln.Trim().Split(' ').Length >= 2))
                    {
                        ret.Members.Add(new PropertyText(ReadBracketGroup(this.Text, i, out int ResumeIndex)).GetCodeMemberProperty());
                        i = ResumeIndex;
                    }
                }
                return ret;
            }
        }

        protected class PropertyText
        {
            private PropertyText() { }
            public PropertyText(string[] text)
            {
                this.Text = text;
            }
            public string[] Text { get; }
            public bool HasSet => Text.AnyContains(new string[] { "Set;", "= value;" }, false);
            public bool HasGet => Text.AnyContains(new string[] { "Get;", "Get =>", "Get {" }, false);
            public string[] Attributes => Text.Where((string s) => s.Trim().StartsWith("[")).ToArray();

            public string Type => DecLineSplit[1];
            public string Name => DecLineSplit[2];
            private string DecLine => Text.First((string s) => s.ToLower().Trim().Split(' ')[0] == "public");
            private string[] DecLineSplit => DecLine.Trim().Split(' ');

            public CodeMemberProperty GetCodeMemberProperty()
            {
                CodeMemberProperty Prop = new CodeMemberProperty();
                Prop.Attributes = MemberAttributes.Public;
                Prop.Comments.AddRange(GetComments(Text));
                Prop.CustomAttributes.AddRange(GetAttributes(Text));
                Prop.HasSet = this.HasSet;
                Prop.HasGet = this.HasGet;
                string typeStr = this.Type;
                Prop.Type = new CodeTypeReference(typeStr);
                Prop.Name = this.Name;
                return Prop;
            }
        }

        #endregion </ ReadInClassFile >

    }



}
