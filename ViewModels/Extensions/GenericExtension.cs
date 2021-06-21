using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace TT.Diary.Desktop.ViewModels.Extensions
{
    [ContentProperty("TypeArguments")]
    public class GenericExtension : MarkupExtension
    {
        public Collection<Type> TypeArguments { get; set; }

        public string TypeName { get; set; }

        public GenericExtension()
        {
            TypeArguments = new Collection<Type>();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var genericType = Type.GetType(TypeName);
            var typeArgumentArray = new Type[TypeArguments.Count];
            TypeArguments.CopyTo(typeArgumentArray, 0);
            var concreteType = genericType.MakeGenericType(typeArgumentArray);
            return concreteType;
        }
    }
}