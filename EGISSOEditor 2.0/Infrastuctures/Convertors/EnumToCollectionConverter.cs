using EGISSOEditor_2._0.Infrastuctures.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace EGISSOEditor_2._0.Infrastuctures.Convertors
{
    [ValueConversion(typeof(Enum), typeof(IEnumerable<ValueDescription>))]
    [MarkupExtensionReturnType(typeof(EnumToCollectionConverter))]
    internal class EnumToCollectionConverter : Base.Convertor
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EnumHelper.GetAllValuesAndDescriptions(value.GetType());
        }
    }
}
