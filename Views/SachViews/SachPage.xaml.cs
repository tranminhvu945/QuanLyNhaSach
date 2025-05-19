using QuanLyNhaSach.ViewModels.SachViewModel;
using System.Windows.Controls;

namespace QuanLyNhaSach.Views.SachViews
{
    public partial class SachPage : Page
    {
        public SachPage(SachPageViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
