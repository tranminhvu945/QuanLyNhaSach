using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Models.dto;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views;

namespace QuanLyNhaSach.ViewModels.PhieuNhapSachViewModel
{
    public partial class LapPhieuNhapSachViewModel : ObservableObject
    {
        // Services
        private readonly IPhieuNhapSachService _phieuNhapSachService;
        private readonly IChiTietPhieuNhapService _phieuNhapSachChiTietService;
        private readonly ISachService _sachService;
        private readonly IThamSoService _thamSoService;

        public LapPhieuNhapSachViewModel(
             IPhieuNhapSachService phieuNhapSachService,
             IChiTietPhieuNhapService phieuNhapSachChiTietService,
             ISachService sachService,
             IThamSoService thamSoService)
        {
            _phieuNhapSachService = phieuNhapSachService;
            _sachService = sachService;
            _phieuNhapSachChiTietService = phieuNhapSachChiTietService;
            _thamSoService = thamSoService;

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _danhSachSach = new List<Sach>(await _sachService.GetAllSach());

            var thamso = await _thamSoService.GetThamSo();
            SoLuongTonToiDa = thamso.SoLuongTonToiDa;
            if (thamso.QuyDinhSoLuongTonToiDa == true)
                NoiDung01 = "Đang áp dụng";
            else
                NoiDung01 = "Không áp dụng";

            SoLuongNhapToiThieu = thamso.SoLuongNhapToiThieu;
            if (thamso.QuyDinhSoLuongNhapToiThieu == true)
                NoiDung02 = "Đang áp dụng";
            else
                NoiDung02 = "Không áp dụng";
        }

        #region Bindings Properties
        private List<Sach> _danhSachSach = [];
        private List<Sach> _danhSachSachDaChon = [];

        // Properties for binding
        [ObservableProperty]
        private string _maPhieuNhapSach = string.Empty;
        [ObservableProperty]
        private ObservableCollection<DisplayDauSachPhieuNhap> _danhSachDauSachPhieuNhap = [];
        [ObservableProperty]
        private DisplayDauSachPhieuNhap _selectedDauSachPhieuNhap = null!;
        [ObservableProperty]
        private ObservableCollection<Sach> _saches = [];
        [ObservableProperty]
        private Sach _selectedSach = null!;
        [ObservableProperty]
        private DateTime _ngayNhap = DateTime.Now;
        [ObservableProperty]
        private int _soLuongTonToiDa = 0;
        [ObservableProperty]
        private int _soLuongNhapToiThieu = 0;
        [ObservableProperty]
        private string _noiDung01 = string.Empty;
        [ObservableProperty]
        private string _noiDung02 = string.Empty;
        #endregion

        #region RelayCommands 
        [RelayCommand]
        private void CloseWindow()
        {
            WeakReferenceMessenger.Default.Send(new DataReloadMessage());
            Application.Current.Windows.OfType<LapPhieuNhapSachWindow>().FirstOrDefault()?.Close();
        }
        [RelayCommand]
        private async Task LapPhieuNhapSach()
        {
            try
            {
                if (DanhSachDauSachPhieuNhap.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một đầu sách", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Validate quantities and prices
                foreach (var item in DanhSachDauSachPhieuNhap)
                {
                    if (item.SoLuongNhap <= 0)
                    {
                        MessageBox.Show($"Số lượng nhập cho {item.SelectedSach.TenSach} phải lớn hơn 0",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    //if (item.SoLuongNhap > item.SoLuongTon)
                    //{
                    //    MessageBox.Show($"Số lượng xuất cho {item.SelectedSach.TenSach} không được vượt quá số lượng tồn ({item.SoLuongTon})",
                    //        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    //    return;
                    //}
                }

                if (string.IsNullOrEmpty(MaPhieuNhapSach))
                {
                    int newId = await _phieuNhapSachService.GenerateAvailableId();
                    MaPhieuNhapSach = newId.ToString();
                }

                var phieuNhapSach = new PhieuNhapSach
                {
                    MaPhieuNhapSach = int.Parse(MaPhieuNhapSach),
                    NgayNhap = NgayNhap,
                };

                await _phieuNhapSachService.AddPhieuNhap(phieuNhapSach);

                foreach (var item in DanhSachDauSachPhieuNhap)
                {
                    var chiTiet = new ChiTietPhieuNhap
                    {
                        MaPhieuNhapSach = phieuNhapSach.MaPhieuNhapSach,
                        MaSach = item.SelectedSach.MaSach,
                        SoLuongNhap = item.SoLuongNhap,
                    };

                    await _phieuNhapSachChiTietService.AddChiTietPhieuNhap(chiTiet);

                    var sach = await _sachService.GetSachById(item.SelectedSach.MaSach);
                    sach.SoLuongTon += item.SoLuongNhap;
                    await _sachService.UpdateSach(sach);
                }

                MessageBox.Show($"Lập phiếu nhập sách thành công. Mã phiếu nhập sách: {MaPhieuNhapSach}",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi lập phiếu nhấp sách: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void PhieuNhapSachMoi()
        {
            SelectedDauSachPhieuNhap = null!;
            DanhSachDauSachPhieuNhap.Clear();
            _ = LoadDataAsync();
        }

        private void UpdateAvailableLists()
        {
            var selectedIds = DanhSachDauSachPhieuNhap.Select(r => r.SelectedSach.MaSach).ToHashSet();

            foreach (var row in DanhSachDauSachPhieuNhap)
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
        private void ThemDauSach()
        {
            var available = new List<Sach>(_danhSachSach);
            if (!available.Any())
            {
                MessageBox.Show("Đã nhập đầy đủ mặt hàng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var newItem = new DisplayDauSachPhieuNhap(available);
            DanhSachDauSachPhieuNhap.Add(newItem);

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
        }

        [RelayCommand]
        private void XoaDauSach()
        {
            if (SelectedDauSachPhieuNhap == null)
            {
                MessageBox.Show("Vui lòng chọn mặt hàng để xóa", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DanhSachDauSachPhieuNhap.Count > 0)
            {
                if (SelectedDauSachPhieuNhap.SelectedSach != null)
                {
                    _danhSachSach.Add(SelectedDauSachPhieuNhap.SelectedSach);
                    _danhSachSachDaChon.Remove(SelectedDauSachPhieuNhap.SelectedSach);
                }

                DanhSachDauSachPhieuNhap.Remove(SelectedDauSachPhieuNhap);

                UpdateAvailableLists();
            }
            else
            {
                MessageBox.Show("Không có mặt hàng nào để xóa", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        private void BoChonDauSach()
        {
            SelectedDauSachPhieuNhap = null!;
        }

        #endregion
    }

}
