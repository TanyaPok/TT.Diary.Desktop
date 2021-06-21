using System;

namespace TT.Diary.Desktop.ViewModels.Extensions
{
    public static class TypeExtension
    {
        public static string GetNameWithoutGenericArity(this Type t)
        {
            var name = t.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }
}