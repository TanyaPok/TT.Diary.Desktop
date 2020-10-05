using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public class ScheduleType : AbstractMenuItem
    {
        public const string DAY = "Day";
        public const string WEEK = "Week";
        public const string MONTH = "Month";
        public const string YEAR = "Year";
        public string ImgUrl { get; private set; }

        public override void SetProperties(XmlNode node)
        {
            base.SetProperties(node);
            ImgUrl = node.Attributes["imgUrl"].Value;
        }
    }
}
