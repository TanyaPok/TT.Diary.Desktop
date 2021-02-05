using System;
using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public class Theme : AbstractSectionItem
    {
        public const string TEA = "Tea";

        public const string COFFEE = "Coffee";

        public string Tip { get; private set; }

        public Uri Source { get; private set; }

        public override void SetProperties(XmlNode node)
        {
            base.SetProperties(node);
            Tip = node.Attributes["tip"].Value;
            Source = new Uri(node.Attributes["source"].Value, UriKind.Relative);
        }
    }
}
