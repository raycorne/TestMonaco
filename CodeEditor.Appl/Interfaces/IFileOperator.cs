using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeEditor.Appl.Interfaces
{
    public interface IFileOperator
    {
        public void SaveScriptFile(string data, string filePath);
        public string OpenScriptFile();
    }
}
