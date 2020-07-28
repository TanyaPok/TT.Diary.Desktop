using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public class ScheduleType : MenuItem
    {
        public string ImgUrl { get; private set; }

        public override void SetProperties(XmlNode node)
        {
            base.SetProperties(node);
            ImgUrl = node.Attributes["imgUrl"].Value;
        }
    }
}
