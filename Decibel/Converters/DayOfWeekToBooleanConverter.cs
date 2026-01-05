using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Decibel.Converters
{
    internal class DayOfWeekToBooleanConverter : IValueConverter
    {
         public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        // Lista -> View
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection<DayOfWeek> daysChecked && parameter is DayOfWeek dayButton)
            {
                return daysChecked.Contains(dayButton);
            }

            return false;
        }
    }
}
