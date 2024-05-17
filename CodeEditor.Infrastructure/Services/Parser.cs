using CodeEditor.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

public class Parser
{
    private List<TokenForParce> _tokens;
    private int _position;
    private StringBuilder _assemblyCode;

    public Parser(List<TokenForParce> tokens)
    {
        _tokens = tokens;
        _position = 0;
        _assemblyCode = new StringBuilder();
    }

    private TokenForParce CurrentToken => _position < _tokens.Count ? _tokens[_position] : null;

    private void Expect(string tokenType)
    {
        if (CurrentToken.Type == tokenType)
        {
            _position++;
        }
        else
        {
            throw new Exception($"Expected {tokenType}, but found {CurrentToken.Type}");
        }
    }

    public string Parse()
    {
        while (CurrentToken != null)
        {
            ParseStatement();
        }
        return _assemblyCode.ToString();
    }

    private void ParseStatement()
    {
        if (CurrentToken.Type == "ID")
        {
            ParseAssignment();
        }
        else if (CurrentToken.Type == "WHILE")
        {
            ParseWhile();
        }
        else if (CurrentToken.Type == "IF")
        {
            ParseIf();
        }
        else
        {
            throw new Exception($"Unexpected token: {CurrentToken.Type}");
        }
    }

    private void ParseAssignment()
    {
        var id = CurrentToken.Value;
        Expect("ID");
        Expect("OPERATOR"); // Assume '='
        var expression = ParseExpression();
        Expect("SEMI");
        _assemblyCode.AppendLine($"{expression}");
        _assemblyCode.AppendLine($"mov {id}, eax");
    }

    private string ParseExpression()
    {
        var operand1 = CurrentToken.Value;
        Expect("NUMBER");

        if (CurrentToken != null && (CurrentToken.Type == "OPERATOR" && (CurrentToken.Value == "+" || CurrentToken.Value == "-" || CurrentToken.Value == "*" || CurrentToken.Value == "/")))
        {
            var op = CurrentToken.Value;
            Expect("OPERATOR");
            var operand2 = CurrentToken.Value;
            Expect("NUMBER");

            var code = new StringBuilder();
            code.AppendLine($"mov eax, {operand1}");

            switch (op)
            {
                case "+":
                    code.AppendLine($"add eax, {operand2}");
                    break;
                case "-":
                    code.AppendLine($"sub eax, {operand2}");
                    break;
                case "*":
                    code.AppendLine($"imul eax, {operand2}");
                    break;
                case "/":
                    code.AppendLine($"mov edx, 0");
                    code.AppendLine($"mov ebx, {operand2}");
                    code.AppendLine($"div ebx");
                    break;
            }

            return code.ToString();
        }

        return $"mov eax, {operand1}";
    }

    private void ParseWhile()
    {
        Expect("WHILE");
        Expect("LPAREN");
        var conditionVar = CurrentToken.Value;
        Expect("ID");
        var operatorType = CurrentToken.Value;
        Expect("OPERATOR");
        var value = CurrentToken.Value;
        Expect("NUMBER");
        Expect("RPAREN");
        Expect("LBRACE");
        var label = $"while_{conditionVar}";
        _assemblyCode.AppendLine($"{label}:");
        _assemblyCode.AppendLine($"cmp {conditionVar}, {value}");
        _assemblyCode.AppendLine($"{GetJumpInstruction(operatorType)} end_{label}");
        ParseStatement();
        Expect("RBRACE");
        _assemblyCode.AppendLine($"jmp {label}");
        _assemblyCode.AppendLine($"end_{label}:");
    }

    private void ParseIf()
    {
        Expect("IF");
        Expect("LPAREN");
        var conditionVar = CurrentToken.Value;
        Expect("ID");
        var operatorType = CurrentToken.Value;
        Expect("OPERATOR");
        var value = CurrentToken.Value;
        Expect("NUMBER");
        Expect("RPAREN");
        Expect("LBRACE");
        var label = $"if_{conditionVar}";
        _assemblyCode.AppendLine($"cmp {conditionVar}, {value}");
        _assemblyCode.AppendLine($"{GetJumpInstruction(operatorType)} end_{label}");
        ParseStatement();
        Expect("RBRACE");
        _assemblyCode.AppendLine($"end_{label}:");
    }

    private string GetJumpInstruction(string operatorType)
    {
        return operatorType switch
        {
            "==" => "jne",
            "!=" => "je",
            "<" => "jge",
            ">" => "jle",
            "<=" => "jg",
            ">=" => "jl",
            _ => throw new Exception($"Unknown operator: {operatorType}")
        };
    }
}