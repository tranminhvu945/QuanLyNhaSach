using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.KhachHangHoaDonViews;
using QuanLyNhaSach.Views.KhachHangViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuanLyNhaSach.ViewModels.KhachHangHoaDonViewModel
{
    public partial class TraCuuKhachHangHoaDonWindowViewModel : ObservableObject
    {
        private readonly IKhachHangService _khachHangService;
        private readonly IPhieuThuService _phieuThuService;
        private readonly IHoaDonService _hoaDonService;
        public TraCuuKhachHangHoaDonWindowViewModel(
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

        #region binding properties
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
        private string tienNoFrom = "";
        [ObservableProperty]
        private string tienNoTo = "";
        [ObservableProperty]
        private DateTime _ngayThuFrom = DateTime.MinValue;
        [ObservableProperty]
        private DateTime _ngayThuTo = DateTime.Now;
        [ObservableProperty]
        private string soTienThuFrom = "";

        [ObservableProperty]
        private string soTienThuTo = "";
        [ObservableProperty]
        private DateTime _ngayLapFrom = DateTime.MinValue;
        [ObservableProperty]
        private DateTime _ngayLapTo = DateTime.Now;
        [ObservableProperty]
        private string tongTienFrom = "";

        [ObservableProperty]
        private string tongTienTo = "";
        #endregion

        #region functional
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

                // Parse TienNoFrom, TienNoTo từ string sang long
                long tienNoFromVal = 0;
                long tienNoToVal = long.MaxValue;

                if (!string.IsNullOrWhiteSpace(TienNoFrom) && !long.TryParse(TienNoFrom, out tienNoFromVal))
                {
                    tienNoFromVal = 0;
                }

                if (!string.IsNullOrWhiteSpace(TienNoTo) && !long.TryParse(TienNoTo, out tienNoToVal))
                {
                    tienNoToVal = long.MaxValue;
                }

                // Kiểm tra điều kiện lọc tiền nợ
                if (tienNoFromVal != 0 || tienNoToVal != long.MaxValue)
                {
                    filteredResults = new ObservableCollection<KhachHang>(
                        filteredResults.Where(d => d.TienNo >= tienNoFromVal && d.TienNo <= tienNoToVal));
                }

                // Parse SoTienThuFrom, SoTienThuTo từ string sang long
                long soTienThuFromVal = 0;
                long soTienThuToVal = long.MaxValue;

                if (!string.IsNullOrWhiteSpace(SoTienThuFrom) && !long.TryParse(SoTienThuFrom, out soTienThuFromVal))
                {
                    soTienThuFromVal = 0;
                }

                if (!string.IsNullOrWhiteSpace(SoTienThuTo) && !long.TryParse(SoTienThuTo, out soTienThuToVal))
                {
                    soTienThuToVal = long.MaxValue;
                }

                // Kiểm tra điều kiện lọc Phiếu Thu (Ngày thu và số tiền thu)
                if (NgayThuFrom != DateTime.MinValue || NgayThuTo != DateTime.Now || soTienThuFromVal != 0 || soTienThuToVal != long.MaxValue)
                {
                    var tasks = filteredResults.Select(async khachHang =>
                    {
                        var phieuThus = await _phieuThuService.GetPhieuThuByKhachHangId(khachHang.MaKhachHang);

                        bool hasPhieuThuInRange = phieuThus.Any(pt =>
                            pt.NgayThu >= NgayThuFrom && pt.NgayThu <= NgayThuTo &&
                            pt.SoTienThu >= soTienThuFromVal && pt.SoTienThu <= soTienThuToVal
                        );

                        return (khachHang, hasPhieuThuInRange);
                    });

                    var results = await Task.WhenAll(tasks);

                    filteredResults = new ObservableCollection<KhachHang>(
                        results.Where(result => result.hasPhieuThuInRange).Select(result => result.khachHang));
                }

                // Tìm kiếm theo HoaDon (ngày lập và tổng tiền)
                // Parse TongTienFrom, TongTienTo từ string sang long
                long tongTienFromVal = 0;
                long tongTienToVal = long.MaxValue;

                if (!string.IsNullOrWhiteSpace(TongTienFrom) && !long.TryParse(TongTienFrom, out tongTienFromVal))
                {
                    tongTienFromVal = 0;
                }

                if (!string.IsNullOrWhiteSpace(TongTienTo) && !long.TryParse(TongTienTo, out tongTienToVal))
                {
                    tongTienToVal = long.MaxValue;
                }

                if (NgayLapFrom != DateTime.MinValue || NgayLapTo != DateTime.Now || tongTienFromVal != 0 || tongTienToVal != long.MaxValue)
                {
                    var tasks = filteredResults.Select(async khachHang =>
                    {
                        var hoaDons = await _hoaDonService.GetHoaDonByKhachHangId(khachHang.MaKhachHang);

                        bool hasHoaDonInRange = hoaDons.Any(hd =>
                            hd.NgayLap >= NgayLapFrom && hd.NgayLap <= NgayLapTo &&
                            hd.TongTien >= tongTienFromVal && hd.TongTien <= tongTienToVal
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
            Application.Current.Windows.OfType<TraCuuKhachHangHoaDonWindow>().FirstOrDefault()?.Close();
        }

        private void ApplySearchResults()
        {
            WeakReferenceMessenger.Default.Send(new SearchCompletedMessage<KhachHang>(SearchResults));
            Close();
        }
        #endregion
    }
}
