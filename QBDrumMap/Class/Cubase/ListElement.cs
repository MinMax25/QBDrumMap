using System.Xml.Linq;

namespace QBDrumMap.Class.Cubase
{
    public abstract class ListElement<T>
        where T : ListElementItem
    {
        #region Properties

        public List<T> Items { get; set; }

        #endregion

        #region ctor

        public ListElement(IEnumerable<T> items)
        {
            Items = items.ToList();
        }

        #endregion

        #region Methods

        internal void GetElement(IEnumerable<XElement> elements)
        {
            elements.ToList().ForEach(element =>
            {
                if (Activator.CreateInstance(typeof(T)) is not T instance) throw new ArgumentException();
                instance.GetElement(element);
                Items.Add(instance);
            });
        }

        internal XElement ToElement()
        {
            XElement element =
                new(
                    "list",
                    [
                        new XAttribute(CubaseAttr.name, GetType().Name.Replace($"{nameof(XElement.Element)}", string.Empty)),
                        new XAttribute(CubaseAttr.type, "list"),
                    ]
                );

            Items.ForEach(item => element.Add(item.ToElement()));

            return element;
        }

        #endregion
    }
}
