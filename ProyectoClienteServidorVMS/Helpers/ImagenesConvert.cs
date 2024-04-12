using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProyectoClienteServidorVMS.Helpers
{
    public class ImagenesConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var img = (string)value;

            if (img == "uc")
                return "/Assets/senal.png";
            else if (img == "slo")
                return "/Assets/slow.png";
            else if (img == "sem")
                return "/Assets/sem.png";
            else
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
