using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSDCustomToolVSIX;

namespace XSDCustomToolVSIX.Generate_Helpers.CSharp
{
    internal class ParsedFile_CSharp : ParsedFile
    {
        internal ParsedFile_CSharp(XSD_Instance instance) : base(instance)
        {

        }

        protected internal override string SummaryIndicator => "///";
        protected internal override string CommentIndicator => "//";

        protected override string ExtractClassName(string Line)
        {
            string[] arr = Line.Split(' ');
            int expectedlocation = arr.ToList().IndexOf("class");
            if (expectedlocation > 0) return arr[expectedlocation + 1]; //public partial class [classname]
            throw new Exception("Unexpected! - Output did not conform to expected format.");
        }

        protected override bool IsLine_ClassStart(string Line) => Line.Contains("partial class");
        protected override bool IsLine_ClassConstructor(string Line, string ClassName) => Line.Contains($"public {ClassName}()");

        protected override bool TryCreateProperty(string Line, out DiscoveredProperty newProp)
        {
            newProp = null;
            List<string> arr = Line.Trim().Split(' ').ToList();
            if (arr[0].ToLower() != "public") return false;
            newProp = new DiscoveredProperty
            {
                Type = arr[1],
                Name = arr[2],
                IsProperty = arr.Count() == 4 && arr[3] == "{"
            };
            return true;
        }

        protected override DiscoveredClass[] ParseLoop()
        {
            int? OpenBracketIndex = null; int OpenBracketCount = 0; int ClosedBracketCount = 0; int i = 0;
            string className = null; int ClassStartIndex = 0; bool classHasConstructor = false;
            List<string> classText = new List<string> { };
            List<string> DiscoverClassNames = new List<string>();
            List<DiscoveredClass_CSharp> DiscoveredClasses = new List<DiscoveredClass_CSharp> { };
            List<DiscoveredProperty> ClassProperties = new List<DiscoveredProperty>();

            while (i <= this.FileText.Length)
            {
                string ln = FileText[i];

                if (String.IsNullOrWhiteSpace(ln))
                {
                    //Do Nothing
                }
                else if (IsLine_ClassStart(ln))
                {
                    //Setup the ClassName and reset the bracket counters
                    className = ExtractClassName(ln);
                    ClassStartIndex = i;
                    classHasConstructor = false;
                    OpenBracketIndex = i;
                    OpenBracketCount = 0;
                    ClosedBracketCount = 0;
                }
                else if (className != null && IsLine_ClassConstructor(ln, className))
                {
                    classHasConstructor = true;
                }
                else if (className != null && IsLine_EventOrDelegate(ln))
                {
                    //Do Nothing
                }
                else if (className != null && TryCreateProperty(ln, out DiscoveredProperty prop))
                {
                    //Parse the last few lines for attributes
                    List<string> attrLines = new List<string>();
                    for (int a = classText.Count - 1; !String.IsNullOrWhiteSpace(classText[a]); a--)
                    {
                        if (classText[a].Contains("///")) break; //Break because a remark is found, and remarks are always before attributes
                        attrLines.Add(classText[a]); //These get added in reverse order, but that doesn't really matter.
                    }

                    //Evaluate the attributes and finalize the object
                    prop.Attributes = attrLines.ToArray();
                    prop.IsSerializable = !attrLines.Any((string s) => s.Contains("XmlIgnoreAttribute"));
                    prop.IsGeneratedClassType =  prop.Type.Contains(className) | DiscoverClassNames.Contains(prop.Type); //Indicates this is a class property of another auto-generated class.
                    prop.ReadOnly = true;
                    ClassProperties.Add(prop);
                }

                // Process the text for tracking the class start/end
                if (className != null) classText.Add(ln);
                OpenBracketCount += ln.Count(c => c == '{'); 
                ClosedBracketCount += ln.Count(c => c == '}');
                if (OpenBracketCount == ClosedBracketCount & className != null)
                {
                    // Finalize Class Properties
                    DiscoveredClass_CSharp @class = new DiscoveredClass_CSharp(this, className, ClassStartIndex, i, classHasConstructor);
                    @class.ClassProperties.AddRange(ClassProperties);
                    //
                    DiscoveredClasses.Add(@class);
                    DiscoverClassNames.Add(@class.ClassName);
                    //Reset any items for the loop
                    className = null;
                    ClassProperties = new List<DiscoveredProperty>();
                }
                else if (OpenBracketCount < ClosedBracketCount) // hit more than closed brackets than open brackets -> end of file.
                    return DiscoveredClasses.ToArray();
                i++;
            }
            //naturally hit end of file
            return DiscoveredClasses.ToArray();
        }
    }
}
