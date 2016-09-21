using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WPFCore.Data
{
    /// <summary>
    ///     Implements a Dictionary which can be serialized to xml
    /// </summary>
    /// <remarks>
    ///     from the internet:
    ///     http://weblogs.asp.net/pwelter34/444961
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {
        private string xmlItemTagName = "item";
        private string xmlKeyTagName = "key";
        private string xmlValueTagName = "value";

        protected string XmlItemTagName
        {
            get { return this.xmlItemTagName; }
            set { this.xmlItemTagName = value; }
        }

        protected string XmlKeyTagName
        {
            get { return this.xmlKeyTagName; }
            set { this.xmlKeyTagName = value; }
        }

        protected string XmlValueTagName
        {
            get { return this.xmlValueTagName; }
            set { this.xmlValueTagName = value; }
        }

        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var keySerializer = new XmlSerializer(typeof (TKey));
            var valueSerializer = new XmlSerializer(typeof (TValue));

            var wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement(this.xmlItemTagName);

                reader.ReadStartElement(this.xmlKeyTagName);
                var key = (TKey) keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement(this.xmlValueTagName);
                var value = (TValue) valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            var keySerializer = new XmlSerializer(typeof (TKey));
            var valueSerializer = new XmlSerializer(typeof (TValue));

            foreach (var key in this.Keys)
            {
                writer.WriteStartElement(this.xmlItemTagName);

                writer.WriteStartElement(this.xmlKeyTagName);
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement(this.xmlValueTagName);
                var value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}