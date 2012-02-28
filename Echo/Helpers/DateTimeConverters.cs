using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;

namespace Echo.Helpers
{
    public class ShortTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var dateTime = (DateTime)value;

            return dateTime.ToShortTimeString();
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            throw new NotSupportedException();

        }

    }

    public class ShortDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var dateTime = (DateTime)value;

            return dateTime.ToShortDateString();
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            throw new NotSupportedException();

        }

    }

}
