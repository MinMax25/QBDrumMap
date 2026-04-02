using System.Xml.Linq;

namespace QBDrumMap.Class.Cubase
{
    public abstract class ListElement<T> where T : ListElementItem
    {
        #region Properties

        // リストアイテムのコレクション
        public List<T> Items { get; set; } = new();

        #endregion

        #region ctor

        public ListElement(IEnumerable<T> items)
        {
            Items = items.ToList();
        }

        #endregion

        #region Methods

        #region General

        internal void GetElement(IEnumerable<XElement> elements)
        {
            elements.ToList().ForEach(element =>
            {
                if (Activator.CreateInstance(typeof(T)) is not T instance)
                {
                    throw new ArgumentException();
                }

                instance.GetElement(element);
                Items.Add(instance);
            });
        }

        internal XElement ToElement()
        {
            XElement element = new XElement(
                "list",
                new XAttribute(CubaseAttr.name, GetType().Name.Replace("Element", string.Empty)),
                new XAttribute(CubaseAttr.type, "list")
            );

            Items.ForEach(item =>
            {
                element.Add(item.ToElement());
            });

            return element;
        }

        #endregion

        #endregion
    }
}