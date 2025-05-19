using System.Windows;
using QuanLyNhaSach.ViewModels.SachViewModel;

namespace QuanLyNhaSach.Views.SachViews
{
    public partial class CapNhatSachWindow : Window
    {
        public CapNhatSachWindow(CapNhatSachViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
