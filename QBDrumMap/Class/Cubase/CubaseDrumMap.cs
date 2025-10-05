using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using QBDrumMap.Class.Extentions;

namespace QBDrumMap.Class.Cubase
{
    public class CubaseDrumMap
    {
        #region Properties

        public string Name { get; set; } = string.Empty;

        public QuantizeElement Quantize { get; set; } = new();

        public MapElement Map { get; set; } = new();

        public List<int> Order { get; set; } = [];

        public OutputDevicesElement OutputDevices { get; set; } = new();

        public int Flags { get; set; }

        #endregion

        #region ctor

        public CubaseDrumMap() { }

        #endregion

        #region Methods

        public static CubaseDrumMap Load(string filePath)
        {
            CubaseDrumMap drumMap;

            if (XDocument.Load(filePath) is not XDocument doc) throw new FileNotFoundException(filePath);

            if (doc.XPathSelectElement($"/{Tag.DrumMap}") is not XElement root) throw new ArgumentException(filePath);

            IEnumerable<XElement> GetItems(string key) => root.XPathSelectElements($"*[@{CubaseAttr.name}='{key}']/{Tag.item}");

            drumMap = new()
            {
                Name = root.XPathSelectElement($"*[@{CubaseAttr.name}='{nameof(Name)}']")?.Attribute($"{CubaseAttr.value}")?.Value ?? string.Empty
            };

            drumMap.Quantize.GetElement(GetItems(nameof(Quantize)));

            drumMap.Map.GetElement(GetItems(nameof(Map)));

            drumMap.Order = GetItems(nameof(Order)).Select(elm => int.Parse(elm.Attribute(CubaseAttr.value)?.Value ?? "0")).ToList();

            drumMap.OutputDevices.GetElement(GetItems(nameof(OutputDevices)));

            drumMap.Flags = int.Parse(root.XPathSelectElement($"*[@{CubaseAttr.name}='{nameof(Flags)}']")?.Attribute(CubaseAttr.value)?.Value ?? "0");

            return drumMap;
        }

        public void Save(string filePath)
        {
            XDocument doc = new XDocument(new XDeclaration(version: "1.0", encoding: "utf-8", null));

            XElement root = new XElement($"{Tag.DrumMap}");

            doc.Add(root);

            root.Add(new XElement(Tag.@string, [new XAttribute(CubaseAttr.name, $"{nameof(Name)}"), new XAttribute(CubaseAttr.value, Name), new XAttribute(CubaseAttr.wide, true.ToString().ToLower())]));

            root.Add(Quantize.ToElement());

            root.Add(Map.ToElement());

            XElement order = new XElement(Tag.list, [new XAttribute(CubaseAttr.name, nameof(Order)), new XAttribute(CubaseAttr.type, Tag.@int)]);
            Order.ForEach(item => order.Add(new XElement(Tag.item, new XAttribute(CubaseAttr.value, $"{item}"))));
            root.Add(order);

            root.Add(OutputDevices.ToElement());

            root.Add(new XElement(Tag.@int, [new XAttribute(CubaseAttr.name, $"{CubaseAttr.Flags}"), new XAttribute($"{CubaseAttr.value}", Flags)]));

            doc.Save(filePath.ToSafeFilePath());
        }

        public void Initialize()
        {
            Name = "Default";

            Quantize.Items.Add(new QuantizeItem());

            Enumerable.Range(0, 128).ToList().ForEach(x =>
            {
                Map.Items.Add(new MapItem(string.Empty, x));
                Order.Add(x);
            });

            OutputDevices.Items.Add(new OutputDevicesItem());
        }

        #endregion
    }
}
