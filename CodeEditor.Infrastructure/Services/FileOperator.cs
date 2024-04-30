using CodeEditor.Appl.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace CodeEditor.Infrastructure.Services
{
    public class FileOperator : IFileOperator
    {
        public string OpenScriptFile()
        {
            throw new NotImplementedException();
        }

        public void SaveScriptFile(string data, string filePath)
        {

            File.WriteAllText(filePath, data);
        }
    }
}
