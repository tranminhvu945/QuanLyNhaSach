using System.Windows;
using QuanLyNhaSach.ViewModels.KhachHangHoaDonViewModel;

namespace QuanLyNhaSach.Views.KhachHangHoaDonViews
{
    public partial class ThemKhachHangHoaDonWindow : Window
    {
        public ThemKhachHangHoaDonWindow(ThemKhachHangHoaDonWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
