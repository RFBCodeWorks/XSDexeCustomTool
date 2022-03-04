using System.IO;

namespace XSDCustomToolVSIX.Interfaces
{
    /// <summary> HelperClass File Generator </summary>
    internal interface ICodeGenerator_HelperClass : ICodeGenerator
    {
        string OutputClassName { get; }
    }
}