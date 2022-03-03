﻿using System.CodeDom;
using System.CodeDom.Compiler;
using XSDCustomToolVSIX.BaseClasses;

namespace XSDCustomToolVSIX.Interfaces
{
    /// <summary>
    /// Interface for a ParsedFile object - Represents the file generated by XSD.exe. Contains methods to create classes based off the parsed file.
    /// </summary>
    internal interface IParsedFile
    {
        /// <inheritdoc cref="ParsedFile.CodeDomObjectProvider"/>
        CodeDomObjectProvider CodeDomObjectProvider { get; }
        /// <inheritdoc cref="ParsedFile.CodeGeneratorOptions"/>
        CodeGeneratorOptions CodeGeneratorOptions { get; }
        /// <inheritdoc cref="ParsedFile.DiscoveredClasses"/>
        DiscoveredClass[] DiscoveredClasses { get; }
        /// <inheritdoc cref="ParsedFile.DiscoveredEnums"/>
        DiscoveredEnum[] DiscoveredEnums { get; }
        /// <inheritdoc cref="ParsedFile.DiscoveredTypes"/>
        CodeTypeDeclarationCollection DiscoveredTypes { get; }
        /// <inheritdoc cref="ParsedFile.HelperClass"/>
        CodeGenerator_HelperClass HelperClass { get; }
        /// <inheritdoc cref="ParsedFile.IsGeneratingClass"/>
        bool IsGeneratingClass { get; }
        /// <inheritdoc cref="ParsedFile.IsGeneratingDataSet"/>
        bool IsGeneratingDataSet { get; }
        /// <inheritdoc cref="ParsedFile.LanguageProvider"/>
        CodeDomProvider LanguageProvider { get; }
        /// <inheritdoc cref="ParsedFile.OutputFileExtension"/>
        string OutputFileExtension { get; }
        /// <inheritdoc cref="ParsedFile.ParsedCode"/>
        CodeCompileUnit ParsedCode { get; }
        /// <inheritdoc cref="ParsedFile.Supplement"/>
        CodeGenerator_SupplementFile Supplement { get; }
        /// <inheritdoc cref="ParsedFile.TargetNameSpace"/>
        CodeNamespace TargetNameSpace { get; }
        /// <inheritdoc cref="ParsedFile.TopLevelClass"/>
        DiscoveredClass TopLevelClass { get; }
        /// <inheritdoc cref="ParsedFile.xSD_Instance"/>
        XSD_Instance xSD_Instance { get; }
        /// <inheritdoc cref="ParsedFile.FindEnumByName(string)"/>
        DiscoveredEnum FindEnumByName(string name);
    }
}