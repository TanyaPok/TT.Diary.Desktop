using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public class ScheduleType : AbstractSectionItem
    {
        public const string DAY = "Day";

        public const string WEEK = "Week";

        public const string MONTH = "Month";

        public const string YEAR = "Year";

        public string Tip { get; private set; }

        public string ImgUrl { get; private set; }

        public override void SetProperties(XmlNode node)
        {
            base.SetProperties(node);
            Tip = node.Attributes["tip"].Value;
            ImgUrl = node.Attributes["imgUrl"].Value;
        }
    }
}
