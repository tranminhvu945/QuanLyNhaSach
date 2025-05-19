using System.Windows;
using QuanLyNhaSach.ViewModels.HoaDonBanViewModel;

namespace QuanLyNhaSach.Views.HoaDonBanViews
{

    public partial class ThemHoaDonBanWindow : Window
    {
        public ThemHoaDonBanWindow(ThemHoaDonBanViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
