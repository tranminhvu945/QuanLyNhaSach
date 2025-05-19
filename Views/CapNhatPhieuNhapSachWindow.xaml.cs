using System.Windows;
using QuanLyNhaSach.ViewModels.PhieuNhapSachViewModel;

namespace QuanLyNhaSach.Views
{
    /// <summary>
    /// Interaction logic for CapNhatPhieuNhapSachWindow.xaml
    /// </summary>
    public partial class CapNhatPhieuNhapSachWindow : Window
    {
        public CapNhatPhieuNhapSachWindow(CapNhatPhieuNhapSachViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
