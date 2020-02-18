using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.Converters
{
    class UserToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UserItem user = value as UserItem;
            return user.FirstName + " " + user.LastName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
