using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;

namespace EnumMatchingAlgorithm
{
    class EnumMatch
    {
        static void Main(string[] args)
        {
            string csText = File.ReadAllText("C:\\Users\\PC_4232\\source\\repos\\wpf_1\\wpf_1\\Enums.cs");
            string newCsText = "";

            Dictionary<string, List<string>> keyValuePairs = new Dictionary<string, List<string>>();
            keyValuePairs = ProcessEnumCode(csText);

            Dictionary<string, string> matchedEnum = new Dictionary<string, string>();

            matchedEnum = dictWrite(keyValuePairs);

            for (int i = 0; i < matchedEnum.Count; i++)
            {

                Console.WriteLine(matchedEnum.Values.ElementAt(i)  + "  " + matchedEnum.Keys.ElementAt(i));
                newCsText = csText.Replace(matchedEnum.Values.ElementAt(i).ToString(), matchedEnum.Keys.ElementAt(i).ToString());
            }    

            Console.WriteLine("\n-------------------- Eşleştirilmiş Enumlar -----------------\n");

            File.WriteAllText("C:\\Users\\PC_4232\\Desktop\\Mehmet\\newEnums.cs", newCsText);

            Console.ReadKey();

        }

        public static Dictionary<string, string> dictWrite(Dictionary<string, List<string>> dict)
        {
            Dictionary<string, string> matchEnum = new Dictionary<string, string>();
            string matchValue = string.Empty;
            for (int i = 0; i < dict.Count; i++)
            {
                Console.WriteLine(dict.Keys.ElementAt(i));
                for (int j = 0; j<dict.Values.ElementAt(i).Count; j++)
                {                   
                    Console.WriteLine("\t" + dict.Values.ElementAt(i)[j]);
                    Console.CursorTop--;
                    Console.CursorLeft = ("\t" + dict.Values.ElementAt(i)[j]).Length + 10;                    
                    matchValue = Console.ReadLine();

                    if (matchValue != "")
                    {
                        matchEnum.Add(dict.Values.ElementAt(i)[j], matchValue);
                    }                   
                }
            }

            return matchEnum;
        }

        public static Dictionary<string, List<string>> ProcessEnumCode(string enumCode)
        {
            Dictionary<string, List<string>> enums = new Dictionary<string, List<string>>();
            

            var syntaxTree = CSharpSyntaxTree.ParseText(enumCode);
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location)
            };

            var compilation = CSharpCompilation.Create("DynamicEnumCompilation")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTree);

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    Console.WriteLine("Derleme hatası:\n" + string.Join("\n", result.Diagnostics));
                    Environment.Exit(0);
                }

                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());

                //List<string> enumOutput = new List<string>();

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsEnum)
                    {
                        List<string> enumValue = new List<string>();
                        foreach (var value in Enum.GetValues(type))
                        {
                            
                            enumValue.Add(value.ToString());
                        }

                        enums.Add(type.Name, enumValue);                        
                    }
                }

                return enums;
            }
        }
    }
}