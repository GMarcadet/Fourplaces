using Fourplaces.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Fourplaces.Converters
{
    class ImagePlaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int idImagePlace = (int)value;
            if ( ApiClient.NONE_IMAGE != idImagePlace )
            {
                return APIResources.buildImageURI(idImagePlace);
            } else
            {
                return "";
            }
           
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
