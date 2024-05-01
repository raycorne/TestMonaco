namespace CodeEditor.Appl.Interfaces
{
    public interface IFileOperator
    {
        public void SaveScriptFile(string data, string filePath);
        public string LoadScriptFile(string filePath);
    }
}
