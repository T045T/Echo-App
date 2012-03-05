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
using Echo.Model;

namespace Echo.Helpers
{
    /// <summary>
    /// Converter that makes a single "Full Name" string out of an array of the form [FirstName, LastName].
    /// The conversion parameter is a boolean value determining which order to print the names in ("First Last" if true, "Last, First" if false).
    /// </summary>
    public class NameConverter : IValueConverter
    {
        private SettingsModel sm;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            sm = new SettingsModel();
            bool FirstNameFirst = sm.getValueOrDefault<bool>(sm.NameOrderSettingKeyName, sm.NameOrderDefault);
            var array = value as string[];
            if (FirstNameFirst)
            {
                return array[0] + " " + array[1];
            }
            else
            {
                return array[1] + ", " + array[0];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string input = value as string;
            if (input.Contains(", "))
            {
                return new string[] { input.Substring(input.LastIndexOf(',') + 2), input.Substring(0, input.LastIndexOf('c')) };
            }
            else
            {
                return new string[] { input.Substring(0, input.LastIndexOf(' ')), input.Substring(input.LastIndexOf(' ') + 1) };
            }
        }
    }
}
