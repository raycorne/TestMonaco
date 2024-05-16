using System.Text.RegularExpressions;

namespace CodeEditor.Infrastructure.Services
{
    public class TranslatorService
    {
        private List<string> assemblyCode;
        private Dictionary<string, string> variableMap = new Dictionary<string, string>(); // Для хранения регистров переменных

        public string TranslateCToAssembly(string cCode)
        {
            assemblyCode = new List<string> {
            "ORG 0000H",
            "MOV R0, #0    ; Инициализация счетчика (counter)"
        };

            // Инициализация переменных
            InitializeVariables(cCode);

            var tokens = Tokenize(cCode);
            ParseTokens(tokens);

            assemblyCode.Add("END_PROGRAM:");
            assemblyCode.Add("SJMP $");
            assemblyCode.Add("END");

            return string.Join("\n", assemblyCode);
        }

        private void InitializeVariables(string code)
        {
            // Находим все объявления переменных
            var variableDeclarations = Regex.Matches(code, @"int (\w+) = (\d+);");
            foreach (Match match in variableDeclarations)
            {
                var variableName = match.Groups[1].Value;
                var initialValue = match.Groups[2].Value;
                variableMap[variableName] = initialValue; // Сохраняем начальное значение
                assemblyCode.Add($"MOV R0, #{initialValue}    ; Инициализация {variableName}");
            }
        }

        private List<string> Tokenize(string code)
        {
            var tokens = new List<string>();
            var token = "";
            var depth = 0;

            foreach (var ch in code)
            {
                if (ch == '{' || ch == '}')
                {
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        tokens.Add(token.Trim());
                        token = "";
                    }
                    tokens.Add(ch.ToString());
                    continue;
                }
                else if (ch == ';' && depth == 0)
                {
                    token += ch;
                    tokens.Add(token.Trim());
                    token = "";
                    continue;
                }
                else if (ch == '(') depth++;
                else if (ch == ')') depth--;

                token += ch;
            }

            if (!string.IsNullOrWhiteSpace(token))
                tokens.Add(token.Trim());

            return tokens;
        }

        private void ParseTokens(List<string> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].StartsWith("while "))
                {
                    HandleWhile(tokens[i]);
                }
                else if (tokens[i].StartsWith("if "))
                {
                    HandleIf(tokens[i]);
                }
                else if (tokens[i].Contains("="))
                {
                    HandleAssignment(tokens[i]);
                }
            }
        }

        private void HandleWhile(string token)
        {
            // Извлекаем условие цикла
            var condition = ExtractCondition(token);
            var translatedCondition = TranslateConditionToAssembly(condition);
            assemblyCode.Add("START_WHILE:");
            assemblyCode.Add($"MOV A, R0");
            assemblyCode.Add($"CJNE A, #{translatedCondition}, WHILE_BODY");
            assemblyCode.Add("SJMP END_PROGRAM");
            assemblyCode.Add("WHILE_BODY:");
        }

        private void HandleIf(string token)
        {
            var condition = ExtractCondition(token);
            var translatedCondition = TranslateConditionToAssembly(condition);
            assemblyCode.Add($"MOV A, R0");
            assemblyCode.Add($"CJNE A, #{translatedCondition}, EVEN");
            assemblyCode.Add("MOV P1, #0x00");
            assemblyCode.Add("SJMP INCREMENT_COUNTER");
            assemblyCode.Add("EVEN:");
            assemblyCode.Add("MOV P1, #0xFF");
        }

        private void HandleAssignment(string token)
        {
            // Обрабатываем присваивание значений переменным
        }

        private string ExtractCondition(string token)
        {
            var match = Regex.Match(token, @"\(([^)]+)\)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return ""; // Возвращаем пустую строку, если условие не найдено
        }
        private string TranslateConditionToAssembly(string condition)
        {
            // Трансляция условий в ассемблерный код, учитывая переменные
            return condition;
        }
    }
}