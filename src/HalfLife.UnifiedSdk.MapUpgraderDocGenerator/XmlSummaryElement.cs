using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HalfLife.UnifiedSdk.MapUpgraderDocGenerator
{
    public sealed class XmlSummaryElement : IXmlSerializable
    {
        public string Contents { get; set; } = string.Empty;

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            // Read the contents as a single text string.
            // This is needed because some summary tags contain text that uses XML tags for styling
            // which XmlSerializer can't deal with.
            // Note that this removes whitespace between adjacent tags which we add back later.
            Contents = reader.ReadInnerXml().Trim();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
