using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public class DiaryType : MenuItem
    {
        public string ImgUrl { get; set; }

        public override void SetProperties(XmlNode node)
        {
            base.SetProperties(node);
            ImgUrl = node.Attributes["imgUrl"].Value;
        }
    }
}
