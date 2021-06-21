using System;
using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public enum ProductivityGradation
    {
        Excellent,
        Good,
        Normal,
        Bad,
        Horrible
    }

    public class Productivity : AbstractSectionItem
    {
        public ProductivityGradation Gradation { get; private set; }

        public double Begin { get; private set; }

        public double End { get; private set; }

        public override void SetProperties(XmlNode node)
        {
            base.SetProperties(node);
            Gradation = (ProductivityGradation) Enum.Parse(typeof(ProductivityGradation), Name);
            if (node.Attributes == null) return;
            Begin = double.Parse(node.Attributes["begin"].Value);
            End = double.Parse(node.Attributes["end"].Value);
        }
    }
}