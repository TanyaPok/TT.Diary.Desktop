using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public abstract class AbstractSectionItem
    {
        public string Name { get; private set; }

        public virtual void SetProperties(XmlNode node)
        {
            if (node.Attributes != null) Name = node.Attributes["name"].Value;
        }
    }
}