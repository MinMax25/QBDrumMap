using System.Xml.Linq;
using System.Xml.XPath;

namespace QBDrumMap.Class.Cubase
{
    public abstract class ListElementItem
    {
        #region Methods

        internal void GetElement(XElement element)
        {
            foreach (var prop in GetType().GetProperties())
            {
                if (element.XPathSelectElement($"*[@{CubaseAttr.name}='{prop.Name}']") is not XElement item) continue;

                object value = null!;

                if (prop.PropertyType == typeof(string))
                    value = item.Attribute(CubaseAttr.value)?.Value ?? string.Empty;
                else if (prop.PropertyType == typeof(int))
                    value = int.Parse(item.Attribute(CubaseAttr.value)?.Value ?? @"0");
                else if (prop.PropertyType == typeof(float))
                    value = float.Parse(item.Attribute(CubaseAttr.value)?.Value ?? @"0");
                else
                    continue;

                if (value == null) throw new ArgumentException();

                prop.SetValue(this, value);
            }
        }

        string getTypeName(Type type)
        {
            if (type == typeof(int)) return Tag.@int;
            if (type == typeof(string)) return Tag.@string;
            if (type == typeof(float)) return Tag.@float;
            throw new ArgumentException();
        }

        internal XElement ToElement()
        {
            XElement element = new XElement(Tag.item);

            foreach (var p in GetType().GetProperties())
                element.Add(
                    new XElement(getTypeName(p.PropertyType),
                    p.PropertyType == typeof(string) ?
                        new XAttribute[] {
                            new XAttribute(CubaseAttr.name, $"{p.Name}"),
                            new XAttribute(CubaseAttr.value, $"{p.GetValue(this)}"),
                            new XAttribute(CubaseAttr.wide, true.ToString().ToLower()),
                        }
                        :
                        [
                            new XAttribute(CubaseAttr.name, $"{p.Name}"),
                            new XAttribute(CubaseAttr.value, $"{p.GetValue(this)}")
                        ]
                    )
                );

            return element;
        }

        #endregion
    }
}
