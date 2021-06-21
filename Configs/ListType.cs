using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public class ListType : AbstractSectionItem
    {
        public const string WishList = "Wish list";

        public const string TodoList = "To-do list";

        public const string Appointments = "Appointments";

        public const string Habits = "Habits";

        public const string Notes = "Notes";

        public string Tip { get; private set; }

        public override void SetProperties(XmlNode node)
        {
            base.SetProperties(node);
            if (node.Attributes != null) Tip = node.Attributes["tip"].Value;
        }
    }
}