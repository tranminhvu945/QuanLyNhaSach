using System.Windows;
using QuanLyNhaSach.ViewModels.HoaDonBanViewModel;

namespace QuanLyNhaSach.Views.HoaDonBanViews
{
    public partial class TraCuuHoaDonBanWindow : Window
    {
        public TraCuuHoaDonBanWindow(TraCuuHoaDonBanViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
