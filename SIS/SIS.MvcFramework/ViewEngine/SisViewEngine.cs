using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using SIS.MvcFramework.Identity;

namespace SIS.MvcFramework.ViewEngine
{
    public class SisViewEngine : IViewEngine
    {
        private const string InvalidExpressionMessage = "Insufficient expression parentheses count: {0}";

        private IView CompileAndInstance(string code, Assembly modelAssembly)
        {
            var compilation = CSharpCompilation.Create("AppViewAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(Assembly.GetEntryAssembly().Location))
                .AddReferences(MetadataReference.CreateFromFile(modelAssembly.Location));

            AssemblyName[] netStandardAssemblyNames = Assembly.Load(new AssemblyName("netstandard")).GetReferencedAssemblies();

            foreach (AssemblyName assembly in netStandardAssemblyNames)
            {
                compilation = compilation.AddReferences(MetadataReference.CreateFromFile(Assembly.Load(assembly).Location));
            }

            compilation = compilation.AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(code));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                EmitResult compilationResult = compilation.Emit(memoryStream);

                if (!compilationResult.Success)
                {
                    foreach (var error in compilationResult.Diagnostics
                        .Where(x => x.Severity == DiagnosticSeverity.Error))
                    {
                        Console.WriteLine(error.GetMessage());
                    }

                    return null;
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                byte[] assemblyBytes = memoryStream.ToArray();

                Assembly assembly = Assembly.Load(assemblyBytes);

                Type type = assembly.GetType("AppViewCodeNamespace.AppViewCode");

                if (type == null)
                {
                    Console.WriteLine("AppViewCode not found.");

                    return null;
                }

                object instance = Activator.CreateInstance(type);

                return (IView) instance;
            }
        }

        private void HandleCSharpExpressions(string line, StringBuilder csharpCode)
        {
            string csharpStringToAppend = "html.AppendLine(@\"";
            string restOfLine = line;

            while (restOfLine.Contains("@"))
            {
                int atSignIndex = restOfLine.IndexOf("@");
                string plainText = restOfLine.Substring(0, atSignIndex)
                    .Replace("\"", "\"\"");
                Match match = Regex.Match(restOfLine.Substring(atSignIndex), @"^@\s*\(.+\s*\)");
                
                if (match.Success)
                {
                    int count = 1;
                    int openingBracketIndex = restOfLine.IndexOf("(", atSignIndex + 1);
                    int closingBracketIndex = openingBracketIndex;

                    while (count > 0 && closingBracketIndex < restOfLine.Length - 1)
                    {
                        closingBracketIndex++;

                        if (restOfLine[closingBracketIndex] == '(')
                        {
                            count++;
                        }
                        else if (restOfLine[closingBracketIndex] == ')')
                        {
                            count--;
                        }
                    }

                    string expression = restOfLine.Substring(openingBracketIndex + 1, closingBracketIndex - openingBracketIndex - 1);

                    if (count > 0)
                    {
                        throw new InvalidOperationException(string.Format(
                            InvalidExpressionMessage,
                            expression));
                    }

                    csharpStringToAppend += plainText + "\" + " + expression + " + @\"";

                    restOfLine = restOfLine.Substring(closingBracketIndex + 1);

                    continue;
                }

                Regex csharpCodeRegex = new Regex(@"[^\s<""]+");
                string csharpExpression = csharpCodeRegex.Match(restOfLine.Substring(atSignIndex + 1))?.Value;

                csharpStringToAppend += plainText + "\" + " + csharpExpression + " + @\"";

                if (restOfLine.Length <= atSignIndex + csharpExpression.Length + 1)
                {
                    restOfLine = string.Empty;
                }
                else
                {
                    restOfLine = restOfLine.Substring(atSignIndex + csharpExpression.Length + 1);
                }
            }

            csharpStringToAppend += $"{restOfLine.Replace("\"", "\"\"")}\");";
            csharpCode.AppendLine(csharpStringToAppend);
        }

        private string GetCSharpCode(string viewContent)
        {
            string[] lines = viewContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            StringBuilder csharpCode = new StringBuilder();

            string[] supportedOperators =
            {
                "if",
                "else",
                "for"
            };

            foreach (string line in lines)
            {
                if (line.TrimStart().StartsWith("{")
                    && line.TrimEnd().EndsWith("}"))
                {
                    int openingBracketIndex = line.IndexOf("{");
                    int closingBracketIndex = line.LastIndexOf("}");

                    string lineWithoutBrackets = line.Substring(openingBracketIndex + 1, closingBracketIndex - openingBracketIndex - 1);

                    csharpCode.AppendLine(lineWithoutBrackets);
                }
                else if (line.TrimStart().StartsWith("{")
                    || line.TrimStart().StartsWith("}"))
                {
                    csharpCode.AppendLine(line);
                }
                else if (supportedOperators.Any(x => line.TrimStart().StartsWith($"@{x}")))
                {
                    string lineWithoutAtSign = line.Substring(line.IndexOf("@") + 1);

                    csharpCode.AppendLine(lineWithoutAtSign);
                }
                else
                {
                    if (!line.Contains("@"))
                    {
                        string csharpLine = $"html.AppendLine(@\"{line.Replace("\"", "\"\"")}\");";

                        csharpCode.AppendLine(csharpLine);
                    }
                    else if (line.Contains("@Render"))
                    {
                        csharpCode.AppendLine($"html.AppendLine(@\"{line}\");");
                    }
                    else
                    {
                        HandleCSharpExpressions(line, csharpCode);
                    }
                }
            }
            
             return csharpCode.ToString();
        }

        public string GetHtml<T>(string viewContent, T model, Principal user = null)
        {
            Type modelType = model?.GetType() ?? typeof(T);
            string modelTypeName = modelType.Name;

            if (model == null)
            {
                model = (T) new object();
            }
            else if (typeof(IEnumerable).IsAssignableFrom(model.GetType()))
            {
                // simple array
                if (modelType.IsArray)
                {
                    string arrayType = modelTypeName.Substring(0, modelTypeName.IndexOf("["));

                    modelTypeName = $"{arrayType}[]";
                }
                else
                {
                    string name = modelTypeName.Substring(0, modelTypeName.IndexOf("`"));

                    modelTypeName = $"{name}<" + modelType.GetGenericArguments()[0] + ">";
                }
            }

            string csharpHtmlCode = GetCSharpCode(viewContent);
            string code = $@"
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using SIS.MvcFramework.Identity;
using SIS.MvcFramework.ViewEngine;

using {model.GetType().Namespace};

namespace AppViewCodeNamespace
{{
    public class AppViewCode : IView
    {{
        public string GetHtml(object model, Principal user)
        {{
            {modelTypeName} Model = ({modelTypeName}) model;
            Principal User = user;

            StringBuilder html = new StringBuilder();

            {csharpHtmlCode}

            return html.ToString().TrimEnd();
        }}
    }}
}}";

            IView view = CompileAndInstance(code, model.GetType().Assembly);
            
            string htmlResult = view?.GetHtml(model, user);

            return htmlResult;
        }        
    }
}