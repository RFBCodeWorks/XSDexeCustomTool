using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace XSDCustomToolVSIX.Interfaces
{
    /// <summary>
    /// Interface all Class Generators must implement
    /// </summary>
    interface ILinqClassGenerator
    {
        /// <summary> This is the FileInfo object that houses the path when writing this file to disk. </summary>
        //FileInfo FileOnDisk { get; }

        /// <summary>
        /// Run this method to create a supplement file for the parial classes XSD.exe generated. <br/>
        /// This method will end by calling <see cref="CodeGenerator_Base.Save(CodeCompileUnit);"/>
        /// </summary>
        //void Generate();
    }
}
