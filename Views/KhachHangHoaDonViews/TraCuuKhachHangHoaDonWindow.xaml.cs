using System.Windows;
using QuanLyNhaSach.ViewModels.KhachHangHoaDonViewModel;

namespace QuanLyNhaSach.Views.KhachHangHoaDonViews
{
    public partial class TraCuuKhachHangHoaDonWindow : Window
    {
        public TraCuuKhachHangHoaDonWindow(TraCuuKhachHangHoaDonWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
