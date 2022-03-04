//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Schema;
//using System.CodeDom;
//using System.CodeDom.Compiler;

//namespace XSDCustomToolVSIX.Base_Classes
//{
//    /// <summary>
//    /// Read the XSD file Directory
//    /// </summary>
//    internal class XSD_Reader
//    {
//        #region < Construction >

//        private XSD_Reader() { }

//        private XSD_Reader(XmlSchema schemaFile)
//        {
//            SchemaFile = schemaFile;
//        }

//        /// <summary>
//        /// Validate the file, then if possible return a reader that will generate the CodeDomObject from the xsd file
//        /// </summary>
//        /// <param name="path"></param>
//        /// <returns></returns>
//        public static XSD_Reader Factory(string path)
//        {
//            if (!File.Exists(path)) throw new FileNotFoundException("Unable to convert file. ( File Missing )", fileName: path);

//            List<ValidationEventArgs> Errs = new List<ValidationEventArgs>();
//            var ErrHandler = new ValidationEventHandler((o, e) =>
//           {
//               throw e.Exception;
//           });

//            using (var txt = File.OpenRead(path))
//            {
//                var Schema = XmlSchema.Read(txt, ErrHandler);
//                if (Errs.Count == 0)
//                {
//                    return new XSD_Reader(Schema);
//                }
//            }
//            return null;
//        }

//        #endregion

//        #region < Properties >

//        XmlSchema SchemaFile { get; }

//        string ClassID => SchemaFile.Id;

//        #endregion

//        #region < Methods >

//        public CodeCompileUnit GetCodeCompileUnit(CodeNamespace @namespace)
//        {
//            var unit = new CodeCompileUnit();
//            unit.Namespaces.Add(@namespace);
//            SchemaFile.Items.
//        }

//        private CodeTypeDeclaration GetElementDeclaration(XmlSchemaElement el)
//        {
//            var dec = new CodeTypeDeclaration(el.Name);
//            if(el.ElementSchemaType.TypeCode = )
//            {
//                case 
//            }

//        }

//        #endregion

//    }
//}
