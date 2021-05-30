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

    [ValueConversion(typeof(double), typeof(double))]
    [MarkupExtensionReturnType(typeof(Linear))]
    internal class Linear : Base.Convertor
    {
        [ConstructorArgument(nameof(K))]
        public double K { get; set; } = 1;

        [ConstructorArgument(nameof(B))]
        public double B { get; set; }

        public Linear() { }
        public Linear(double K) => this.K = K;
        public Linear(double K, double B) : this(K) => this.B = B;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                throw new NotImplementedException();
            var x = System.Convert.ToDouble(value, culture);

            return K * x + B;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                throw new NotImplementedException();
            var x = System.Convert.ToDouble(value, culture);

            return (x - B) / B;
        }
    }
}
