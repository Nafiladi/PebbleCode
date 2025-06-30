using System.Collections.Generic;
using System.Data;
using System.Text;

namespace PebbleCode
{
    public class PebbleCodeInterpreter
    {
        private readonly Dictionary<string, object> _variables = new Dictionary<string, object>();
        private readonly Dictionary<string, FunctionDefinition> _functions = new Dictionary<string, FunctionDefinition>();

        public event Func<string, Task<string>> OnRequestInput;
        public async Task<string> ExecuteAsync(string code, Dictionary<string, object> variables = null)
        {
            if (variables != null)
            {
                foreach (var v in variables)
                {
                    _variables[v.Key] = v.Value;
                }
            }

            var output = new StringBuilder();
            var lines = new List<string>(code.Split('
'));

            for (int i = 0; i < lines.Count; i++)
            {
                var trimmedLine = lines[i].Trim();

                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#") || trimmedLine.StartsWith("define "))
                {
                    continue;
                }

                if (trimmedLine.StartsWith("ask "))
                {
                    var parts = trimmedLine.Substring(4).Split(new[] { " into " }, 2, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        var prompt = parts[0].Trim().Trim('"');
                        var varName = parts[1].Trim();
                        if (OnRequestInput != null)
                        {
                            var userInput = await OnRequestInput(prompt);
                            _variables[varName] = userInput;
                        }
                    }
                }
                else if (trimmedLine.StartsWith("set "))
                {
                    var parts = trimmedLine.Substring(4).Split(new[] { " to " }, 2, System.StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        var varName = parts[0].Trim();
                        var expression = parts[1].Trim();
                        _variables[varName] = EvaluateExpression(expression);
                    }
                }
                else if (trimmedLine.StartsWith("say "))
                {
                    var valueStr = trimmedLine.Substring(4).Trim();
                    var evaluated = EvaluateExpression(valueStr);
                    output.AppendLine(evaluated.ToString());
                }
                else if (IsFunctionCall(trimmedLine, out var funcName, out var args))
                {
                    if (_functions.TryGetValue(funcName, out var funcDef))
                    {
                        var funcScope = new Dictionary<string, object>();
                        for (int j = 0; j < funcDef.Parameters.Count; j++)
                        {
                            funcScope[funcDef.Parameters[j]] = EvaluateExpression(args[j]);
                        }
                        var funcCode = string.Join("
", funcDef.Body);
                        var funcInterpreter = new PebbleCodeInterpreter { OnRequestInput = OnRequestInput, _functions = _functions };
                        output.Append(await funcInterpreter.ExecuteAsync(funcCode, funcScope));
                    }
                }
                else if (trimmedLine.StartsWith("if "))
                {
                    var ifBlock = GetBlock(lines, i);
                    output.Append(await ProcessIfBlockAsync(ifBlock));
                    i += ifBlock.Count - 1;
                }
                else if (trimmedLine.StartsWith("repeat "))
                {
                    var parts = trimmedLine.Split(' ');
                    if (parts.Length == 3 && parts[2] == "times" && int.TryParse(parts[1], out int repeatCount))
                    {
                        var loopBodyBlock = GetBlock(lines, i);
                        var loopBodyCode = string.Join("
", loopBodyBlock.GetRange(1, loopBodyBlock.Count - 2));

                        for (int k = 0; k < repeatCount; k++)
                        {
                            var loopInterpreter = new PebbleCodeInterpreter { OnRequestInput = OnRequestInput, _functions = _functions, _variables = new Dictionary<string, object>(_variables) };
                            output.Append(await loopInterpreter.ExecuteAsync(loopBodyCode));
                            foreach (var v in loopInterpreter._variables)
                            {
                                _variables[v.Key] = v.Value;
                            }
                        }
                        i += loopBodyBlock.Count - 1;
                    }
                }
            }

            return output.ToString();
        }

        private void PreProcessFunctions(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim();
                if (line.StartsWith("define "))
                {
                    var defLine = line.Substring(7);
                    var withIndex = defLine.IndexOf(" with ");
                    var funcName = withIndex > 0 ? defLine.Substring(0, withIndex).Trim() : defLine.Trim();
                    var paramString = withIndex > 0 ? defLine.Substring(withIndex + 6).Trim() : "";
                    var parameters = string.IsNullOrEmpty(paramString) ? new List<string>() : paramString.Split(new[] { " and " }, StringSplitOptions.None).Select(p => p.Trim()).ToList();

                    var body = new List<string>();
                    int endIndex = -1;
                    for (int j = i + 1; j < lines.Count; j++)
                    {
                        if (lines[j].Trim() == "end")
                        {
                            endIndex = j;
                            break;
                        }
                        body.Add(lines[j]);
                    }

                    if (endIndex != -1)
                    {
                        _functions[funcName] = new FunctionDefinition { Name = funcName, Parameters = parameters, Body = body };
                        i = endIndex;
                    }
                }
            }
        }

        private bool IsFunctionCall(string line, out string name, out List<string> args)
        {
            name = null;
            args = null;
            var parts = line.Split(new[] { ' ' }, 2);
            var funcName = parts[0];

            if (_functions.TryGetValue(funcName, out var funcDef))
            {
                name = funcName;
                var argString = parts.Length > 1 ? parts[1] : "";
                args = string.IsNullOrEmpty(argString) ? new List<string>() : argString.Split(new[] { " and " }, StringSplitOptions.None).Select(a => a.Trim()).ToList();
                return true;
            }
            return false;
        }

        private object EvaluateExpression(string expression)
        {
            // This is a simplified implementation of an expression evaluator.
            // It uses the Shunting-yard algorithm to handle operator precedence.
            // A more robust solution would use a proper parsing library.

            // Replace variables with their values
            foreach (var variable in _variables)
            {
                expression = expression.Replace(variable.Key, variable.Value.ToString());
            }

            try
            {
                var result = new DataTable().Compute(expression, null);
                return result;
            }
            catch
            {
                return $"Error evaluating expression: {expression}";
            }
        }

        private List<string> GetBlock(List<string> lines, int startIndex)
        {
            var block = new List<string>();
            var blockDepth = 0;
            for (int i = startIndex; i < lines.Count; i++)
            {
                var line = lines[i].Trim();
                block.Add(lines[i]);
                if (line.StartsWith("if ") || line.StartsWith("repeat "))
                {
                    blockDepth++;
                }
                else if (line == "end")
                {
                    blockDepth--;
                    if (blockDepth == 0)
                    {
                        break;
                    }
                }
            }
            return block;
        }

        private async Task<string> ProcessIfBlockAsync(List<string> ifBlock)
        {
            var output = new StringBuilder();
            var conditionMet = false;
            var blockDepth = 0;

            for (int i = 0; i < ifBlock.Count; i++)
            {
                var line = ifBlock[i].Trim();

                if (line.StartsWith("if ") || line.StartsWith("otherwise if "))
                {
                    if (blockDepth == 0)
                    {
                        if (conditionMet)
                        {
                            i += GetBlock(ifBlock, i).Count - 1;
                            continue;
                        }

                        var thenIndex = line.IndexOf(" then");
                        var conditionString = line.Substring(line.IndexOf(" ") + 1, thenIndex - line.IndexOf(" ") - 1).Trim();
                        var result = EvaluateExpression(conditionString);

                        if (result is bool boolResult && boolResult)
                        {
                            conditionMet = true;
                            var bodyBlock = GetBlock(ifBlock, i);
                            var bodyCode = string.Join("\n", bodyBlock.GetRange(1, bodyBlock.Count - 2));
                            var bodyInterpreter = new PebbleCodeInterpreter { OnRequestInput = OnRequestInput, _functions = _functions, _variables = new Dictionary<string, object>(_variables) };
                            output.Append(await bodyInterpreter.ExecuteAsync(bodyCode));
                            foreach(var v in bodyInterpreter._variables)
                            {
                                _variables[v.Key] = v.Value;
                            }
                        }
                    }
                }
                else if (line == "otherwise")
                {
                    if (blockDepth == 0)
                    {
                        if (!conditionMet)
                        {
                            var bodyBlock = GetBlock(ifBlock, i);
                            var bodyCode = string.Join("\n", bodyBlock.GetRange(1, bodyBlock.Count - 2));
                            var bodyInterpreter = new PebbleCodeInterpreter { OnRequestInput = OnRequestInput, _functions = _functions, _variables = new Dictionary<string, object>(_variables) };
                            output.Append(await bodyInterpreter.ExecuteAsync(bodyCode));
                            foreach (var v in bodyInterpreter._variables)
                            {
                                _variables[v.Key] = v.Value;
                            }
                        }
                        break;
                    }
                }

                if (line.StartsWith("if ") || line.StartsWith("repeat "))
                {
                    blockDepth++;
                }
                else if (line == "end")
                {
                    blockDepth--;
                }
            }
            return output.ToString();
        }
    }
}

class FunctionDefinition
{
    public string Name { get; set; }
    public List<string> Parameters { get; set; }
    public List<string> Body { get; set; }
}
