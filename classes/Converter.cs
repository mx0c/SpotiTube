using SpotiTube;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace App1
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public BooleanToVisibilityConverter(){}

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Collapsed;

            if (value is bool && (bool)value)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible);
        }
    }

    public class InvBooleanToVisibilityConverter : IValueConverter
    {
        public InvBooleanToVisibilityConverter() { }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Visible;

            if (value is bool && (bool)value)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility && (Visibility)value == Visibility.Collapsed);
        }
    }

    public class BooleanToOpacityConverter : IValueConverter
    {
        public BooleanToOpacityConverter() { }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return 0.3;

            if (value is bool && (bool)value)
            {
                return 1.0;
            }
            return 0.3;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((double)value == 1.0) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string base64string = value as string;
            return Helper.base64toBmp(base64string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}


