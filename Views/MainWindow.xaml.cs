using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuanLyNhaSach.Views.CustomAnimation;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Media;
using QuanLyNhaSach.Data;
using System;

namespace QuanLyNhaSach.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly double collapsedWidth = 60;
        private readonly double expandedWidth = 200;
        private readonly IServiceProvider _serviceProvider;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // configure the window
            WindowState = WindowState.Maximized;


            Loaded += (s, e) =>
            {
                NavigateToPage("BaoCao");
            };
        }


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                if (radioButton.Tag is string pageName)
                {
                    NavigateToPage(pageName);
                }
            }
        }

        private void NavigateToPage(string pageName)
        {
            MainContent.Visibility = Visibility.Collapsed;

            switch (pageName)
            {
                case "PhieuSach":
                    MainContent.Content = null;                        // Xóa nội dung hiện tại
                    StackPanelTabButton.Visibility = Visibility.Visible;  // Hiện StackPanel điều hướng (nếu cần)

                    // Tìm RadioButton có Tag = "PhieuSach" rồi check nó
                    var phieuSachButton = FindVisualChildren<RadioButton>(this)
                        .FirstOrDefault(rb => (rb.Tag as string) == "PhieuSach");

                    if (phieuSachButton != null)
                    {
                        phieuSachButton.IsChecked = true;             // Check nút tương ứng để load trang
                    }
                    break;
                case "BaoCao":
                    var baoCaoPage = _serviceProvider.GetRequiredService<BaoCaoViews.BaoCaoChiTietPage>();
                    MainContent.Navigate(baoCaoPage);               // Load trang báo cáo
                    break;
                case "KhachHang":
                    var khachHangPage = _serviceProvider.GetRequiredService<KhachHangViews.KhachHangPage>();
                    MainContent.Navigate(khachHangPage);          // Load trang khách hàng
                    break;
                case "Sach":
                    var sachPage = _serviceProvider.GetRequiredService<SachViews.SachPage>();
                    MainContent.Navigate(sachPage);                // Load trang sách
                    break;
                case "ThamSo":
                    var thamSoPage = _serviceProvider.GetRequiredService<ThamSoViews.ThamSoPage>();
                    MainContent.Navigate(thamSoPage);           // Load trang phiếu nhập
                    break;
                case "HoaDon":
                    var hoaDonPage = _serviceProvider.GetRequiredService<HoaDonBanViews.HoaDonBanPage>();
                    MainContent.Navigate(hoaDonPage);              // Load trang hóa đơn
                    break;
                case "PhieuThu":
                    var phieuThuPage = _serviceProvider.GetRequiredService<PhieuThuViews.PhieuThuPage>();
                    MainContent.Navigate(phieuThuPage);             // Load trang phiếu thu
                    break;
                default:
                    break;
            }

            // Now set it back to visible
            MainContent.Visibility = Visibility.Visible;

            // Re-apply Z-index fixes after navigation
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void MainContent_ContentRendered(object sender, EventArgs e)
        {
            // Make sure navigation buttons reflect current page
            // This ensures sync if navigation happens programmatically
        }

        private void MainContent_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
