using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XSDCustomToolVSIX
{
    public class Enums
    {

        /// <summary> These are the languages that XSD.exe supports </summary>
        public enum SupportedLanguages
        {
            /// <summary> C-Sharp </summary>
            CSharp,
            /// <summary> Visual Basic </summary>
            VisualBasic,
            /// <summary> JavaScript </summary>
            JavaScript,
            /// <summary> Visual J-Sharp </summary>
            JSharp
        }

        /// <summary> XSD.exe can either generate CLASSES or DATASET outputs </summary>       
        public enum Generate
        {
            /// <summary> Generate a CLASS object from the supplied XSD file </summary>
            CLASSES,
            /// <summary> Generate a DATASET object from the supplied XSD file </summary>
            DATASET,
        }
        
        /// <summary>
        /// Converts the SupportedLanguages enum to the appropriate string to pass into XSD.exe
        /// </summary>
        public static XSDCustomTool_ParametersXSDexeOptionsLanguage LanguageEnumToEnum(SupportedLanguages Language)
        {
            switch (Language)
            {
                case SupportedLanguages.CSharp: return XSDCustomTool_ParametersXSDexeOptionsLanguage.CS;
                case SupportedLanguages.VisualBasic: return  XSDCustomTool_ParametersXSDexeOptionsLanguage.VB;
                case SupportedLanguages.JavaScript: return XSDCustomTool_ParametersXSDexeOptionsLanguage.JS ;
                case SupportedLanguages.JSharp: return  XSDCustomTool_ParametersXSDexeOptionsLanguage.VJS;
                default: return XSDCustomTool_ParametersXSDexeOptionsLanguage.CS;
            }
        }

        /// <summary>
        /// Converts the SupportedLanguages enum to the appropriate string to pass into XSD.exe
        /// </summary>
        public static SupportedLanguages LanguageEnumToEnum(XSDCustomTool_ParametersXSDexeOptionsLanguage Language)
        {
            switch (Language)
            {
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.CS:  return SupportedLanguages.CSharp;
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VB:  return SupportedLanguages.VisualBasic;
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.JS:  return SupportedLanguages.JavaScript;
                case XSDCustomTool_ParametersXSDexeOptionsLanguage.VJS: return SupportedLanguages.JSharp;
                default: return SupportedLanguages.CSharp;
            }
        }

        /// <summary>
        /// Converts the SupportedLanguages enum to the appropriate string to pass into XSD.exe
        /// </summary>
        public static string LanguageEnumToString(SupportedLanguages Language)
        {
            switch (Language)
            {
                case SupportedLanguages.CSharp: return "CS";
                case SupportedLanguages.VisualBasic: return "VB";
                case SupportedLanguages.JavaScript: return "JS";
                case SupportedLanguages.JSharp: return "VJS";
                default: return "CS";
            }
        }

        /// <summary>
        /// Converts the input string to the enum
        /// </summary>
        public static SupportedLanguages StringToLanguageEnum(string languageString)
        {
            switch (languageString)
            {
                case "CS": return SupportedLanguages.CSharp;
                case "VB": return SupportedLanguages.VisualBasic;
                case "JS": return SupportedLanguages.JavaScript;
                case "VJS": return SupportedLanguages.JSharp;
                default: throw new Exception($"Invalid Language String! -- Expected one of the following: \nCS (C#) \nJS (JavaScript)\nVJS (J#)\nVB (Visual Basic). \n String Received: {languageString}");
            }
        }
    }
}
