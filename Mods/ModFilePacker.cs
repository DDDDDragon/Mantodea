using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mantodea.Mods
{
    public class ModFilePacker
    {
        public static bool Pack(string modPath)
        {
            if (!Directory.Exists(modPath)) return false;

            DirectoryInfo dir = new(modPath);

            var modName = dir.Name;

            #region Compile
            bool successful;
            byte[] code;
            try
            {
                successful = CompileMod(
                    modName,
                    modPath,
                    Main.ModPath,
                    new Type[1] { typeof(Mod) },
                    out code,
                    out _
                );
            }
            catch (Exception ex)
            {
                throw;
            }

            if (!successful) return false;
            #endregion

            var assembly = Assembly.Load(code);

            var modClass = assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(Mod)), null);

            if (modClass == null)
                return false;

            var instance = Activator.CreateInstance(modClass) as Mod;

            FileInfo[] textures = dir.GetFiles("*.png", SearchOption.AllDirectories);

            int offset = 0;

            if (!Directory.Exists(Main.ModPath))
                Directory.CreateDirectory(Main.ModPath);


            FileStream fs = new(Path.Combine(Main.ModPath, modName + ".smod"), FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(Encoding.UTF8.GetBytes("SSMN"));
            bw.Write(instance.ModVersion);
            bw.Write(instance.GameVersion);

            bw.Write(textures.Length);
            foreach(var tex in textures)
            {
                string name = modName + tex.FullName.Replace(modPath, "");
                bw.Write(name);
                bw.Write(offset);
                var len = CalculateBytesLength(tex);
                bw.Write(len);
                offset += len;
            }

            foreach(var tex in textures)
            {
                var stream = new FileStream(tex.FullName, FileMode.Open);
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                bw.Write(bytes, 0, bytes.Length);
                stream.Close();
            }

            bw.Write(code.Length);
            bw.Write(code);
            bw.Close();

            return true;
        }

        public static int CalculateBytesLength(FileInfo f)
        {
            var stream = new FileStream(f.FullName, FileMode.Open);
            var len = (int)stream.Length;
            stream.Close();
            return len;
        }

        public static List<MetadataReference> GetSystemMetadataReferences()
        {
            string trustedAssemblies = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string)!;
            string[] trustedList = trustedAssemblies.Split(Path.PathSeparator);
            List<string> required = new()
            {
                "System.Runtime.dll",
                "System.Linq.dll",
                "System.ObjectModel.dll",
                "System.Collections.dll",
                "System.Private.CoreLib.dll",
                "System.Text.RegularExpressions.dll"
            };
            IEnumerable<string> filteredPath = trustedList.Where(p => required.Exists(r => p.Contains(r)));
            return filteredPath.Select(x => MetadataReference.CreateFromFile(x) as MetadataReference).ToList();
        }

        public static Diagnostic[] RoslynCompile(string name, IEnumerable<string> files, IEnumerable<string> preprocessorSymbols, string dllfolder, Type[] neededType, out byte[] code, out byte[] pdb)
        {
            NameSyntax[] qualifiedNames = {
            SyntaxFactory.ParseName("System"),
            SyntaxFactory.ParseName("System.Linq"),
            SyntaxFactory.ParseName("System.Collections"),
            SyntaxFactory.ParseName("System.Collections.Generic"),
            SyntaxFactory.ParseName("System.Runtime")
        };
            // creating compilation options
            CSharpCompilationOptions options = new(
                OutputKind.DynamicallyLinkedLibrary,
                checkOverflow: true,
                optimizationLevel: OptimizationLevel.Release,
                allowUnsafe: false
            );

            // creating parse options
            CSharpParseOptions parseOptions = new(LanguageVersion.Preview, preprocessorSymbols: preprocessorSymbols);

            // creating emit options
            EmitOptions emitOptions = new(debugInformationFormat: DebugInformationFormat.PortablePdb);

            // convert string of dll into MetadataReference
            IEnumerable<MetadataReference> defaultReferences = Directory.GetFiles(dllfolder, "*.dll")
                .Select(x => MetadataReference.CreateFromFile(x));

            try
            {
                defaultReferences = defaultReferences
                    .Concat(neededType.Select(x => MetadataReference.CreateFromFile(x.Assembly.Location)))
                    .Concat(GetSystemMetadataReferences());
            }
            catch (Exception ex)
            {
                throw;
            }

            IEnumerable<SyntaxTree> src = files.Select(f => SyntaxFactory.ParseSyntaxTree(File.ReadAllText(f), parseOptions, f, Encoding.UTF8));
            // update tree before compilation to add needed using
            src = src.Select(tree => (tree.GetRoot() as CompilationUnitSyntax)!
                .AddUsings(qualifiedNames.Select(qualifiedNameSpace => SyntaxFactory.UsingDirective(qualifiedNameSpace)).ToArray()).NormalizeWhitespace().SyntaxTree);

            CSharpCompilation comp = CSharpCompilation.Create(name, src, defaultReferences, options);

            using MemoryStream peStream = new();
            using MemoryStream pdbStream = new();

            EmitResult results = comp.Emit(peStream, pdbStream, options: emitOptions);

            code = peStream.ToArray();
            pdb = pdbStream.ToArray();

            return results.Diagnostics.ToArray();
        }

        public static bool CompileMod(string name, string path, string dllfolder, Type[] neededType, out byte[] code, out byte[] pdb)
        {
            IEnumerable<string> files = Directory
                .GetFiles(path, "*.cs", SearchOption.AllDirectories)
                .Where(file => !IgnoreCompletely(path, file));

            Diagnostic[] result = Array.Empty<Diagnostic>();
            try
            {
                result = RoslynCompile(name, files, new[] { "FNA" }, dllfolder, neededType, out code, out pdb);
            }
            catch
            {
                throw;
            }

            if (Array.Exists(result, e => e.Severity == DiagnosticSeverity.Error))
            {
                return false;
            }
            return true;
        }

        public static bool IgnoreCompletely(string root, string file)
        {
            string path_from_root = file.Replace(root + Path.DirectorySeparatorChar.ToString(), "");
            string[] dirs = path_from_root.Split(Path.DirectorySeparatorChar.ToString());

            string dir = "";
            if (dirs.Length > 1) // not only a file, but also folders
            {
                dir = dirs[0]; // topmost directory
            }
            return dir.StartsWith('.') || dir == "bin" || dir == "obj";
        }
    }
}
