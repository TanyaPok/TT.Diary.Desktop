using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace TT.Diary.Desktop.Configs
{
    public class Section<T> : IConfigurationSectionHandler where T : AbstractSectionItem, new()
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            IList<T> list = new List<T>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                var item = new T();
                item.SetProperties(childNode);
                list.Add(item);
            }

            return list;
        }
    }
}