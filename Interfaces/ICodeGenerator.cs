using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using XSDCustomToolVSIX.BaseClasses;

namespace XSDCustomToolVSIX.Interfaces
{
    /// <summary>
    /// Interface all Class Generators must implement
    /// </summary>
    interface ICodeGenerator
    {
        /// <summary> This is the FileInfo object that houses the path when writing this file to disk. </summary>
        FileInfo FileOnDisk { get; }

        /// <summary>
        /// Run this method to Generate the file. <br/>
        /// This method will end by calling <see cref="CodeGenerator_Base.Save(CodeCompileUnit);"/>
        /// </summary>
        void Generate();
    }
}
