namespace CodeEditor.Infrastructure.Services
{
    public class TokenForParce
    {
        public string Type { get; }
        public string Value { get; }

        public TokenForParce(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type}: {Value}";
        }
    }
}
