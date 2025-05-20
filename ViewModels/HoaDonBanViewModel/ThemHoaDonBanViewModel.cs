using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Models.dto;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.HoaDonBanViews;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Views.KhachHangViews;
using QuanLyNhaSach.Views.SachViews;
using Microsoft.Extensions.DependencyInjection;

namespace QuanLyNhaSach.ViewModels.HoaDonBanViewModel
{
    public partial class ThemHoaDonBanViewModel : ObservableObject,
        IRecipient<DataReloadMessage>
    {
        private readonly IHoaDonService _hoaDonService;
        private readonly IChiTietHoaDonService _hoaDonChiTietService;
        private readonly ISachService _sachService;
        private readonly IKhachHangService _khachHangService;
        private readonly IThamSoService _thamsoService;
        private readonly IServiceProvider _serviceProvider;

        public ThemHoaDonBanViewModel(
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
            _ = LoadDataAsync();
        }

        public void Receive(DataReloadMessage message)
        {
            _ = LoadDataAsync();
        }

        #region Bindings Properties
        private List<Sach> _danhSachSach = [];
        private List<Sach> _danhSachSachDaChon = [];

        [ObservableProperty]
        private string _maHoaDon = string.Empty;

        [ObservableProperty]
        private DateTime _ngayLap = DateTime.Now;

        [ObservableProperty]
        private ObservableCollection<KhachHang> _khachHangs = [];

        [ObservableProperty]
        private KhachHang _selectedKhachHang = null!;

        [ObservableProperty]
        private string _quyDinhTienNoToiDa = "";

        [ObservableProperty]
        private long _noToiDa = 0;

        [ObservableProperty]
        private bool _isNoToiDaVisible;

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

        // Load data 
        private async Task LoadDataAsync()
        {
            _danhSachSach = new List<Sach>(await _sachService.GetAllSach());
            var listKhachHang = await _khachHangService.GetAllKhachHang();

            var sortedListKhachHang = listKhachHang.OrderBy(kh => kh.TenKhachHang).ToList();
            KhachHangs = new ObservableCollection<KhachHang>(sortedListKhachHang);

            if (KhachHangs.Count > 0)
            {
                var thamSo = await _thamsoService.GetThamSo();
                QuyDinhTienNoToiDa = thamSo.QuyDinhTienNoToiDa ? "Đang áp dụng" : "Không áp dụng";
                IsNoToiDaVisible = thamSo.QuyDinhTienNoToiDa;

                SelectedKhachHang = KhachHangs.First();

                if (IsNoToiDaVisible)
                    NoToiDa = thamSo.TienNoToiDa;
                else
                    NoToiDa = 0; // hoặc null nếu bạn dùng nullable

                TienNo = SelectedKhachHang.TienNo;
            }
        }

        partial void OnSelectedKhachHangChanged(KhachHang oldValue, KhachHang newValue)
        {
            if (newValue != null)
            {
                _ = UpdateTienNoAndQuyDinhAsync(newValue);
            }
        }

        private async Task UpdateTienNoAndQuyDinhAsync(KhachHang value)
        {
            var thamSo = await _thamsoService.GetThamSo();
            QuyDinhTienNoToiDa = thamSo.QuyDinhTienNoToiDa ? "Đang áp dụng" : "Không áp dụng";
            IsNoToiDaVisible = thamSo.QuyDinhTienNoToiDa;

            if (IsNoToiDaVisible)
            {
                NoToiDa = thamSo.TienNoToiDa;
                if (value.TienNo > NoToiDa)
                {
                    MessageBox.Show($"Khách hàng {value.TenKhachHang} đã vượt quá tiền nợ tối đa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                NoToiDa = 0;
            }

            TienNo = value.TienNo;
        }

        // Methods for commands
        [RelayCommand]
        private void Close()
        {
            WeakReferenceMessenger.Default.Send(new DataReloadMessage());
            Application.Current.Windows.OfType<ThemHoaDonBanWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private void LapHoaDon()
        {
            // Use Task.Run to execute the async method without awaiting
            _ = LapHoaDonAsync();
        }

        private async Task LapHoaDonAsync()
        {
            try
            {
                if (SelectedKhachHang == null)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (DanhSachSachHoaDon.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một đầu sách", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validate quantities and prices
                foreach (var item in DanhSachSachHoaDon)
                {
                    if (item.SoLuongBan <= 0)
                    {
                        MessageBox.Show($"Số lượng bán cho {item.SelectedSach.TenSach} phải lớn hơn 0",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (item.SoLuongBan > item.SoLuongTon)
                    {
                        MessageBox.Show($"Số lượng bán cho {item.SelectedSach.TenSach} không được vượt quá số lượng tồn ({item.SoLuongTon})",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (item.DonGiaBan <= 0)
                    {
                        MessageBox.Show($"Đơn giá bán cho {item.SelectedSach.TenSach} phải lớn hơn 0",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                // Cập nhật tổng tiền trước khi kiểm tra
                CalculateTongTien();

                // Kiểm tra quy định tiền nợ tối đa
                var thamSo = await _thamsoService.GetThamSo();
                if (thamSo.QuyDinhTienNoToiDa)
                {
                    long tienNoDuKien = SelectedKhachHang.TienNo + TongTien;

                    if (tienNoDuKien > thamSo.TienNoToiDa)
                    {
                        MessageBox.Show($"Khách hàng đã vượt quá tiền nợ tối đa ({thamSo.TienNoToiDa}). Không thể lập hoá đơn.",
                                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return; // Ngăn không lập hoá đơn
                    }
                }

                if (string.IsNullOrEmpty(MaHoaDon))
                {
                    int newId = await _hoaDonService.GenerateAvailableId();
                    MaHoaDon = newId.ToString();
                }

                var HoaDon = new HoaDon
                {
                    MaHoaDon = int.Parse(MaHoaDon),
                    MaKhachHang = SelectedKhachHang.MaKhachHang,
                    NgayLap = NgayLap,
                    TongTien = TongTien
                };

                await _hoaDonService.AddHoaDon(HoaDon);

                foreach (var item in DanhSachSachHoaDon)
                {
                    var chiTiet = new ChiTietHoaDon
                    {
                        MaHoaDon = HoaDon.MaHoaDon,
                        MaSach = item.SelectedSach.MaSach,
                        SoLuongBan = item.SoLuongBan,
                        DonGiaBan = item.DonGiaBan,
                        ThanhTien = item.ThanhTien
                    };

                    await _hoaDonChiTietService.AddChiTietHoaDon(chiTiet);

                    var Sach = await _sachService.GetSachById(item.SelectedSach.MaSach);
                    Sach.SoLuongTon -= item.SoLuongBan;
                    await _sachService.UpdateSach(Sach);
                }

                // Lấy lại khách hàng từ database để đảm bảo dữ liệu mới nhất
                var khachHangHienTai = await _khachHangService.GetKhachHangById(SelectedKhachHang.MaKhachHang);

                // Cộng thêm tổng tiền hóa đơn mới vào tiền nợ
                khachHangHienTai.TienNo += TongTien;

                await _khachHangService.UpdateKhachHang(khachHangHienTai);

                // Cập nhật lại SelectedKhachHang để giao diện hiển thị chính xác
                SelectedKhachHang = khachHangHienTai;

                MessageBox.Show($"Lập hoá đơn thành công. Mã hoá đơn: {MaHoaDon}",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi lập hoá đơn: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void UpdateAvailableLists()
        {
            var selectedIds = DanhSachSachHoaDon.Select(r => r.SelectedSach.MaSach).ToHashSet();
            foreach (var row in DanhSachSachHoaDon)
            {
                var own = row.SelectedSach;
                var available = _danhSachSach
                    .Where(m => !selectedIds.Contains(m.MaSach))
                    .Concat(new[] { own })
                    .OrderBy(s => s.TenSach, StringComparer.CurrentCultureIgnoreCase)
                    .ToList();
                row.DanhSachSach = new ObservableCollection<Sach>(available);
            }
        }

        [RelayCommand]
        private void AddKhachHang()
        {
            SelectedKhachHang = null!;

            var addKhachHangtWindow = _serviceProvider.GetRequiredService<ThemKhachHangWindow>();
            addKhachHangtWindow.Show();
        }

        [RelayCommand]
        private void ThemSach()
        {
            var available = new List<Sach>(_danhSachSach);
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
                    _danhSachSach.Add(e.OldSach);
                    _danhSachSachDaChon.Remove(e.OldSach);
                }
                if (e.NewSach != null)
                {
                    _danhSachSach.Remove(e.NewSach);
                    _danhSachSachDaChon.Add(e.NewSach);
                }
                UpdateAvailableLists();
            };

            // Lúc mới tạo dòng, cũng cần thêm sách đầu tiên vào danh sách đã chọn
            if (newItem.SelectedSach != null)
            {
                _danhSachSach.Remove(newItem.SelectedSach);
                _danhSachSachDaChon.Add(newItem.SelectedSach);
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
                    _danhSachSach.Add(SelectedSachHoaDon.SelectedSach);
                    _danhSachSachDaChon.Remove(SelectedSachHoaDon.SelectedSach);
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
            OnPropertyChanged();
        }

        [RelayCommand]
        private async Task UpdateTien()
        {
            try
            {
                NoToiDa = (await _thamsoService.GetThamSo()).TienNoToiDa;
                TienNo = SelectedKhachHang.TienNo;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Please god don't go in here");
            }
        }
    }
}
