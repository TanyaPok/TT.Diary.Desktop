using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public class ListType : AbstractSectionItem
    {
        public const string WISH_LIST = "Wish list";

        public const string TODO_LIST = "To-do list";

        public const string APPOINTMENTS = "Appointments";

        public const string HABITS = "Habits";

        public const string NOTES = "Notes";

        public string Tip { get; private set; }

        public override void SetProperties(XmlNode node)
        {
            base.SetProperties(node);
            Tip = node.Attributes["tip"].Value;
        }
    }
}
