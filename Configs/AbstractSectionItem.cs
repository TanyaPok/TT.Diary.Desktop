using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public abstract class AbstractSectionItem
    {
        public string Name { get; protected set; }

        public virtual void SetProperties(XmlNode node)
        {
            Name = node.Attributes["name"].Value;
        }
    }
}
