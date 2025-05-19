using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using QuanLyNhaSach.ViewModels.HoaDonBanViewModel;
using System.Windows.Controls;

namespace QuanLyNhaSach.Views.HoaDonBanViews
{
    /// <summary>
    /// Interaction logic for HoaDonBanPage.xaml
    /// </summary>
    public partial class HoaDonBanPage : Page
    {
        public HoaDonBanPage(HoaDonBanPageViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
