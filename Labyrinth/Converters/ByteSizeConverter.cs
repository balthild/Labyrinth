using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Labyrinth.Support;

namespace Labyrinth.Converters {
    public class ByteSizeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return Utils.BinarySize((long) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
