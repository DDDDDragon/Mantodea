using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mantodea.Utils
{
    public class ValueConverter
    {
        public static Dictionary<Type, Convert> Converter { get; set; }

        public delegate object Convert(string text);

        static ValueConverter()
        {
            Converter = new Dictionary<Type, Convert>
            {
                { typeof(string), (str) => str },
                { 
                    typeof(int), (str) => 
                    {
                        int val = 0;
                        try
                        {
                            val = int.Parse(str);
                        }
                        catch
                        {

                        }
                        return val;
                    } 
                },
                { typeof(bool), (str) => bool.Parse(str) }
            };
        }

        public static object ConvertValue(string text, PropertyInfo property)
        {
            Converter.TryGetValue(property.PropertyType, out var value);

            return value?.Invoke(text);
        }
    }
}
