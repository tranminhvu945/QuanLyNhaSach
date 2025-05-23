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

        private void DienThoaiTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is CapNhatHoaDonBanViewModel vm)
            {
                if (vm.TimKhachHangTheoDienThoaiCommand.CanExecute(null))
                {
                    vm.TimKhachHangTheoDienThoaiCommand.Execute(null);
                }
            }
        }
    }
}
