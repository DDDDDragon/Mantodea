using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Mantodea.Mods
{
    public class ModFileReader
    {
        public static void Read(string modPath)
        {
            if (!File.Exists(modPath))
                return;

            FileStream fs = new(modPath, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            string modName = fs.Name.Split("\\")[^1].Replace(".smod", "");

            ModFile file = new()
            {
                Stream = fs,
                Name = modName
            };

            if (Encoding.UTF8.GetString(br.ReadBytes(4)) != "SSMN")
            {
                fs.Close();
                return;
            }

            file.ModVersion = br.ReadString();
            file.GameVersion = br.ReadString();

            var count = br.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                FileChunk chunk = new()
                {
                    Name = br.ReadString(),
                    Offset = br.ReadInt32(),
                    Length = br.ReadInt32()
                };

                file.Files.Add(chunk);
            }

            foreach (var chunk in file.Files)
                br.ReadBytes(chunk.Length);

            var assemblyLength = br.ReadInt32();

            file.Assembly = Assembly.Load(br.ReadBytes(assemblyLength));
        }
    }
}
