using CodeEditor.Infrastructure.Services;
using System.Text.RegularExpressions;

public class Lexer
{
    private string _input;
    private int _position;
    private Dictionary<string, string> _tokenPatterns;

    public Lexer(string input)
    {
        _input = input;
        _position = 0;
        _tokenPatterns = new Dictionary<string, string>
        {
            {"WHILE", @"while"},
            {"IF", @"if"},
            {"ID", @"[a-zA-Z_][a-zA-Z0-9_]*"},
            {"NUMBER", @"\d+"},
            {"OPERATOR", @"==|!=|<=|>=|<|>|=|\+|\-|\*|\/"},
            {"SEMI", @";"},
            {"LPAREN", @"\("},
            {"RPAREN", @"\)"},
            {"LBRACE", @"{"},
            {"RBRACE", @"}"},
            {"WHITESPACE", @"\s+"}
        };
    }

    public List<TokenForParce> Tokenize()
    {
        List<TokenForParce> tokens = new List<TokenForParce>();
        while (_position < _input.Length)
        {
            bool matchFound = false;
            foreach (var pattern in _tokenPatterns)
            {
                var regex = new Regex($"^{pattern.Value}");
                var match = regex.Match(_input.Substring(_position));
                if (match.Success)
                {
                    if (pattern.Key != "WHITESPACE")
                    {
                        tokens.Add(new TokenForParce(pattern.Key, match.Value));
                    }
                    _position += match.Length;
                    matchFound = true;
                    break;
                }
            }
            if (!matchFound)
            {
                throw new Exception("Unexpected character: " + _input[_position]);
            }
        }
        return tokens;
    }
}