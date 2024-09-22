using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mantodea.Content.Components
{
    public class ObjectPack : Component
    {
        public ObjectPack() 
        {
            Objects = new();
        }

        public Dictionary<string, object> Objects;

        public override int Height => 0;

        public override int Width => 0;

        public object this[string id]
        {
            get => Objects.ContainsKey(id) ? Objects[id] : null;
            set {
                if(Objects.ContainsKey(id)) Objects[id] = value;
                else Objects.Add(id, value);
            }
        }
    }
}
