using System;
using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public class Theme : AbstractMenuItem
    {
        public const string TEA = "Tea";
        public const string COFFEE = "Coffee";
        public Uri Source { get; private set; }

        public override void SetProperties(XmlNode node)
        {
            base.SetProperties(node);
            Source = new Uri(node.Attributes["source"].Value, UriKind.Relative);
        }
    }
}
