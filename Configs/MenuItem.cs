using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public abstract class MenuItem
    {
        public string Name { get; set; }
        public string Tip { get; set; }
        public virtual void SetProperties(XmlNode node)
        {
            Name = node.Attributes["name"].Value;
            Tip = node.Attributes["tip"].Value;
        }
    }
}
