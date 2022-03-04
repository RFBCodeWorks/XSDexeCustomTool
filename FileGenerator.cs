using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSDCustomToolVSIX.Interfaces;

namespace XSDCustomToolVSIX
{
    internal class FileGenerator : IFileGenerator
    {
        private FileGenerator() { }

        internal FileGenerator(XSD_Instance xSD) 
        {
            var Lang = xSD.XSDexeOptions.Language;
            CodeDomObjectProvider = BaseClasses.CodeDomObjectProvider.Factory(Lang);
            ParsedFile = BaseClasses.ParsedFile.ParsedFileFactory(this);
            HelperClassGenerator = BaseClasses.CodeGenerator_HelperClass.Factory(this.ParsedFile);
            SupplementFileGenerator = BaseClasses.CodeGenerator_SupplementFile.Factory(this.ParsedFile);
            LinqClassGenerator = BaseClasses.LinqClassGenerator_Base.Factory(this.ParsedFile);
        }

        public XSD_Instance XSD_Settings { get; private set; }

        public ICodeDomObjectProvider CodeDomObjectProvider { get; private set; }

        public IParsedFile ParsedFile { get; private set; }

        public ICodeGenerator_SupplementFile SupplementFileGenerator { get; private set; }

        public ICodeGenerator_HelperClass HelperClassGenerator { get; private set; }

        public ILinqClassGenerator LinqClassGenerator { get; private set; }
    }
}
