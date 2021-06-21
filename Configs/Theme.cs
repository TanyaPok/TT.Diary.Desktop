using System;
using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public class Theme : AbstractSectionItem
    {
        public const string Tea = "Tea";

        public const string Coffee = "Coffee";

        public string Tip { get; private set; }

        public Uri Source { get; private set; }

        public override void SetProperties(XmlNode node)
        {
            base.SetProperties(node);
            if (node.Attributes == null) return;
            Tip = node.Attributes["tip"].Value;
            Source = new Uri(node.Attributes["source"].Value, UriKind.Relative);
        }
    }
}