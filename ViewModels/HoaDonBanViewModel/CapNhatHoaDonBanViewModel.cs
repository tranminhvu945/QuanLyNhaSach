using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Models.dto;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.HoaDonBanViews;
using QuanLyNhaSach.Views.KhachHangHoaDonViews;
using QuanLyNhaSach.Views.KhachHangViews;
using QuanLyNhaSach.Views.SachViews;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace QuanLyNhaSach.ViewModels.HoaDonBanViewModel
{
    public partial class CapNhatHoaDonBanViewModel : ObservableObject, IRecipient<SelectedIdMessage>
    {
        private readonly IHoaDonService _hoaDonService;
        private readonly IChiTietHoaDonService _hoaDonChiTietService;
        private readonly ISachService _sachService;
        private readonly IKhachHangService _khachHangService;
        private readonly IThamSoService _thamsoService;
        private readonly IServiceProvider _serviceProvider;
        private int _hoaDonID;

        public CapNhatHoaDonBanViewModel(
            IHoaDonService hoaDonService,
            IChiTietHoaDonService hoaDonChiTietService,
            ISachService sachService,
            IKhachHangService khachHangService,
            IServiceProvider serviceProvider,
            IThamSoService thamsoService)
        {
            _hoaDonService = hoaDonService;
            _hoaDonChiTietService = hoaDonChiTietService;
            _sachService = sachService;
            _khachHangService = khachHangService;
            _serviceProvider = serviceProvider;
            _thamsoService = thamsoService;

            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        public void Receive(SelectedIdMessage message)
        {
            _hoaDonID = message.Value;
            _ = LoadDataAsync();
        }

        #region binding properties
        [ObservableProperty]
        private ObservableCollection<Sach> _danhSachSach = [];

        [ObservableProperty]
        private ObservableCollection<Sach> _danhSachSachDaChon = [];

        [ObservableProperty]
        private string _maHoaDon = string.Empty;

        [ObservableProperty]
        private DateTime _ngayLap = DateTime.Now;

        [ObservableProperty]
        private ObservableCollection<KhachHang> _khachHangs = [];

        [ObservableProperty]
        private KhachHang _selectedKhachHang = null!;

        [ObservableProperty]
        private string _dienThoai = "";

        [ObservableProperty]
        private string _soLuongTonToiThieuSauBan = "XXX";

        [ObservableProperty]
        private string _noToiDa = "XXX";

        [ObservableProperty]
        private long _tienNo = 0;

        [ObservableProperty]
        private long _tongTien = 0;

        [ObservableProperty]
        private ObservableCollection<DisplaySachHoaDon> _danhSachSachHoaDon = [];

        [ObservableProperty]
        private DisplaySachHoaDon _selectedSachHoaDon = null!;

        [ObservableProperty]
        private Sach _selectedSach = null!;
        #endregion

        partial void OnDienThoaiChanged(string oldValue, string newValue)
        {
            if (!string.IsNullOrWhiteSpace(newValue) && (newValue.Length == 10 || newValue.Length == 11))
            {
                _ = TimKhachHangTheoDienThoai();
            }
            else
            {
                SelectedKhachHang = null!;
                TienNo = 0;
            }
        }

        [RelayCommand]
        public async Task TimKhachHangTheoDienThoai()
        {
            if (string.IsNullOrWhiteSpace(DienThoai))
            {
                SelectedKhachHang = null!;
                TienNo = 0;
                return;
            }

            var khachHang = KhachHangs.FirstOrDefault(kh => kh.DienThoai == DienThoai);

            if (khachHang != null!)
            {
                SelectedKhachHang = khachHang;
                TienNo = khachHang.TienNo;
                // Nếu muốn cập nhật thêm UI khác khi chọn khách hàng, gọi ở đây
            }
            else
            {
                SelectedKhachHang = null!;
                TienNo = 0;
                MessageBox.Show("Không tìm thấy khách hàng với số điện thoại này", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        partial void OnSelectedKhachHangChanged(KhachHang value)
        {
            if (value != null)
            {
                DienThoai = value.DienThoai ?? "";
                _ = UpdateTienNoAndQuyDinhAsync(value);
            }
            else
            {
                DienThoai = "";
                TienNo = 0;
            }
        }

        private async Task UpdateTienNoAndQuyDinhAsync(KhachHang value)
        {
            try
            {
                var thamSo = await _thamsoService.GetThamSo();

                // Hiển thị tiền nợ tối đa hoặc "XXX"
                NoToiDa = thamSo.QuyDinhTienNoToiDa ? thamSo.TienNoToiDa.ToString("N0") : "XXX";

                // Hiển thị số lượng tồn tối thiểu hoặc "XXX"
                SoLuongTonToiThieuSauBan = thamSo.QuyDinhSoLuongTonToiThieu ? thamSo.SoLuongTonToiThieu.ToString() : "XXX";

                // Cập nhật tiền nợ khách hàng bình thường
                TienNo = value.TienNo;

                if (thamSo.QuyDinhTienNoToiDa && TienNo > thamSo.TienNoToiDa)
                {
                    MessageBox.Show($"Khách hàng {value.TenKhachHang} đã vượt quá tiền nợ tối đa.",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật số tiền nợ và quy định: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task LoadDataAsync()
        {
            try
            {
                MaHoaDon = _hoaDonID.ToString();

                var existingHoaDon = await _hoaDonService.GetHoaDonById(_hoaDonID);
                if (existingHoaDon == null)
                {
                    MessageBox.Show("Không tìm thấy hoá đơn để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                NgayLap = existingHoaDon.NgayLap;
                TongTien = existingHoaDon.TongTien;

                var sachList = await _sachService.GetAllSach();
                DanhSachSach = new ObservableCollection<Sach>(sachList);

                var listKhachHang = await _khachHangService.GetAllKhachHang();
                var sortedListKhachHang = listKhachHang.OrderBy(kh => kh.TenKhachHang).ToList();
                KhachHangs = new ObservableCollection<KhachHang>(sortedListKhachHang);

                SelectedKhachHang = KhachHangs.FirstOrDefault(kh => kh.MaKhachHang == existingHoaDon.MaKhachHang);

                var thamSo = await _thamsoService.GetThamSo();
                NoToiDa = thamSo.QuyDinhTienNoToiDa ? thamSo.TienNoToiDa.ToString("N0") : "XXX";
                SoLuongTonToiThieuSauBan = thamSo.QuyDinhSoLuongTonToiThieu ? thamSo.SoLuongTonToiThieu.ToString() : "XXX";

                DanhSachSachHoaDon.Clear();

                var listChiTietHoaDon = await _hoaDonChiTietService.GetChiTietHoaDonByHoaDonId(_hoaDonID);
                foreach (var chiTiet in listChiTietHoaDon)
                {
                    var sach = sachList.FirstOrDefault(s => s.MaSach == chiTiet.MaSach);
                    if (sach != null)
                    {
                        var item = new DisplaySachHoaDon(DanhSachSach)
                        {
                            SelectedSach = sach,
                            SoLuongBan = chiTiet.SoLuongBan,
                            DonGiaBan = chiTiet.DonGiaBan,
                            SoLuongTonBanDau = sach.SoLuongTon + chiTiet.SoLuongBan  // lưu số tồn ban đầu tại thời điểm này
                        };
                        item.ThanhTienChanged += (s, e) => CalculateTongTien();

                        DanhSachSachHoaDon.Add(item);
                    }
                }

                CalculateTongTien();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Close()
        {
            WeakReferenceMessenger.Default.Send(new DataReloadMessage());
            Application.Current.Windows.OfType<CapNhatHoaDonBanWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private void CapNhatHoaDon()
        {
            // Use Task.Run to execute the async method without awaiting
            _ = CapNhatHoaDonAsync();
        }

        private async Task CapNhatHoaDonAsync()
        {
            try
            {
                if (DanhSachSachHoaDon.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một đầu sách", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var thamSo = await _thamsoService.GetThamSo();

                foreach (var item in DanhSachSachHoaDon)
                {
                    if (item.SoLuongBan <= 0)
                    {
                        MessageBox.Show($"Số lượng bán của {item.SelectedSach.TenSach} phải lớn hơn 0",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // So sánh với tồn kho gốc trước khi bán
                    if (item.SoLuongBan > item.SoLuongTonTruocKhiXuat)
                    {
                        MessageBox.Show($"Số lượng bán của {item.SelectedSach.TenSach} không được vượt quá số lượng tồn ban đầu ({item.SoLuongTonTruocKhiXuat})",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Kiểm tra quy định số lượng tồn tối thiểu sau bán (nếu áp dụng)
                    if (thamSo.QuyDinhSoLuongTonToiThieu)
                    {
                        int soLuongTonSauBan = item.SoLuongTonBanDau - item.SoLuongBan;
                        if (soLuongTonSauBan < thamSo.SoLuongTonToiThieu)
                        {
                            MessageBox.Show($"Số lượng tồn của sách '{item.SelectedSach.TenSach}' sau khi bán phải lớn hơn hoặc bằng {thamSo.SoLuongTonToiThieu}. Hiện tại còn {soLuongTonSauBan}.",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    if (item.DonGiaBan <= 0)
                    {
                        MessageBox.Show($"Đơn giá bán của {item.SelectedSach.TenSach} phải lớn hơn 0",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                // Cập nhật tổng tiền hóa đơn trước
                CalculateTongTien();

                // Kiểm tra quy định tiền nợ tối đa
                if (thamSo.QuyDinhTienNoToiDa)
                {
                    var khachHangHienTai = await _khachHangService.GetKhachHangById(SelectedKhachHang.MaKhachHang);
                    var existingHoaDon = await _hoaDonService.GetHoaDonById(_hoaDonID);
                    
                    long tienNoDuKien = khachHangHienTai.TienNo + TongTien - existingHoaDon.TongTien;

                    if (tienNoDuKien > thamSo.TienNoToiDa)
                    {
                        MessageBox.Show($"Khách hàng đã vượt quá tiền nợ tối đa ({thamSo.TienNoToiDa}). Không thể cập nhật hoá đơn.",
                                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var hoaDon = await _hoaDonService.GetHoaDonById(_hoaDonID);
                long oldTongTriGia = hoaDon.TongTien;

                hoaDon.MaKhachHang = SelectedKhachHang.MaKhachHang;
                hoaDon.NgayLap = NgayLap;
                hoaDon.TongTien = TongTien;

                await _hoaDonService.UpdateHoaDon(hoaDon);

                var existingChiTiet = await _hoaDonChiTietService.GetChiTietHoaDonByHoaDonId(_hoaDonID);

                foreach (var chiTiet in existingChiTiet)
                {
                    var Sach = await _sachService.GetSachById(chiTiet.MaSach);
                    Sach.SoLuongTon += chiTiet.SoLuongBan;
                    await _sachService.UpdateSach(Sach);
                    await _hoaDonChiTietService.DeleteChiTietHoaDon(chiTiet);
                }

                foreach (var item in DanhSachSachHoaDon)
                {
                    var chiTiethoaDon = new ChiTietHoaDon
                    {
                        MaHoaDon = _hoaDonID,
                        MaSach = item.SelectedSach.MaSach,
                        SoLuongBan = item.SoLuongBan,
                        DonGiaBan = item.DonGiaBan,
                        ThanhTien = item.ThanhTien
                    };

                    await _hoaDonChiTietService.AddChiTietHoaDon(chiTiethoaDon);

                    var Sach = await _sachService.GetSachById(item.SelectedSach.MaSach);
                    Sach.SoLuongTon -= item.SoLuongBan;
                    await _sachService.UpdateSach(Sach);
                }

                var KhachHang = await _khachHangService.GetKhachHangById(SelectedKhachHang.MaKhachHang);
                KhachHang.TienNo = KhachHang.TienNo - oldTongTriGia + TongTien;

                await _khachHangService.UpdateKhachHang(KhachHang);

                MessageBox.Show("Cập nhật phiếu bán thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi cập nhật phiếu bán: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Cập nhật danh sách available cho tất cả các dòng
        [RelayCommand]
        private void UpdateAvailableLists()
        {
            var selectedIds = DanhSachSachHoaDon.Select(r => r.SelectedSach.MaSach).ToHashSet();
            foreach (var row in DanhSachSachHoaDon)
            {
                var own = row.SelectedSach;
                var available = DanhSachSach
                    .Where(m => !selectedIds.Contains(m.MaSach))
                    .Concat(new[] { own })
                    .OrderBy(s => s.TenSach, StringComparer.CurrentCultureIgnoreCase)
                    .ToList();
                row.DanhSachSach = new ObservableCollection<Sach>(available);
            }
        }

        [RelayCommand]
        private void ThongTinKhachHang()
        {
            SelectedKhachHang = null!;

            var thongTinKhachHangWindow = _serviceProvider.GetRequiredService<KhachHangHoaDonWindow>();
            thongTinKhachHangWindow.Show();
        }

        [RelayCommand]
        private void ThemSach()
        {
            var available = new List<Sach>(DanhSachSach);
            if (!available.Any())
            {
                MessageBox.Show("Đã nhập đầy đủ đầu sách", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var newItem = new DisplaySachHoaDon(available);
            newItem.ThanhTienChanged += (s, e) => CalculateTongTien();
            DanhSachSachHoaDon.Add(newItem);

            // Đăng ký lắng nghe khi SelectedSach thay đổi
            newItem.SelectedSachChanged += (sender, e) =>
            {
                if (e.OldSach != null)
                {
                    DanhSachSach.Add(e.OldSach);
                    DanhSachSachDaChon.Remove(e.OldSach);
                }
                if (e.NewSach != null)
                {
                    DanhSachSach.Remove(e.NewSach);
                    DanhSachSachDaChon.Add(e.NewSach);
                }
                UpdateAvailableLists();
            };

            // Lúc mới tạo dòng, cũng cần thêm sách đầu tiên vào danh sách đã chọn
            if (newItem.SelectedSach != null)
            {
                DanhSachSach.Remove(newItem.SelectedSach);
                DanhSachSachDaChon.Add(newItem.SelectedSach);
            }

            UpdateAvailableLists();
            CalculateTongTien();
        }

        [RelayCommand]
        private void XoaSach()
        {
            if (SelectedSachHoaDon == null!)
            {
                MessageBox.Show("Vui lòng chọn đầu sách để xóa", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DanhSachSachHoaDon.Count > 0)
            {
                if (SelectedSachHoaDon.SelectedSach != null)
                {
                    DanhSachSach.Add(SelectedSachHoaDon.SelectedSach);
                    DanhSachSachDaChon.Remove(SelectedSachHoaDon.SelectedSach);
                }

                DanhSachSachHoaDon.Remove(SelectedSachHoaDon);

                UpdateAvailableLists();
                CalculateTongTien();
            }
            else
            {
                MessageBox.Show("Không có đầu sách nào để xóa", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void BoChonSach()
        {
            SelectedSachHoaDon = null!;
        }

        [RelayCommand]
        private void CalculateTongTien()
        {
            TongTien = DanhSachSachHoaDon.Sum(item => item.ThanhTien);
            OnPropertyChanged(nameof(TongTien));
        }

        [RelayCommand]
        private async Task UpdateTien()
        {
            try
            {
                var thamSo = await _thamsoService.GetThamSo();
                NoToiDa = thamSo.TienNoToiDa.ToString();
                if (SelectedKhachHang != null!)
                    TienNo = SelectedKhachHang.TienNo;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error updating TienNo");
            }
        }
    }
}
