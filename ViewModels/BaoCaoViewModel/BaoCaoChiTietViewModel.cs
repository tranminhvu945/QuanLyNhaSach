using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models.dto;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.BaoCaoViews;

namespace QuanLyNhaSach.ViewModels.BaoCaoViewModel
{
    public partial class BaoCaoChiTietViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IKhachHangService _khachHangService;
        private readonly IPhieuThuService _phieuThuService;
        private readonly IHoaDonService _hoaDonService;
        private readonly IChiTietHoaDonService _chiTietHoaDonService;
        private readonly ISachService _sachService;
        private readonly IPhieuNhapSachService _phieuNhapSachService;
        private readonly IChiTietPhieuNhapService _chiTietPhieuNhapService;
        public BaoCaoChiTietViewModel(
            IServiceProvider serviceProvider,
            IKhachHangService khachHangService,
            IPhieuThuService phieuThuService,
            IHoaDonService hoaDonService,
            IChiTietHoaDonService chiTietHoaDonService,
            ISachService sachService,
            IPhieuNhapSachService phieuNhapSachService,
            IChiTietPhieuNhapService chiTietPhieuNhapService
            )
        {
            _serviceProvider = serviceProvider;
            _khachHangService = khachHangService;
            _phieuThuService = phieuThuService;
            _hoaDonService = hoaDonService;
            _chiTietHoaDonService = chiTietHoaDonService;
            _sachService = sachService;
            _phieuNhapSachService = phieuNhapSachService;
            _chiTietPhieuNhapService = chiTietPhieuNhapService;
            WeakReferenceMessenger.Default.RegisterAll(this);

            InitializeMonthYearOptions();
            _ = InitializeCongNoData();
            _ = InitializeTonSachData();
        }

        public DefaultTooltip TonSachTooltip { get; set; } = new DefaultTooltip
        {
            SelectionMode = TooltipSelectionMode.OnlySender,
            FontSize = 16,
            FontFamily = new FontFamily("Nunito"),
            ShowTitle = true,
            Background = new SolidColorBrush(Color.FromRgb(250, 250, 250)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200))
        };

        public DefaultTooltip CongNoTooltip { get; set; } = new DefaultTooltip
        {
            SelectionMode = TooltipSelectionMode.OnlySender,
            FontSize = 16,
            FontFamily = new FontFamily("Nunito"),
            ShowTitle = true,
            Background = new SolidColorBrush(Color.FromRgb(250, 250, 250)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200))
        };

        public void InitializeMonthYearOptions()
        {
            var currentDate = DateTime.Now;
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;

            MonthOptions =
            [
                "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4",
                "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8",
                "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
            ];

            YearOptions = [];
            for (int i = currentYear - 4; i <= currentYear; i++)
            {
                YearOptions.Add(i);
            }

            SelectedTonSachMonth = MonthOptions[currentMonth - 1];
            SelectedTonSachYear = currentYear;
            SelectedCongNoMonth = MonthOptions[currentMonth - 1];
            SelectedCongNoYear = currentYear;
        }

        [ObservableProperty]
        private List<string> _monthOptions = [];

        [ObservableProperty]
        private List<int> _yearOptions = [];

        [ObservableProperty]
        private SeriesCollection _tonSachSeries = [];

        [ObservableProperty]
        private SeriesCollection _congNoSeries = [];

        [ObservableProperty]
        private string[] _tonSachLabels = null!;

        [ObservableProperty]
        private string[] _congNoLabels = null!;

        [ObservableProperty]
        private string _selectedTonSachMonth = $"Tháng {DateTime.Now.Month}";

        partial void OnSelectedTonSachMonthChanged(string value)
        {
            _ = InitializeTonSachData();
        }

        [ObservableProperty]
        private int _selectedTonSachYear = DateTime.Now.Year;
        partial void OnSelectedTonSachYearChanged(int value)
        {
            _ = InitializeTonSachData();
        }

        [ObservableProperty]
        private string _selectedCongNoMonth = $"Tháng {DateTime.Now.Month}";

        partial void OnSelectedCongNoMonthChanged(string value)
        {
            _ = InitializeCongNoData();
        }

        [ObservableProperty]
        private int _selectedCongNoYear = DateTime.Now.Year;
        private object _phieuNhapService;

        partial void OnSelectedCongNoYearChanged(int value)
        {
            _ = InitializeCongNoData();
        }

        [RelayCommand]
        private void TonSach()
        {
            try
            {
                var window = _serviceProvider.GetRequiredService<BaoCaoTonWindow>();
                int month = int.Parse(SelectedTonSachMonth.Replace("Tháng ", ""));
                int year = SelectedTonSachYear;
                WeakReferenceMessenger.Default.Send(new SelectedDateMessage(month, year));
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ báo cáo tồn sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void CongNo()
        {
            try
            {
                var window = _serviceProvider.GetRequiredService<BaoCaoCongNoWindow>();
                int month = int.Parse(SelectedCongNoMonth.Replace("Tháng ", ""));
                int year = SelectedCongNoYear;
                WeakReferenceMessenger.Default.Send(new SelectedDateMessage(month, year));
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ báo cáo công nợ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public async Task InitializeCongNoData()
        {
            try
            {
                // Lấy tháng từ chuỗi "Tháng x"
                if (!int.TryParse(new string(SelectedCongNoMonth.Where(char.IsDigit).ToArray()), out int selectedMonth))
                    return;

                int selectedYear = SelectedCongNoYear;

                // Lấy danh sách khách hàng
                var khachHangList = await _khachHangService.GetAllKhachHang();

                // Trường hợp không có khách hàng
                if (khachHangList == null || !khachHangList.Any())
                {
                    CongNoLabels = Array.Empty<string>();
                    CongNoSeries = new SeriesCollection();
                    return;
                }

                // Khởi tạo danh sách các tác vụ bất đồng bộ cho các khách hàng
                var tasks = khachHangList.Select(async khachHang =>
                {
                    try
                    {
                        // Lấy dữ liệu phiếu thu và phiếu xuất cho đại lý
                        var phieuThu = await _phieuThuService.GetPhieuThuByKhachHangId(khachHang.MaKhachHang);
                        var hoaDon = await _hoaDonService.GetHoaDonByKhachHangId(khachHang.MaKhachHang);

                        var phieuThus = phieuThu
                            .Where(p => p.NgayThu.Month == selectedMonth && p.NgayThu.Year == selectedYear);
                        var hoaDons = hoaDon
                            .Where(p => p.NgayLap.Month == selectedMonth && p.NgayLap.Year == selectedYear);

                        double tongPhieuThu = phieuThus.Sum(p => p.SoTienThu);
                        double tongHoaDon = hoaDons.Sum(p => p.TongTien);
                        double congNo = tongHoaDon - tongPhieuThu;

                        return (TenKhachHang: khachHang.TenKhachHang, CongNo: congNo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi xử lý đại lý {khachHang.TenKhachHang}: {ex.Message}");
                        return (TenKhachHang: khachHang.TenKhachHang, CongNo: 0);
                    }
                }).ToList();

                // Chờ tất cả các tác vụ hoàn thành và lấy kết quả
                var khachHangCongNoList = await Task.WhenAll(tasks);

                // Sắp xếp và lọc ra top 10 công nợ lớn nhất
                var filteredData = khachHangCongNoList
                    .OrderByDescending(d => d.CongNo)
                    .Take(10)
                    .ToArray();

                // Trường hợp không có dữ liệu
                if (!filteredData.Any())
                {
                    TonSachLabels = Array.Empty<string>();
                    TonSachSeries = new SeriesCollection();
                    return;
                }

                // Cập nhật label và series cho biểu đồ công nợ
                CongNoLabels = filteredData.Select(d => d.TenKhachHang).ToArray();
                var sortedDebts = filteredData.Select(d => d.CongNo).ToArray();

                CongNoSeries = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Công nợ",
                        Values = new ChartValues<double>(sortedDebts),
                        DataLabels = true,
                        LabelPoint = point => point.Y.ToString("N0") + " đ",
                        Fill = new SolidColorBrush(Color.FromRgb(233, 30, 99)),
                        MaxColumnWidth = 50
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi khởi tạo dữ liệu công nợ: {ex.Message}");
            }
        }

        public async Task InitializeTonSachData()
        {
            try
            {
                // Lấy tháng từ chuỗi "Tháng x"
                if (!int.TryParse(new string(SelectedTonSachMonth.Where(char.IsDigit).ToArray()), out int selectedMonth))
                    return;

                int selectedYear = SelectedTonSachYear;

                // Lấy danh sách các sách
                var sachList = await _sachService.GetAllSach();

                if (sachList == null || !sachList.Any())
                {
                    TonSachLabels = Array.Empty<string>();
                    TonSachSeries = new SeriesCollection();
                    return;
                }

                // Khởi tạo danh sách các tác vụ bất đồng bộ cho các khách hàng
                var tasks = sachList.Select(async sach =>
                {
                    try
                    {
                        var chiTietPhieuNhapList = await _chiTietPhieuNhapService.GetAllChiTietPhieuNhap();
                        var chiTietPhieuNhapTheoSach = chiTietPhieuNhapList
                            .Where(ct => ct.MaSach == sach.MaSach)
                            .ToList();

                        var chiTietHoaDonList = await _chiTietHoaDonService.GetAllChiTietHoaDon();
                        var chiTietHoaDonTheoSach = chiTietHoaDonList
                            .Where(ct => ct.MaSach == sach.MaSach)
                            .ToList();

                        int phatSinh = 0;
                        foreach (var chiTiet in chiTietPhieuNhapTheoSach)
                        {
                            var phieuNhap = await _phieuNhapSachService.GetPhieuNhapById(chiTiet.MaPhieuNhapSach);

                            // Tính tồn đầu tháng (tất cả tồn trước tháng được tính)
                            if (phieuNhap.NgayNhap.Month == selectedMonth && phieuNhap.NgayNhap.Year == selectedYear)
                                phatSinh += chiTiet.SoLuongNhap;
                        }

                        foreach (var chiTiet in chiTietHoaDonTheoSach)
                        {
                            var hoaDon = await _hoaDonService.GetHoaDonById(chiTiet.MaHoaDon);
                            if (hoaDon.NgayLap.Month == selectedMonth && hoaDon.NgayLap.Year == selectedYear)
                                phatSinh -= chiTiet.SoLuongBan;
                        }

                        return (TenSach: sach.TenSach, TonSach: phatSinh);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi xử lý đại lý {sach.TenSach}: {ex.Message}");
                        return (TenSach: sach.TenSach, TonSach: 0);
                    }
                }).ToList();

                // Chờ tất cả các tác vụ hoàn thành và lấy kết quả
                var khachHangCongNoList = await Task.WhenAll(tasks);

                // Sắp xếp và lọc ra top 10 công nợ lớn nhất
                var filteredData = khachHangCongNoList
                    .OrderByDescending(d => d.TonSach)
                    .Take(10)
                    .ToArray();

                // Trường hợp không có dữ liệu
                if (!filteredData.Any())
                {
                    TonSachLabels = Array.Empty<string>();
                    TonSachSeries = new SeriesCollection();
                    return;
                }

                // Cập nhật label và series cho biểu đồ công nợ
                TonSachLabels = filteredData.Select(d => d.TenSach).ToArray();
                var sortedDebts = filteredData.Select(d => d.TonSach).ToArray();

                TonSachSeries = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Tồn sách",
                        Values = new ChartValues<int>(sortedDebts),
                        DataLabels = true,
                        LabelPoint = point => point.Y.ToString("N0"),
                        Fill = new SolidColorBrush(Color.FromRgb(233, 30, 99)),
                        MaxColumnWidth = 50
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi khởi tạo dữ liệu công nợ: {ex.Message}");
            }
        }
    }
}