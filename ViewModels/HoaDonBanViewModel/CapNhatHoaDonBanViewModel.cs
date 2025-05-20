using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Models.dto;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.HoaDonBanViews;
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

        // Load data 
        private async Task LoadDataAsync()
        {
            try
            {
                MaHoaDon = _hoaDonID.ToString();
                var existingHoaDon = await _hoaDonService.GetHoaDonById(_hoaDonID);
                SelectedKhachHang = existingHoaDon.KhachHang;
                NgayLap = existingHoaDon.NgayLap;
                TongTien = existingHoaDon.TongTien;

                DanhSachSach = [.. (await _sachService.GetAllSach())];
                var listKhachHang = await _khachHangService.GetAllKhachHang();
                // Sắp xếp theo tên khách hàng (TenKhachHang)
                var sortedListKhachHang = listKhachHang.OrderBy(kh => kh.TenKhachHang).ToList();

                KhachHangs = new ObservableCollection<KhachHang>(sortedListKhachHang);


                DanhSachSachHoaDon.Clear();

                var listChiTietHoaDon = await _hoaDonChiTietService.GetChiTietHoaDonByHoaDonId(_hoaDonID);
                foreach (var chiTiet in listChiTietHoaDon)
                {
                    DanhSachSachHoaDon.Add(new DisplaySachHoaDon(DanhSachSach)
                    {
                        SelectedSach = await _sachService.GetSachById(chiTiet.MaSach),
                        SoLuongBan = chiTiet.SoLuongBan,
                        DonGiaBan = chiTiet.DonGiaBan,
                    });
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi tải dữ liệu: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
        private long _noToiDa = 0;

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

        // Methods
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

                foreach (var item in DanhSachSachHoaDon)
                {
                    if (item.SoLuongBan <= 0)
                    {
                        MessageBox.Show($"Số lượng bán của {item.SelectedSach.TenSach} phải lớn hơn 0",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (item.SoLuongBan > item.SoLuongTon)
                    {
                        MessageBox.Show($"Số lượng bán của {item.SelectedSach.TenSach} không được vượt quá số lượng tồn",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (item.DonGiaBan <= 0)
                    {
                        MessageBox.Show($"Đơn giá bán của {item.SelectedSach.TenSach} phải lớn hơn 0",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private async Task SearchSach()
        {
            SelectedSach = null!;

            var traCuuSachWindow = _serviceProvider.GetRequiredService<TraCuuSachWindow>();
            traCuuSachWindow.Show();
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
