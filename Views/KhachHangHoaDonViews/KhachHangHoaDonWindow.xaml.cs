using System.Windows;
using QuanLyNhaSach.ViewModels.KhachHangHoaDonViewModel;

namespace QuanLyNhaSach.Views.KhachHangHoaDonViews
{
    public partial class KhachHangHoaDonWindow : Window
    {
        public KhachHangHoaDonWindow(KhachHangHoaDonWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
