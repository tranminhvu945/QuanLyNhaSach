using QuanLyNhaSach.ViewModels.HoaDonBanViewModel;
using System.Windows.Controls;

namespace QuanLyNhaSach.Views.HoaDonBanViews
{
    public partial class HoaDonBanPage : Page
    {
        public HoaDonBanPage(HoaDonBanPageViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
