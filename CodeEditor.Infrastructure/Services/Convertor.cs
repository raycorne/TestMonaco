using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeEditor.Infrastructure.Services
{
    public class Convertor
    {
        public string ConvertToIR(string cCode)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            List<string> irLines = new List<string>();

            Regex varRegex = new Regex(@"int\s+(\w+)\s*=\s*(\d+)\s*;");
            Regex whileRegex = new Regex(@"while\s*\((.*?)\)\s*{");
            Regex ifRegex = new Regex(@"if\s*\((.*?)\)\s*{");
            Regex assignRegex = new Regex(@"(\w+)\s*=\s*(.*?);");

            string[] lines = cCode.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                Match varMatch = varRegex.Match(trimmedLine);
                if (varMatch.Success)
                {
                    string varName = varMatch.Groups[1].Value;
                    string varValue = varMatch.Groups[2].Value;
                    variables[varName] = varValue;
                    irLines.Add($"int {varName} = {varValue}");
                    continue;
                }

                Match whileMatch = whileRegex.Match(trimmedLine);
                if (whileMatch.Success)
                {
                    string condition = whileMatch.Groups[1].Value;
                    irLines.Add($"while ({condition}) {{");
                    continue;
                }

                Match ifMatch = ifRegex.Match(trimmedLine);
                if (ifMatch.Success)
                {
                    string condition = ifMatch.Groups[1].Value;
                    irLines.Add($"if ({condition}) {{");
                    continue;
                }

                Match assignMatch = assignRegex.Match(trimmedLine);
                if (assignMatch.Success)
                {
                    string varName = assignMatch.Groups[1].Value;
                    string expression = assignMatch.Groups[2].Value;
                    irLines.Add($"{varName} = {expression}");
                    continue;
                }

                if (trimmedLine == "}")
                {
                    irLines.Add("}");
                }
            }

            return string.Join("\n", irLines);
        }

        public string ConvertIRToAsm(string irCode)
        {
            Dictionary<string, int> variables = new Dictionary<string, int>();
            List<string> asmLines = new List<string>();
            Stack<string> labelStack = new Stack<string>();
            int tempVarCount = 0;

            string[] lines = irCode.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("int"))
                {
                    string[] parts = trimmedLine.Split(new[] { ' ', '=', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4 && int.TryParse(parts[3], out int value))
                    {
                        string varName = parts[1];
                        variables[varName] = value;
                        asmLines.Add($"mov {varName}, {value}");
                    }
                }
                else if (trimmedLine.StartsWith("while"))
                {
                    string condition = trimmedLine.Substring(6, trimmedLine.Length - 8);
                    string startLabel = $"L{tempVarCount++}";
                    string endLabel = $"{startLabel}_end";
                    labelStack.Push(endLabel);
                    asmLines.Add($"{startLabel}:");
                    string[] condParts = ParseCondition(condition);
                    asmLines.Add($"cmp {condParts[0]}, {condParts[2]}");
                    asmLines.Add($"{condParts[1]} {endLabel}");
                    asmLines.Add($"{startLabel}_body:");
                }
                else if (trimmedLine.StartsWith("if"))
                {
                    string condition = trimmedLine.Substring(3, trimmedLine.Length - 5);
                    string elseLabel = $"L{tempVarCount++}_else";
                    labelStack.Push(elseLabel);
                    string[] condParts = ParseCondition(condition);
                    asmLines.Add($"cmp {condParts[0]}, {condParts[2]}");
                    asmLines.Add($"{condParts[1]} {elseLabel}");
                    asmLines.Add($"{elseLabel}_body:");
                }
                else if (trimmedLine == "}")
                {
                    if (labelStack.Count > 0)
                    {
                        string endLabel = labelStack.Pop();
                        asmLines.Add($"jmp {endLabel}");
                        asmLines.Add($"{endLabel}:");
                    }
                }
                else if (trimmedLine.Contains("="))
                {
                    string[] parts = trimmedLine.Split(new[] { '=', '+', '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        asmLines.Add($"mov {parts[0].Trim()}, {parts[1].Trim()}");
                    }
                    else if (parts.Length == 3 && trimmedLine.Contains("+"))
                    {
                        asmLines.Add($"add {parts[0].Trim()}, {parts[2].Trim()}");
                    }
                    else if (parts.Length == 3 && trimmedLine.Contains("-"))
                    {
                        asmLines.Add($"sub {parts[0].Trim()}, {parts[2].Trim()}");
                    }
                }
            }

            return string.Join("\n", asmLines);
        }

        private string[] ParseCondition(string condition)
        {
            condition = condition.Replace(" ", "");
            if (condition.Contains("<"))
            {
                return new string[] { condition.Split('<')[0], "jge", condition.Split('<')[1] };
            }
            if (condition.Contains(">"))
            {
                return new string[] { condition.Split('>')[0], "jle", condition.Split('>')[1] };
            }
            if (condition.Contains("=="))
            {
                return new string[] { condition.Split(new[] { "==" }, StringSplitOptions.None)[0], "jne", condition.Split(new[] { "==" }, StringSplitOptions.None)[1] };
            }
            return new string[0];
        }
    }
}
