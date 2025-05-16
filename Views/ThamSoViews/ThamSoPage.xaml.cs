using System.Windows.Controls;
using QuanLyNhaSach.ViewModels.ThamSoViewModel;

namespace QuanLyNhaSach.Views.ThamSoViews
{
    public partial class ThamSoPage : Page
    {
        public ThamSoPage(ThamSoPageViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
