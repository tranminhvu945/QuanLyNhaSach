using System.Windows;
using QuanLyNhaSach.ViewModels.SachViewModel;

namespace QuanLyNhaSach.Views.SachViews
{
    public partial class ThemSachWindow : Window
    {
        public ThemSachWindow(ThemSachViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
