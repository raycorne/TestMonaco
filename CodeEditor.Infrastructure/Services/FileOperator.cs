using CodeEditor.Appl.Interfaces;

namespace CodeEditor.Infrastructure.Services
{
    public class FileOperator : IFileOperator
    {
        public string LoadScriptFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}
