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

        private void DienThoaiTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is ThemHoaDonBanViewModel vm)
            {
                if (vm.TimKhachHangTheoDienThoaiCommand.CanExecute(null))
                {
                    vm.TimKhachHangTheoDienThoaiCommand.Execute(null);
                }
            }
        }
    }
}
