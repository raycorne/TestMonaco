using CodeEditor.Appl.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace CodeEditor.Infrastructure.Services
{
    public class FileOperator : IFileOperator
    {
        public string LoadScriptFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public void SaveScriptFile(string data, string filePath)
        {

            File.WriteAllText(filePath, data);
        }
    }
}
