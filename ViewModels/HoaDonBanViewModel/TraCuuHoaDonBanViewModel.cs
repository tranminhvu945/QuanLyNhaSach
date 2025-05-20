using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views;
using QuanLyNhaSach.Views.HoaDonBanViews;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;

namespace QuanLyNhaSach.ViewModels.HoaDonBanViewModel
{
    public partial class TraCuuHoaDonBanViewModel : ObservableObject
    {
        private readonly IHoaDonService _hoaDonService;
        private readonly IChiTietHoaDonService _hoaDonChiTietService;
        private readonly ISachService _sachService;
        private readonly IKhachHangService _khachHangService;
        private readonly IThamSoService _thamsoService;

        public TraCuuHoaDonBanViewModel(
            IHoaDonService hoaDonService,
            IChiTietHoaDonService hoaDonChiTietService,
            ISachService sachService,
            IKhachHangService khachHangService,
            IThamSoService thamsoService)
        {
            _hoaDonService = hoaDonService;
            _hoaDonChiTietService = hoaDonChiTietService;
            _sachService = sachService;
            _khachHangService = khachHangService;
            _thamsoService = thamsoService;


            _ = LoadDataAsync();
        }

        [ObservableProperty]
        private string _maHoaDon = string.Empty;

        [ObservableProperty]
        private string _tongTienFrom = string.Empty;

        [ObservableProperty]
        private string _tongTienTo = string.Empty;

        [ObservableProperty]
        private DateTime _ngayLapHoaDonFrom = DateTime.MinValue;

        [ObservableProperty]
        private DateTime _ngayLapHoaDonTo = DateTime.Now;

        [ObservableProperty]
        private ObservableCollection<KhachHang> _khachHangs = new ObservableCollection<KhachHang>();

        [ObservableProperty]
        private KhachHang _selectedKhachHang = new();

        [ObservableProperty]
        private string _dienThoai = string.Empty;

        [ObservableProperty]
        private string _diaChi = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _tienNoFrom = string.Empty;

        [ObservableProperty]
        private string _tienNoTo = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Sach> _sachs = new ObservableCollection<Sach>();

        [ObservableProperty]
        private Sach _selectedSach = new();

        [ObservableProperty]
        private string _theLoai = string.Empty;

        [ObservableProperty]
        private string _tacGia = string.Empty;

        [ObservableProperty]
        private string _soLuongTonFrom = string.Empty;

        [ObservableProperty]
        private string _soLuongTonTo = string.Empty;

        [ObservableProperty]
        private string _donGiaBanFrom = string.Empty;

        [ObservableProperty]
        private string _donGiaBanTo = string.Empty;

        [ObservableProperty]
        private string _soLuongBanFrom = string.Empty;

        [ObservableProperty]
        private string _soLuongBanTo = string.Empty;

        [ObservableProperty]
        private string _thanhTienFrom = string.Empty;

        [ObservableProperty]
        private string _thanhTienTo = string.Empty;

        [ObservableProperty]
        private ObservableCollection<HoaDon> _searchResults = [];

        private async Task LoadDataAsync()
        {
            try
            {
                var listKhachHang = await _khachHangService.GetAllKhachHang();
                var listSach = await _sachService.GetAllSach();

                KhachHangs.Clear();
                Sachs.Clear();


                // Populate the collections
                // Sắp xếp theo tên khách hàng (TenKhachHang)
                var sortedListKhachHang = listKhachHang.OrderBy(kh => kh.TenKhachHang).ToList();

                KhachHangs = new ObservableCollection<KhachHang>(sortedListKhachHang);

                // Sắp xếp theo tên sách (TenSach)
                var sortedListSach = listSach.OrderBy(kh => kh.TenSach).ToList();

                Sachs = new ObservableCollection<Sach>(sortedListSach);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySearchResults()
        {
            WeakReferenceMessenger.Default.Send(new SearchCompletedMessage<HoaDon>(SearchResults));
            // Close the window after applying HoaDon
            CloseWindow();
        }

        [RelayCommand]
        private void CloseWindow()
        {
            Application.Current.Windows.OfType<TraCuuHoaDonBanWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private async Task SearchHoaDon()
        {
            try
            {
                var hoaDons = await _hoaDonService.GetAllHoaDon();

                if (!string.IsNullOrEmpty(MaHoaDon))
                {
                    hoaDons = hoaDons.Where(h => h.MaHoaDon.ToString().Contains(MaHoaDon));
                }

                if (!string.IsNullOrEmpty(TongTienFrom) && !string.IsNullOrEmpty(TongTienTo)
                    && long.TryParse(TongTienFrom, out var fromNo) && long.TryParse(TongTienTo, out var toNo))
                {
                    hoaDons = hoaDons.Where(d => d.KhachHang.TienNo >= fromNo && d.KhachHang.TienNo <= toNo);
                }
                else
                {
                    if (!string.IsNullOrEmpty(TongTienFrom) && long.TryParse(TongTienFrom, out fromNo))
                    {
                        hoaDons = hoaDons.Where(d => d.KhachHang.TienNo >= fromNo);
                    }

                    if (!string.IsNullOrEmpty(TongTienTo) && long.TryParse(TongTienTo, out toNo))
                    {
                        hoaDons = hoaDons.Where(d => d.KhachHang.TienNo <= toNo);
                    }
                }

                if (NgayLapHoaDonFrom != DateTime.MinValue && NgayLapHoaDonTo != DateTime.MinValue)
                {
                    hoaDons = hoaDons.Where(d => d.NgayLap >= NgayLapHoaDonFrom && d.NgayLap <= NgayLapHoaDonTo);
                }
                else
                {

                    if (NgayLapHoaDonFrom != DateTime.MinValue)
                    {
                        hoaDons = hoaDons.Where(d => d.NgayLap >= NgayLapHoaDonFrom);
                    }

                    if (NgayLapHoaDonTo != DateTime.MinValue)
                    {
                        hoaDons = hoaDons.Where(d => d.NgayLap <= NgayLapHoaDonTo);
                    }
                }

                if (SelectedKhachHang.MaKhachHang != 0)
                {
                    hoaDons = hoaDons.Where(h => h.MaKhachHang == SelectedKhachHang.MaKhachHang);
                }

                if (!string.IsNullOrEmpty(DienThoai))
                {
                    hoaDons = hoaDons.Where(h => h.KhachHang.DienThoai.Contains(DienThoai));
                }

                if (!string.IsNullOrEmpty(DiaChi))
                {
                    hoaDons = hoaDons.Where(h => h.KhachHang.DiaChi.Contains(DiaChi));
                }

                if (!string.IsNullOrEmpty(Email))
                {
                    hoaDons = hoaDons.Where(h => h.KhachHang.Email.Contains(Email));
                }

                if (!string.IsNullOrEmpty(TienNoFrom) && !string.IsNullOrEmpty(TienNoTo)
                    && long.TryParse(TienNoFrom, out var fromTienNo) && long.TryParse(TienNoTo, out var toTienNo))
                {
                    hoaDons = hoaDons.Where(d => d.KhachHang.TienNo >= fromTienNo && d.KhachHang.TienNo <= toTienNo);
                }
                else
                {
                    if (!string.IsNullOrEmpty(TienNoFrom) && long.TryParse(TienNoFrom, out fromNo))
                    {
                        hoaDons = hoaDons.Where(d => d.KhachHang.TienNo >= fromNo);
                    }

                    if (!string.IsNullOrEmpty(TienNoTo) && long.TryParse(TienNoTo, out toNo))
                    {
                        hoaDons = hoaDons.Where(d => d.KhachHang.TienNo <= toNo);
                    }
                }

                if (SelectedSach.MaSach != 0)
                {
                    hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.Sach.MaSach == SelectedSach.MaSach));
                }

                if (!string.IsNullOrEmpty(TheLoai))
                {
                    hoaDons = hoaDons.Where(h => h.DsChiTietHoaDon.Any(ct => ct.Sach.TheLoai.Contains(TheLoai)));
                }

                if (!string.IsNullOrEmpty(TacGia))
                {
                    hoaDons = hoaDons.Where(h => h.DsChiTietHoaDon.Any(ct => ct.Sach.TacGia.Contains(TacGia)));
                }

                if (!string.IsNullOrEmpty(SoLuongTonFrom) && !string.IsNullOrEmpty(SoLuongTonTo)
                    && int.TryParse(SoLuongTonFrom, out var fromSoLuongTon) && int.TryParse(SoLuongTonTo, out var toSoLuongTon))
                {
                    hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.Sach.SoLuongTon >= fromSoLuongTon && ct.Sach.SoLuongTon <= toSoLuongTon));
                }
                else
                {
                    if (!string.IsNullOrEmpty(SoLuongTonFrom) && int.TryParse(SoLuongTonFrom, out fromSoLuongTon))
                    {
                        hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.Sach.SoLuongTon >= fromSoLuongTon));
                    }
                    if (!string.IsNullOrEmpty(SoLuongTonTo) && int.TryParse(SoLuongTonTo, out toSoLuongTon))
                    {
                        hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.Sach.SoLuongTon <= toSoLuongTon));
                    }
                }

                if (!string.IsNullOrEmpty(DonGiaBanFrom) && !string.IsNullOrEmpty(DonGiaBanTo)
                    && int.TryParse(DonGiaBanFrom, out var fromDonGiaBan) && int.TryParse(DonGiaBanTo, out var toDonGiaBan))
                {
                    hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.DonGiaBan >= fromDonGiaBan && ct.DonGiaBan <= toDonGiaBan));
                }
                else
                {
                    if (!string.IsNullOrEmpty(DonGiaBanFrom) && int.TryParse(DonGiaBanFrom, out fromDonGiaBan))
                    {
                        hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.DonGiaBan >= fromDonGiaBan));
                    }
                    if (!string.IsNullOrEmpty(DonGiaBanTo) && int.TryParse(DonGiaBanTo, out toDonGiaBan))
                    {
                        hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.DonGiaBan <= toDonGiaBan));
                    }
                }

                if (!string.IsNullOrEmpty(SoLuongBanFrom) && !string.IsNullOrEmpty(SoLuongBanTo)
                    && int.TryParse(SoLuongBanFrom, out var fromSoLuongBan) && int.TryParse(SoLuongBanTo, out var toSoLuongBan))
                {
                    hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.SoLuongBan >= fromSoLuongBan && ct.SoLuongBan <= toSoLuongBan));
                }
                else
                {
                    if (!string.IsNullOrEmpty(SoLuongBanFrom) && int.TryParse(SoLuongBanFrom, out fromSoLuongBan))
                    {
                        hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.SoLuongBan >= fromSoLuongBan));
                    }
                    if (!string.IsNullOrEmpty(SoLuongBanTo) && int.TryParse(SoLuongBanTo, out toSoLuongBan))
                    {
                        hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.SoLuongBan <= toSoLuongBan));
                    }
                }

                if (!string.IsNullOrEmpty(ThanhTienFrom) && !string.IsNullOrEmpty(ThanhTienTo)
                    && int.TryParse(ThanhTienFrom, out var fromThanhTien) && int.TryParse(ThanhTienTo, out var toThanhTien))
                {
                    hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.ThanhTien >= fromThanhTien && ct.ThanhTien <= toThanhTien));
                }
                else
                {
                    if (!string.IsNullOrEmpty(ThanhTienFrom) && int.TryParse(ThanhTienFrom, out fromThanhTien))
                    {
                        hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.ThanhTien >= fromThanhTien));
                    }
                    if (!string.IsNullOrEmpty(ThanhTienTo) && int.TryParse(ThanhTienTo, out toThanhTien))
                    {
                        hoaDons = hoaDons.Where(d => d.DsChiTietHoaDon.Any(ct => ct.ThanhTien <= toThanhTien));
                    }
                }

                SearchResults = [.. hoaDons];

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
    }
}
