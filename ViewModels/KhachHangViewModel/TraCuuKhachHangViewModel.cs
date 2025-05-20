using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.KhachHangViews;

namespace QuanLyNhaSach.ViewModels.KhachHangViewModel
{
    public partial class TraCuuKhachHangViewModel: ObservableObject
    {
        private readonly IKhachHangService _khachHangService;
        private readonly IPhieuThuService _phieuThuService;
        private readonly IHoaDonService _hoaDonService;
        public TraCuuKhachHangViewModel(
            IKhachHangService khachHangService,
            IPhieuThuService phieuThuService,
            IHoaDonService hoaDonService
            )
        {
            _khachHangService = khachHangService;
            _phieuThuService = phieuThuService;
            _hoaDonService = hoaDonService;
            WeakReferenceMessenger.Default.RegisterAll(this);

            _ = LoadDataAsync();
        }

        [ObservableProperty]
        private List<KhachHang> _khachHangList = [];
        [ObservableProperty]
        private KhachHang _selectedKhachHang = new();
        [ObservableProperty]
        private KhachHang _tenKhachHang;
        public ObservableCollection<KhachHang> SearchResults = [];
        [ObservableProperty]
        private string _maKhachHang = string.Empty;
        //[ObservableProperty]
        //private string _tenKhachHang = string.Empty;
        [ObservableProperty]
        private string _dienThoai = string.Empty;
        [ObservableProperty]
        private string _diaChi = string.Empty;
        [ObservableProperty]
        private string _email = string.Empty;
        [ObservableProperty]
        private long _tienNoFrom = 0;
        [ObservableProperty]
        private long _tienNoTo = long.MaxValue;
        [ObservableProperty]
        private DateTime _ngayThuFrom = DateTime.MinValue;
        [ObservableProperty]
        private DateTime _ngayThuTo = DateTime.Now;
        [ObservableProperty]
        private long _soTienThuFrom = 0;
        [ObservableProperty]
        private long _soTienThuTo = long.MaxValue;
        [ObservableProperty]
        private DateTime _ngayLapFrom = DateTime.MinValue;
        [ObservableProperty]
        private DateTime _ngayLapTo = DateTime.Now;
        [ObservableProperty]
        private long _tongTienFrom = 0; 
        [ObservableProperty]
        private long _tongTienTo = long.MaxValue; 
        private async Task LoadDataAsync()
        {
            try
            {
                var listKhachHang = await _khachHangService.GetAllKhachHang();

                KhachHangList.Clear();
                KhachHangList = [.. listKhachHang];

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task SearchKhachHang()
        {
            try
            {
                var khachHangs = await _khachHangService.GetAllKhachHang();
                ObservableCollection<KhachHang> filteredResults = [.. khachHangs];

                if (!string.IsNullOrEmpty(MaKhachHang))
                {
                    filteredResults = [.. filteredResults.Where(d => d.MaKhachHang.ToString().Contains(MaKhachHang))];
                }
                if (!string.IsNullOrEmpty(DienThoai))
                {
                    filteredResults = [.. filteredResults.Where(d => d.DienThoai.Contains(DienThoai))];
                }
                if (!string.IsNullOrEmpty(DiaChi))
                {
                    filteredResults = [.. filteredResults.Where(d => d.DiaChi.Contains(DiaChi))];
                }
                if (!string.IsNullOrEmpty(Email))
                {
                    filteredResults = [.. filteredResults.Where(d => d.Email.Contains(Email))];
                }
                if (!string.IsNullOrEmpty(SelectedKhachHang.TenKhachHang))
                {
                    filteredResults = [.. filteredResults.Where(d => d.TenKhachHang == SelectedKhachHang.TenKhachHang)];
                }
                // Tìm kiếm theo tiền nợ (từ - đến)
                if (TienNoFrom != 0 || TienNoTo != long.MaxValue)
                {
                    filteredResults = [.. filteredResults.Where(d => d.TienNo >= TienNoFrom && d.TienNo <= TienNoTo)];
                }

                // Tìm kiếm theo PhieuThu (ngày thu và số tiền thu)
                if (NgayThuFrom != DateTime.MinValue || NgayThuTo != DateTime.Now || SoTienThuFrom != 0 || SoTienThuTo != long.MaxValue)
                {
                    var tasks = filteredResults.Select(async khachHang =>
                    {
                        var phieuThus = await _phieuThuService.GetPhieuThuByKhachHangId(khachHang.MaKhachHang);

                        // Kiểm tra có Phiếu Thu nào thỏa mãn cả ngày thu và tiền thu
                        bool hasPhieuThuInRange = phieuThus.Any(pt =>
                            pt.NgayThu >= NgayThuFrom && pt.NgayThu <= NgayThuTo &&
                            pt.SoTienThu >= SoTienThuFrom && pt.SoTienThu <= SoTienThuTo
                        );

                        return (khachHang, hasPhieuThuInRange);
                    });

                    var results = await Task.WhenAll(tasks);

                    filteredResults = new ObservableCollection<KhachHang>(
                        results.Where(result => result.hasPhieuThuInRange).Select(result => result.khachHang));
                }

                // Tìm kiếm theo HoaDon (ngày lập và tổng tiền)
                if (NgayLapFrom != DateTime.MinValue || NgayLapTo != DateTime.Now || TongTienFrom != 0 || TongTienTo != long.MaxValue)
                {
                    var tasks = filteredResults.Select(async khachHang =>
                    {
                        var hoaDons = await _hoaDonService.GetHoaDonByKhachHangId(khachHang.MaKhachHang);

                        // Kiểm tra có Phiếu Thu nào thỏa mãn cả ngày thu và tiền thu
                        bool hasHoaDonInRange = hoaDons.Any(pt =>
                            pt.NgayLap >= NgayLapFrom && pt.NgayLap <= NgayLapTo &&
                            pt.TongTien >= TongTienFrom && pt.TongTien <= TongTienTo
                        );

                        return (khachHang, hasHoaDonInRange);
                    });

                    var results = await Task.WhenAll(tasks);

                    filteredResults = new ObservableCollection<KhachHang>(
                        results.Where(result => result.hasHoaDonInRange).Select(result => result.khachHang));
                }


                SearchResults = [.. filteredResults];

                ApplySearchResults();

                if (SearchResults.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy kết quả nào phù hợp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Close()
        {
            Application.Current.Windows.OfType<TraCuuKhachHangWindow>().FirstOrDefault()?.Close();
        }

        private void ApplySearchResults()
        {
            WeakReferenceMessenger.Default.Send(new SearchCompletedMessage<KhachHang>(SearchResults));
            Close();
        }
    }
}
