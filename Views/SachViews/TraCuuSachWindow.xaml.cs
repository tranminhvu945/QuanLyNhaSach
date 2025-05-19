using System.Windows;
using QuanLyNhaSach.ViewModels.SachViewModel;

namespace QuanLyNhaSach.Views.SachViews
{
    public partial class TraCuuSachWindow : Window
    {
        public TraCuuSachWindow(TraCuuSachViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
