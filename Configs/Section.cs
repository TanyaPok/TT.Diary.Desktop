using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public class Section<T> : IConfigurationSectionHandler
        where T : MenuItem, new()
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            IList<T> diaryTypes = new List<T>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                var item = new T();
                item.SetProperties(childNode);
                diaryTypes.Add(item);
            }

            return diaryTypes;
        }
    }
}
