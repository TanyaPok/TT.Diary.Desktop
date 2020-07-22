using System.Collections.Generic;
using System.Configuration;
using TT.Diary.Desktop.Configs;

namespace TT.Diary.Desktop.ViewModel
{
    public class Context
    {
        private readonly IList<DiaryType> _diaryTypes;
        public IList<DiaryType> DiaryTypes { get { return _diaryTypes; } }
        private readonly IList<ListType> _listTypes;
        public IList<ListType> ListTypes { get { return _listTypes; } }
        private readonly string _dictionaryTip;
        public string DictionaryTip
        {
            get { return _dictionaryTip; }
        }

        public Context()
        {
            _diaryTypes = ConfigurationManager.GetSection("diaryTypes") as IList<DiaryType>;
            _listTypes = ConfigurationManager.GetSection("listTypes") as IList<ListType>;
            _dictionaryTip = ConfigurationManager.AppSettings["dictionaryTip"];
        }
    }
}
