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
                // Trích xuất số tháng từ chuỗi "Tháng x"
                if (!int.TryParse(new string(SelectedCongNoMonth.Where(char.IsDigit).ToArray()), out int selectedMonth))
                    return;

                int selectedYear = SelectedCongNoYear;

                var khachHangList = await _khachHangService.GetAllKhachHang();

                if (khachHangList == null || !khachHangList.Any())
                {
                    CongNoLabels = Array.Empty<string>();
                    CongNoSeries = new SeriesCollection();
                    return;
                }

                var tasks = khachHangList.Select(async kh =>
                {
                    try
                    {
                        var phieuThuList = await _phieuThuService.GetPhieuThuByKhachHangId(kh.MaKhachHang);
                        var hoaDonList = await _hoaDonService.GetHoaDonByKhachHangId(kh.MaKhachHang);

                        // Lọc theo tháng + năm
                        static bool IsInSelectedMonthYear(DateTime date, int month, int year)
                            => date.Month == month && date.Year == year;

                        double tongPhieuThu = phieuThuList
                            .Where(p => IsInSelectedMonthYear(p.NgayThu, selectedMonth, selectedYear))
                            .Sum(p => p.SoTienThu);

                        double tongHoaDon = hoaDonList
                            .Where(p => IsInSelectedMonthYear(p.NgayLap, selectedMonth, selectedYear))
                            .Sum(p => p.TongTien);

                        double congNo = tongHoaDon - tongPhieuThu;

                        return (kh.TenKhachHang, CongNo: congNo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi xử lý khách hàng {kh.TenKhachHang}: {ex.Message}");
                        return (kh.TenKhachHang, CongNo: 0);
                    }
                }).ToList();

                var results = await Task.WhenAll(tasks);

                var top10CongNo = results
                    .OrderByDescending(r => r.CongNo)
                    .Take(10)
                    .ToArray();

                if (!top10CongNo.Any())
                {
                    TonSachLabels = Array.Empty<string>();
                    TonSachSeries = new SeriesCollection();
                    return;
                }

                // Cập nhật biểu đồ
                CongNoLabels = top10CongNo.Select(r => r.TenKhachHang).ToArray();
                var debtValues = top10CongNo.Select(r => r.CongNo).ToArray();

                CongNoSeries = new SeriesCollection
        {
            new ColumnSeries
            {
                Title = "Công nợ",
                Values = new ChartValues<double>(debtValues),
                DataLabels = true,
                LabelPoint = point => point.Y.ToString("N0") + " VNĐ",
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
                if (!int.TryParse(new string(SelectedTonSachMonth.Where(char.IsDigit).ToArray()), out int selectedMonth))
                    return;

                int selectedYear = SelectedTonSachYear;

                var sachList = await _sachService.GetAllSach();
                if (sachList == null || !sachList.Any())
                {
                    TonSachLabels = Array.Empty<string>();
                    TonSachSeries = new SeriesCollection();
                    return;
                }

                // Load toàn bộ dữ liệu một lần
                var chiTietPhieuNhapList = await _chiTietPhieuNhapService.GetAllChiTietPhieuNhap();
                var phieuNhapDict = (await _phieuNhapSachService.GetAllPhieuNhap()).ToDictionary(p => p.MaPhieuNhapSach);

                var chiTietHoaDonList = await _chiTietHoaDonService.GetAllChiTietHoaDon();
                var hoaDonDict = (await _hoaDonService.GetAllHoaDon()).ToDictionary(h => h.MaHoaDon);

                // Gom nhóm theo mã sách để truy vấn nhanh hơn
                var phieuNhapLookup = chiTietPhieuNhapList.ToLookup(ct => ct.MaSach);
                var hoaDonLookup = chiTietHoaDonList.ToLookup(ct => ct.MaSach);

                // Tính tồn sách cho từng quyển
                var result = sachList.Select(sach =>
                {
                    int tonSach = 0;

                    foreach (var chiTiet in phieuNhapLookup[sach.MaSach])
                    {
                        if (phieuNhapDict.TryGetValue(chiTiet.MaPhieuNhapSach, out var phieuNhap))
                        {
                            if (phieuNhap.NgayNhap.Month == selectedMonth && phieuNhap.NgayNhap.Year == selectedYear)
                                tonSach += chiTiet.SoLuongNhap;
                        }
                    }

                    foreach (var chiTiet in hoaDonLookup[sach.MaSach])
                    {
                        if (hoaDonDict.TryGetValue(chiTiet.MaHoaDon, out var hoaDon))
                        {
                            if (hoaDon.NgayLap.Month == selectedMonth && hoaDon.NgayLap.Year == selectedYear)
                                tonSach -= chiTiet.SoLuongBan;
                        }
                    }

                    return (TenSach: sach.TenSach, TonSach: tonSach);
                })
                .OrderByDescending(d => d.TonSach)
                .Take(10)
                .ToArray();

                // Hiển thị biểu đồ
                if (!result.Any())
                {
                    TonSachLabels = Array.Empty<string>();
                    TonSachSeries = new SeriesCollection();
                    return;
                }

                TonSachLabels = result.Select(d => d.TenSach).ToArray();
                var sortedDebts = result.Select(d => d.TonSach).ToArray();

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
                Console.WriteLine($"Lỗi khi khởi tạo dữ liệu tồn sách: {ex.Message}");
            }
        }

    }
}