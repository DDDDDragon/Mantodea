using Mantodea.Content;
using Mantodea.Content.Components;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mantodea.RoomEditor
{
    public class EntityInfo
    {
        public static IEnumerable<EntityInfoUnit> GetDisplayProperties(Entity entity)
        {
            var type = entity.GetType();

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true);

                var attr = (from attribute in attributes
                            where attribute is DisplayInEditor
                            select attribute).FirstOrDefault(defaultValue: null);

                if (attr == null) continue;

                if (attr is DisplayInEditor dis)
                {
                    var unit = new EntityInfoUnit(dis.DisplayMode, property);
                    yield return unit;
                }
            }
        }
    }

    public struct EntityInfoUnit
    {
        public EntityInfoUnit(DisplayMode displayMode, PropertyInfo property)
        {
            DisplayMode = displayMode;
            PropertyInfo = property;
        }

        public DisplayMode DisplayMode;

        public PropertyInfo PropertyInfo;
    }
}
