using System.Windows.Controls;
using QuanLyNhaSach.ViewModels.PhieuThuViewModel;

namespace QuanLyNhaSach.Views.PhieuThuViews
{
    public partial class PhieuThuPage : Page
    {
        public PhieuThuPage(PhieuThuPageViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
