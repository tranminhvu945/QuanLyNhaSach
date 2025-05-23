using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.SachViews;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Windows;

namespace QuanLyNhaSach.ViewModels.SachViewModel
{
    public partial class TraCuuSachViewModel : ObservableObject
    {
        private readonly ISachService _sachService;
        private readonly IKhachHangService _khachHangService;

        public TraCuuSachViewModel(
            ISachService sachService,
            IKhachHangService khachHangService
        )
        {
            _sachService = sachService;
            _khachHangService = khachHangService;

            _ = LoadDataAsync();
        }
        #region binding
        [ObservableProperty]
        private ObservableCollection<Sach> _sachs = [];

        [ObservableProperty]
        private Sach _selectedSach = null!;

        [ObservableProperty]
        private string _maSach = "";

        [ObservableProperty]
        private string _theLoai = "";

        [ObservableProperty]
        private string _tacGia = "";

        [ObservableProperty]
        private string _soLuongTonFrom = "";

        [ObservableProperty]
        private string _soLuongTonTo = "";

        [ObservableProperty]
        private ObservableCollection<KhachHang> _khachHangs = [];

        [ObservableProperty]
        private KhachHang _selectedKhachHang = null!;

        [ObservableProperty]
        private string _tienNoKhachHangFrom = "";

        [ObservableProperty]
        private string _tienNoKhachHangTo = "";

        [ObservableProperty]
        private string _maHoaDonFrom = "";

        [ObservableProperty]
        private string _maHoaDonTo = "";

        [ObservableProperty]
        private DateTime _ngayLapHoaDonFrom = DateTime.MinValue;

        [ObservableProperty]
        private DateTime _ngayLapHoaDonTo = DateTime.Now;

        [ObservableProperty]
        private string _donGiaBanFrom = "";

        [ObservableProperty]
        private string _donGiaBanTo = "";

        [ObservableProperty]
        private string _soLuongBanFrom = "";

        [ObservableProperty]
        private string _soLuongBanTo = "";

        [ObservableProperty]
        private string _thanhTienFrom = "";

        [ObservableProperty]
        private string _thanhTienTo = "";

        [ObservableProperty]
        private string _tongGiaTriHoaDonFrom = "";

        [ObservableProperty]
        private string _tongGiaTriHoaDonTo = "";

        [ObservableProperty]
        private string _maNhapPhieuFrom = "";

        [ObservableProperty]
        private string _maNhapPhieuTo = "";

        [ObservableProperty]
        private DateTime _ngayNhapFrom = DateTime.MinValue;

        [ObservableProperty]
        private DateTime _ngayNhapTo = DateTime.Now;

        [ObservableProperty]
        private string _soLuongNhapFrom = "";

        [ObservableProperty]
        private string _soLuongNhapTo = "";

        public ObservableCollection<Sach> SearchResults = [];
        #endregion

        private async Task LoadDataAsync()
        {
            try
            {
                var listKhachHang = await _khachHangService.GetAllKhachHang();
                var listSach = await _sachService.GetAllSach();

                KhachHangs = [.. listKhachHang];
                Sachs = [.. listSach];

                KhachHangs.Clear();
                Sachs.Clear();

                // Populate the collections
                KhachHangs = new ObservableCollection<KhachHang>(listKhachHang);
                Sachs = new ObservableCollection<Sach>(listSach);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void CloseWindow()
        {
            Application.Current.Windows.OfType<TraCuuSachWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private async Task SearchSach()
        {
            try
            {
                var sachs = await _sachService.GetAllSach();

                if (!string.IsNullOrWhiteSpace(MaSach))
                {
                    sachs = sachs.Where(d =>
                        d.MaSach.ToString().IndexOf(MaSach, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (!string.IsNullOrWhiteSpace(TheLoai))
                {
                    sachs = sachs.Where(d =>
                        !string.IsNullOrEmpty(d.TheLoai) &&
                        d.TheLoai.IndexOf(TheLoai, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (!string.IsNullOrWhiteSpace(TacGia))
                {
                    sachs = sachs.Where(d =>
                        !string.IsNullOrEmpty(d.TacGia) &&
                        d.TacGia.IndexOf(TacGia, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (!string.IsNullOrEmpty(SoLuongTonFrom)
                    && !string.IsNullOrEmpty(SoLuongTonTo)
                    && int.TryParse(SoLuongTonFrom, out var fromQty)
                    && int.TryParse(SoLuongTonTo, out var toQty))
                {
                    sachs = sachs.Where(d => d.SoLuongTon >= fromQty && d.SoLuongTon <= toQty);
                }
                else
                {
                    // Nếu chỉ nhập From
                    if (!string.IsNullOrEmpty(SoLuongTonFrom)
                        && int.TryParse(SoLuongTonFrom, out fromQty))
                    {
                        sachs = sachs.Where(d => d.SoLuongTon >= fromQty);
                    }
                    // Nếu chỉ nhập To
                    if (!string.IsNullOrEmpty(SoLuongTonTo)
                        && int.TryParse(SoLuongTonTo, out toQty))
                    {
                        sachs = sachs.Where(d => d.SoLuongTon <= toQty);
                    }
                }

                if (SelectedKhachHang != null! && SelectedKhachHang.MaKhachHang != 0)
                {
                    sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.HoaDon.MaKhachHang == SelectedKhachHang.MaKhachHang));
                }

                if (!string.IsNullOrEmpty(TienNoKhachHangFrom)
                && !string.IsNullOrEmpty(TienNoKhachHangTo)
                && SelectedKhachHang != null
                && int.TryParse(TienNoKhachHangFrom, out var fromTienNo)
                && int.TryParse(TienNoKhachHangTo, out var toTienNo))
                {
                    sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd =>
                        hd.HoaDon.MaKhachHang == SelectedKhachHang.MaKhachHang &&
                        hd.HoaDon.KhachHang.TienNo >= fromTienNo &&
                        hd.HoaDon.KhachHang.TienNo <= toTienNo));
                }
                else
                {
                    if (!string.IsNullOrEmpty(TienNoKhachHangFrom)
                        && SelectedKhachHang != null
                        && int.TryParse(TienNoKhachHangFrom, out fromTienNo))
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd =>
                            hd.HoaDon.MaKhachHang == SelectedKhachHang.MaKhachHang &&
                            hd.HoaDon.KhachHang.TienNo >= fromTienNo));
                    }
                    if (!string.IsNullOrEmpty(TienNoKhachHangTo)
                        && SelectedKhachHang != null
                        && int.TryParse(TienNoKhachHangTo, out toTienNo))
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd =>
                            hd.HoaDon.MaKhachHang == SelectedKhachHang.MaKhachHang &&
                            hd.HoaDon.KhachHang.TienNo <= toTienNo));
                    }
                }

                if (!string.IsNullOrEmpty(MaHoaDonFrom)
                    && !string.IsNullOrEmpty(MaHoaDonTo)
                    && int.TryParse(MaHoaDonFrom, out var fromMaHD)
                    && int.TryParse(MaHoaDonTo, out var toMaHD))
                {
                    sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.HoaDon.MaHoaDon >= fromMaHD && hd.HoaDon.MaHoaDon <= toMaHD));
                }
                else
                {
                    if (!string.IsNullOrEmpty(MaHoaDonFrom)
                        && int.TryParse(MaHoaDonFrom, out fromMaHD))
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.HoaDon.MaHoaDon >= fromMaHD));
                    }
                    if (!string.IsNullOrEmpty(MaHoaDonTo)
                        && int.TryParse(MaHoaDonTo, out toMaHD))
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.HoaDon.MaHoaDon <= toMaHD));
                    }
                }

                if (NgayLapHoaDonFrom != DateTime.MinValue && NgayLapHoaDonTo != DateTime.MinValue)
                {
                    sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.HoaDon.NgayLap >= NgayLapHoaDonFrom && hd.HoaDon.NgayLap <= NgayLapHoaDonTo));
                }
                else
                {
                    if (NgayLapHoaDonFrom != DateTime.MinValue)
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.HoaDon.NgayLap >= NgayLapHoaDonFrom));
                    }
                    if (NgayLapHoaDonTo != DateTime.MinValue)
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.HoaDon.NgayLap <= NgayLapHoaDonTo));
                    }
                }

                if (!string.IsNullOrEmpty(DonGiaBanFrom)
                    && !string.IsNullOrEmpty(DonGiaBanTo)
                    && decimal.TryParse(DonGiaBanFrom, out var fromDonGia)
                    && decimal.TryParse(DonGiaBanTo, out var toDonGia))
                {
                    sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.DonGiaBan >= fromDonGia && hd.DonGiaBan <= toDonGia));
                }
                else
                {
                    if (!string.IsNullOrEmpty(DonGiaBanFrom)
                        && decimal.TryParse(DonGiaBanFrom, out fromDonGia))
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.DonGiaBan >= fromDonGia));
                    }
                    if (!string.IsNullOrEmpty(DonGiaBanTo)
                        && decimal.TryParse(DonGiaBanTo, out toDonGia))
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.DonGiaBan <= toDonGia));
                    }
                }

                if (!string.IsNullOrEmpty(SoLuongBanFrom)
                    && !string.IsNullOrEmpty(SoLuongBanTo)
                    && int.TryParse(SoLuongBanFrom, out var fromSoLuongBan)
                    && int.TryParse(SoLuongBanTo, out var toSoLuongBan))
                {
                    sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.SoLuongBan >= fromSoLuongBan && hd.SoLuongBan <= toSoLuongBan));
                }
                else
                {
                    if (!string.IsNullOrEmpty(SoLuongBanFrom)
                        && int.TryParse(SoLuongBanFrom, out fromSoLuongBan))
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.SoLuongBan >= fromSoLuongBan));
                    }
                    if (!string.IsNullOrEmpty(SoLuongBanTo)
                        && int.TryParse(SoLuongBanTo, out toSoLuongBan))
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.SoLuongBan <= toSoLuongBan));
                    }
                }

                if (!string.IsNullOrEmpty(ThanhTienFrom)
                    && !string.IsNullOrEmpty(ThanhTienTo)
                    && decimal.TryParse(ThanhTienFrom, out var fromThanhTien)
                    && decimal.TryParse(ThanhTienTo, out var toThanhTien))
                {
                    sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.ThanhTien >= fromThanhTien && hd.ThanhTien <= toThanhTien));
                }
                else
                {
                    if (!string.IsNullOrEmpty(ThanhTienFrom)
                        && decimal.TryParse(ThanhTienFrom, out fromThanhTien))
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.ThanhTien >= fromThanhTien));
                    }
                    if (!string.IsNullOrEmpty(ThanhTienTo)
                        && decimal.TryParse(ThanhTienTo, out toThanhTien))
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.ThanhTien <= toThanhTien));
                    }
                }

                if (!string.IsNullOrEmpty(TongGiaTriHoaDonFrom)
                    && !string.IsNullOrEmpty(TongGiaTriHoaDonTo)
                    && decimal.TryParse(TongGiaTriHoaDonFrom, out var fromTongGiaTri)
                    && decimal.TryParse(TongGiaTriHoaDonTo, out var toTongGiaTri))
                {
                    sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.HoaDon.TongTien >= fromTongGiaTri && hd.HoaDon.TongTien <= toTongGiaTri));
                }
                else
                {
                    if (!string.IsNullOrEmpty(TongGiaTriHoaDonFrom)
                        && decimal.TryParse(TongGiaTriHoaDonFrom, out fromTongGiaTri))
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.HoaDon.TongTien >= fromTongGiaTri));
                    }
                    if (!string.IsNullOrEmpty(TongGiaTriHoaDonTo)
                        && decimal.TryParse(TongGiaTriHoaDonTo, out toTongGiaTri))
                    {
                        sachs = sachs.Where(d => d.DsChiTietHoaDon.Any(hd => hd.HoaDon.TongTien <= toTongGiaTri));
                    }
                }

                if (!string.IsNullOrEmpty(MaNhapPhieuFrom)
                    && !string.IsNullOrEmpty(MaNhapPhieuTo)
                    && int.TryParse(MaNhapPhieuFrom, out var fromMaNhapPhieu)
                    && int.TryParse(MaNhapPhieuTo, out var toMaNhapPhieu))
                {
                    sachs = sachs.Where(d => d.DsChiTietPhieuNhap.Any(hd => hd.PhieuNhapSach.MaPhieuNhapSach >= fromMaNhapPhieu
                                                                        && hd.PhieuNhapSach.MaPhieuNhapSach <= toMaNhapPhieu));

                }
                else
                {
                    if (!string.IsNullOrEmpty(MaNhapPhieuFrom)
                        && int.TryParse(MaNhapPhieuFrom, out fromMaNhapPhieu))
                    {
                        sachs = sachs.Where(d => d.DsChiTietPhieuNhap.Any(hd => hd.PhieuNhapSach.MaPhieuNhapSach >= fromMaNhapPhieu));
                    }
                    if (!string.IsNullOrEmpty(MaNhapPhieuTo)
                        && int.TryParse(MaNhapPhieuTo, out toMaNhapPhieu))
                    {
                        sachs = sachs.Where(d => d.DsChiTietPhieuNhap.Any(hd => hd.PhieuNhapSach.MaPhieuNhapSach <= toMaNhapPhieu));
                    }
                }

                if (NgayNhapFrom != DateTime.MinValue && NgayNhapTo != DateTime.MinValue)
                {
                    sachs = sachs.Where(d => d.DsChiTietPhieuNhap.Any(hd => hd.PhieuNhapSach.NgayNhap >= NgayNhapFrom && hd.PhieuNhapSach.NgayNhap <= NgayNhapTo));
                }
                else
                {
                    if (NgayNhapFrom != DateTime.MinValue)
                    {
                        sachs = sachs.Where(d => d.DsChiTietPhieuNhap.Any(hd => hd.PhieuNhapSach.NgayNhap >= NgayNhapFrom));
                    }
                    if (NgayNhapTo != DateTime.MinValue)
                    {
                        sachs = sachs.Where(d => d.DsChiTietPhieuNhap.Any(hd => hd.PhieuNhapSach.NgayNhap <= NgayNhapTo));
                    }
                }

                if (!string.IsNullOrEmpty(SoLuongNhapFrom)
                    && !string.IsNullOrEmpty(SoLuongNhapTo)
                    && int.TryParse(SoLuongNhapFrom, out var fromSoLuongNhap)
                    && int.TryParse(SoLuongNhapTo, out var toSoLuongNhap))
                {
                    sachs = sachs.Where(d => d.DsChiTietPhieuNhap.Any(hd => hd.SoLuongNhap >= fromSoLuongNhap && hd.SoLuongNhap <= toSoLuongNhap));
                }
                else
                {
                    if (!string.IsNullOrEmpty(SoLuongNhapFrom)
                        && int.TryParse(SoLuongNhapFrom, out fromSoLuongNhap))
                    {
                        sachs = sachs.Where(d => d.DsChiTietPhieuNhap.Any(hd => hd.SoLuongNhap >= fromSoLuongNhap));
                    }
                    if (!string.IsNullOrEmpty(SoLuongNhapTo)
                        && int.TryParse(SoLuongNhapTo, out toSoLuongNhap))
                    {
                        sachs = sachs.Where(d => d.DsChiTietPhieuNhap.Any(hd => hd.SoLuongNhap <= toSoLuongNhap));
                    }
                }

                SearchResults = [.. sachs];

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

        private void ApplySearchResults()
        {
            WeakReferenceMessenger.Default.Send(new SearchCompletedMessage<Sach>(SearchResults));
            CloseWindow();
        }
    }
}
