using System;
using System.IO;
using System.Linq;

namespace ConverterOfNspecToXunit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Digite o caminho da pasta com os arquivos");
            var root = Console.ReadLine();

            Console.WriteLine("======================= MOVENDO PASTA DOS ARQUIVOS ==========================");

            Console.WriteLine("Digite o novo caminho de pasta para os arquivos!");

            var dest = Console.ReadLine();

            var directories = Directory.GetDirectories(root);

            bool sub = false;
            if (directories.Count() >= 1) sub = true;

            DirectoryCopy.Copy(root, dest, sub);


            Console.WriteLine("======================= RENOMEANDO ARQUIVOS E CLASSES ==========================");
            Console.WriteLine("Escreva o caminho a ser ignorado no novo namespace");
            var ignored = Console.ReadLine();

            RenameFilesAndPaths(dest, ignored, sub);

            void RenameFilesAndPaths(string root, string ignored, bool sub)
            {
                DirectoryInfo dir = new DirectoryInfo(root);
                DirectoryInfo[] dirs = dir.GetDirectories();

                Console.WriteLine("\n" + dir);

                var files = Directory.GetFiles(dir.FullName, "*.cs");

                FileInfo fiInfo = null;

                Console.WriteLine("\nNomes antigos:");
                foreach (var fi in files)
                {
                    fiInfo = new FileInfo(fi);
                    Console.WriteLine(fiInfo.Name);
                }

                Console.WriteLine("\nNomes convertidos:");
                foreach (var fi in files)
                {
                    fiInfo = new FileInfo(fi);
                    var converted = ConvertSnakeToPascal.Convert(fiInfo.Name);

                    string cSharpCode = File.ReadAllText(fiInfo.FullName);

                    var newCSharpCode = new StreamWriter(fiInfo.FullName);

                    newCSharpCode.Write(cSharpCode.Replace(fiInfo.Name.Replace(".cs", ""), converted.Replace(".cs", "")).Replace("void before_all", converted.Replace(".cs", "")));
                    newCSharpCode.Close();

                    File.Move(fiInfo.FullName, fiInfo.FullName.Replace(fiInfo.Name, converted));

                    Console.WriteLine(converted);
                }

                Console.WriteLine("\n======================= RENOMEANDO NAMESPACE ===================");
                var renamedFiles = Directory.GetFiles(dir.FullName, "*.cs");
                foreach (var fi in renamedFiles)
                {
                    fiInfo = new FileInfo(fi);
                    string cSharpCode = File.ReadAllText(fiInfo.FullName);

                    
                    var findNameSpace = cSharpCode.Split("\r\n");

                    var t = 0;

                    for (int i = 0; i < findNameSpace.Length; i++)
                    {
                        if (findNameSpace[i].Contains("namespace")) break;
                        t++;
                    }

                    var oldNameSpace = findNameSpace[t];


                    var newCSharpCode = new StreamWriter(fiInfo.FullName);
                    var newNameSpace = ("namespace " + fiInfo.DirectoryName.Replace(ignored, "").Replace('\\', '.'));

                    if (cSharpCode.Contains("Tag"))
                    {
                        for (int i = 0; i < findNameSpace.Length; i++)
                        {
                            if (findNameSpace[i].Contains("Tag"))
                                cSharpCode = cSharpCode.Replace(findNameSpace[i], "");
                            if (!cSharpCode.Contains("Tag")) break;
                        }
                    }

                    var teste = cSharpCode.IndexOf("\r\n\r\n\r");
                    if (teste > 0)
                    {
                        cSharpCode = cSharpCode.Remove(teste, "\r\n\r\n\r".Length + 2);
                    }

                    cSharpCode = cSharpCode.Replace(oldNameSpace, newNameSpace);

                    var comented = cSharpCode.Split("\r\n");

                    var finish = comented.Count() - 2;

                    cSharpCode = cSharpCode.Replace(comented[0], "/*" + comented[0]);

                    newCSharpCode.Write(cSharpCode);
                    newCSharpCode.WriteLine("*/");
                    newCSharpCode.Close();
                }

                sub = false;
                if (dirs.Count() >= 1) sub = true;

                if (sub)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        RenameFilesAndPaths(subdir.FullName, ignored, sub);
                    }
                }

            }
        }
    }
}
