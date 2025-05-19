using System.Windows;
using QuanLyNhaSach.ViewModels.HoaDonBanViewModel;

namespace QuanLyNhaSach.Views.HoaDonBanViews
{
    public partial class CapNhatHoaDonBanWindow : Window
    {
        public CapNhatHoaDonBanWindow(CapNhatHoaDonBanViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
