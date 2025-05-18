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
             ISachService sachService,
             IThamSoService thamSoService
)
        {
            _phieuNhapSachService = phieuNhapSachService;
            _sachService = sachService;
            _thamSoService = thamSoService;

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _danhSachSach = new List<Sach>(await _sachService.GetAllSach());
        }

        #region Bindings Properties
        private List<Sach> _danhSachSach = [];
        private List<Sach> _danhSachSachDaChon = [];

        [ObservableProperty]
        // Properties for binding
        private string _maPhieuNhapSach = string.Empty;

        [ObservableProperty]
        private ObservableCollection<DisplayDauSachPhieuNhap> _danhSachDauSachPhieuNhap = [];

        private DisplayDauSachPhieuNhap _selectedDauSachPhieuNhap = null!;
        public DisplayDauSachPhieuNhap SelectedDauSachPhieuNhap
        {
            get => _selectedDauSachPhieuNhap;
            set
            {
                _selectedDauSachPhieuNhap = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Sach> _saches = [];
        public ObservableCollection<Sach> Saches
        {
            get => _saches;
            set
            {
                _saches = value;
                OnPropertyChanged();
            }
        }

        private Sach _selectedSach = null!;
        public Sach SelectedSach
        {
            get => _selectedSach;
            set
            {
                _selectedSach = value;
                OnPropertyChanged();
            }
        }

        private DateTime _ngayNhap = DateTime.Now;
        public DateTime NgayNhap
        {
            get => _ngayNhap;
            set
            {
                _ngayNhap = value;
                OnPropertyChanged();
            }
        }

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
        private async Task PhieuNhapSachMoi()
        {
            SelectedDauSachPhieuNhap = null!;
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
                    .ToList();
                row.DanhSachSach = new ObservableCollection<Sach>(available);
            }
        }
        [RelayCommand]
        private async Task ThemDauSach()
        {
            var available = new List<Sach>(_danhSachSach);
            if (!available.Any())
            {
                MessageBox.Show("Đã nhập đầy đủ sách", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var newItem = new DisplayDauSachPhieuNhap(available);

            DanhSachDauSachPhieuNhap.Add(newItem);
            _danhSachSach.Remove(newItem.SelectedSach);
            _danhSachSachDaChon.Add(newItem.SelectedSach);

            UpdateAvailableLists();
        }

        [RelayCommand]
        private async Task XoaDauSach()
        {
            if (SelectedDauSachPhieuNhap == null)
            {
                MessageBox.Show("Vui lòng chọn đầu sách để xóa", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DanhSachDauSachPhieuNhap.Count > 0)
            {
                _danhSachSach.Add(SelectedDauSachPhieuNhap.SelectedSach);
                DanhSachDauSachPhieuNhap.Remove(SelectedDauSachPhieuNhap);
            }
            else
            {
                MessageBox.Show("Không có mặt hàng nào để xóa", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task BoChonDauSach()
        {
            SelectedDauSachPhieuNhap = null!;
        }

        #endregion
    }

}
