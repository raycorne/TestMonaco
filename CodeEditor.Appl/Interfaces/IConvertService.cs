using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeEditor.Appl.Interfaces
{
    public interface IConvertService
    {
        public string ConvertToIR(string cCode);
        public string ConvertIRToAsm(string irCode);
    }
}
