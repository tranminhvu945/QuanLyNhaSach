using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Helpers
{
    public class ComboBoxItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            if (value is Sach sach)
                return sach.TenSach;

            if (value is KhachHang khachhang)
                return khachhang.TenKhachHang;

            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter is only used for display, so we don't need to implement ConvertBack
            throw new NotImplementedException();
        }
    }
}
