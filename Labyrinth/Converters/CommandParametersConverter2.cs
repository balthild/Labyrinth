using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Labyrinth.Converters {
    public class CommandParametersConverter2 : IMultiValueConverter {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) {
            return (values[0], values[1]);
        }
    }
}
