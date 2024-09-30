using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mantodea.RoomEditor
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DisplayInEditor : Attribute
    {
        public DisplayMode DisplayMode;

        public DisplayInEditor(DisplayMode mode)
        {
            DisplayMode = mode;
        }
    }

    public enum DisplayMode
    {
        EditableValue,
        SelectableValue
    }
}
