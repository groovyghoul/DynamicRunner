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
            string lines = File.ReadAllText(@"softclass.txt", Encoding.UTF8);
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
                            "System.dll",
                            Assembly.GetExecutingAssembly().Location,
                            "Backender.dll"
                        }
                    },
                    lines
                );

                var type = res.CompiledAssembly.GetType("FooClass");

                var obj = Activator.CreateInstance(type);

                var output = type.GetMethod("Execute").Invoke(obj, new object[] { "richard", myClass });

                // attempt to run a method as an action
                var mi = type.GetMethod("FooAction");
                var action = DelegateBuilder.BuildDelegate<Action<object, string>>(mi);
                action(obj, "This is a message from a soft action");
                
                Console.WriteLine(output);
            }
        }
    }
}