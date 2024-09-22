using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mantodea.Mods
{
    public class ModLoader
    {
        public List<ModFile> LoadedMod = new();

        public List<ModFile> EnabledMod = new();

        public List<Assembly> EnabledModAssemblies = new();
    }
}
