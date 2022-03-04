using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XSDCustomToolVSIX.Interfaces
{
    interface IFileGenerator
    {
        /// <summary>
        /// Reference to the XSD Settings Object
        /// </summary>
        XSD_Instance XSD_Settings { get; }

        /// <summary>
        /// <inheritdoc cref="ICodeDomObjectProvider"/>
        /// </summary>
        ICodeDomObjectProvider CodeDomObjectProvider { get; }

        /// <summary>
        /// <inheritdoc cref="IParsedFile"/>
        /// </summary>
        IParsedFile ParsedFile { get; }

        /// <summary>
        /// <inheritdoc cref="ICodeGenerator_SupplementFile"/>
        /// </summary>
        ICodeGenerator_SupplementFile SupplementFileGenerator { get; }

        /// <summary>
        /// <inheritdoc cref="ICodeGenerator_HelperClass"/>
        /// </summary>
        ICodeGenerator_HelperClass HelperClassGenerator { get; }

        /// <summary>
        /// <inheritdoc cref="ILinqClassGenerator"/>
        /// </summary>
        ILinqClassGenerator LinqClassGenerator { get; }
    }
}
