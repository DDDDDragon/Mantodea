using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Mantodea.Mods
{
    public class Mod
    {
        public Mod() { }

        public virtual string Name { get => GetType().Name; }

        public virtual string Author { get => "N/A"; }

        public virtual string Description { get => "N/A"; }

        public virtual string ModVersion { get => "v0.0.0.0"; }

        public virtual string GameVersion { get => "v0.0.0.0"; }

        public virtual void Load()
        {

        }

        public virtual void Unload()
        {

        }
    }

    public class ModFile
    {
        public string Name { get; set; }

        public string ModVersion { get; set; }

        public string GameVersion { get; set; }

        public List<FileChunk> Files = new();

        public Assembly Assembly;

        public int FileOffset;

        public FileStream Stream;

        public string Path;
    }

    public class FileChunk
    {
        public string Name { get; set; }

        public int Offset { get; set; }

        public int Length { get; set; }
    }
}
