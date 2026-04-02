using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace QBDrumMap.Class.Cubase
{
    public abstract class ListElementItem
    {
        #region Methods

        #region General

        internal void GetElement(XElement element)
        {
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (element.XPathSelectElement($"*[@{CubaseAttr.name}='{prop.Name}']") is not XElement item)
                {
                    continue;
                }

                object value;

                if (prop.PropertyType == typeof(string))
                {
                    value = item.Attribute(CubaseAttr.value)?.Value ?? string.Empty;
                }
                else if (prop.PropertyType == typeof(int))
                {
                    value = int.Parse(item.Attribute(CubaseAttr.value)?.Value ?? "0");
                }
                else if (prop.PropertyType == typeof(float))
                {
                    value = float.Parse(item.Attribute(CubaseAttr.value)?.Value ?? "0");
                }
                else
                {
                    continue;
                }

                if (value == null)
                {
                    throw new ArgumentException();
                }

                prop.SetValue(this, value);
            }
        }

        internal XElement ToElement()
        {
            XElement element = new XElement(Tag.item);

            foreach (PropertyInfo p in GetType().GetProperties())
            {
                XElement child = new XElement(GetTypeName(p.PropertyType));

                child.Add(new XAttribute(CubaseAttr.name, $"{p.Name}"));
                child.Add(new XAttribute(CubaseAttr.value, $"{p.GetValue(this)}"));

                if (p.PropertyType == typeof(string))
                {
                    child.Add(new XAttribute(CubaseAttr.wide, true.ToString().ToLower()));
                }

                element.Add(child);
            }

            return element;
        }

        private string GetTypeName(Type type)
        {
            if (type == typeof(int))
            {
                return Tag.@int;
            }

            if (type == typeof(string))
            {
                return Tag.@string;
            }

            if (type == typeof(float))
            {
                return Tag.@float;
            }

            throw new ArgumentException();
        }

        #endregion

        #endregion
    }
}