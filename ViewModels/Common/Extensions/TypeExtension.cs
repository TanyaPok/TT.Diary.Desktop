using System;

namespace TT.Diary.Desktop.ViewModels.Common.Extensions
{
    public static class TypeExtension
    {
        public static string GetNameWithoutGenericArity(this Type t)
        {
            string name = t.Name;
            int index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }
}
