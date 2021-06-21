using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public class ScheduleType : AbstractSectionItem
    {
        public const string Day = "Day";

        public const string Week = "Week";

        public const string Month = "Month";

        public const string Year = "Year";

        public string Tip { get; private set; }

        public string ImgUrl { get; private set; }

        public override void SetProperties(XmlNode node)
        {
            base.SetProperties(node);
            if (node.Attributes == null) return;
            Tip = node.Attributes["tip"].Value;
            ImgUrl = node.Attributes["imgUrl"].Value;
        }
    }
}