using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace DynamicRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            DoSomethingSoft();
            Console.ReadKey();
        }
        
        private static void DoSomethingSoft()
        {
            var myClass = new MyClass();
            // from file
            string lines = File.ReadAllText(@"C:\source\prototypes\DynamicRunner\DynamicRunner\softclass.txt", Encoding.UTF8);
            // from string
            //string line = "public class FooClass { public string Execute() { return \"output!\";}}";
            
            using (Microsoft.CSharp.CSharpCodeProvider foo = 
                new Microsoft.CSharp.CSharpCodeProvider())
            {
                var res = foo.CompileAssemblyFromSource(
                    new System.CodeDom.Compiler.CompilerParameters() 
                    {  
                        GenerateExecutable = false,
                        GenerateInMemory = true,
                        ReferencedAssemblies =
                        {
                            Assembly.GetExecutingAssembly().Location,
                            "Backender.dll"
                        }
                    },
                    lines
                );

                var type = res.CompiledAssembly.GetType("FooClass");

                var obj = Activator.CreateInstance(type);

                var output = type.GetMethod("Execute").Invoke(obj, new object[] { "richard", myClass });
                
                Console.WriteLine(output);
            }
        }
    }
}