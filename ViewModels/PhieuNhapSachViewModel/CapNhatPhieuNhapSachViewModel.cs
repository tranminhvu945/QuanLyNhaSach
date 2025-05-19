using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using QuanLyNhaSach.Models.dto;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views;
using System.ComponentModel;
using System.Windows.Input;

namespace QuanLyNhaSach.ViewModels.PhieuNhapSachViewModel
{
    public partial class CapNhatPhieuNhapSachViewModel : INotifyPropertyChanged
    {
        // Services
        private readonly IPhieuNhapSachService _phieuNhapSachService;
        private readonly IChiTietPhieuNhapService _phieuNhapSachChiTietService;
        private readonly ISachService _sachService;
        private readonly IThamSoService _thamSoService;
        private readonly int _maPhieuNhapSachPassed;

        // Commands 
        public ICommand CloseWindowCommand { get; }
        public ICommand CapNhatPhieuNhapSachCommand { get; }
        public ICommand ThemDauSachCommand { get; }
        public ICommand XoaDauSachCommand { get; }
        public ICommand BoChonDauSachCommand { get; }

        public CapNhatPhieuNhapSachViewModel(
            IPhieuNhapSachService phieuNhapSachService,
            IChiTietPhieuNhapService phieuNhapSachChiTietService,
            ISachService sachService,
            IThamSoService thamSoService,
            int maPhieuNhapSachPassed
        )
        {
            _phieuNhapSachService = phieuNhapSachService;
            _sachService = sachService;
            _phieuNhapSachChiTietService = phieuNhapSachChiTietService;
            _thamSoService = thamSoService;
            _maPhieuNhapSachPassed = maPhieuNhapSachPassed;

            // Initialize commands
            CloseWindowCommand = new RelayCommand(CloseWindow);
            CapNhatPhieuNhapSachCommand = new RelayCommand(CapNhatPhieuNhapSach);
            ThemDauSachCommand = new RelayCommand(ThemDauSach);
            XoaDauSachCommand = new RelayCommand(XoaDauSach);
            BoChonDauSachCommand = new RelayCommand(BoChonDauSach);

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                MaPhieuNhapSach = _maPhieuNhapSachPassed.ToString();
                var phieuNhapSach = await _phieuNhapSachService.GetPhieuNhapById(_maPhieuNhapSachPassed);
                NgayNhap = phieuNhapSach.NgayNhap;

                _danhSachSach = [.. (await _sachService.GetAllSach())];

                DanhSachDauSachPhieuNhap.Clear();

                var listChiTietPhieuNhapSach = await _phieuNhapSachChiTietService.GetChiTietPhieuNhapByPhieuNhapId(_maPhieuNhapSachPassed);
                foreach (var chiTiet in listChiTietPhieuNhapSach)
                {
                    DanhSachDauSachPhieuNhap.Add(new DisplayDauSachPhieuNhap(_danhSachSach)
                    {
                        SelectedSach = await _sachService.GetSachById(chiTiet.MaSach),
                        SoLuongNhap = chiTiet.SoLuongNhap,
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi tải dữ liệu: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Bindings Properties
        private List<Sach> _danhSachSach = [];
        private List<Sach> _danhSachSachDaChon = [];

        private string _maPhieuNhapSach = string.Empty;
        public string MaPhieuNhapSach
        {
            get => _maPhieuNhapSach;
            set
            {
                _maPhieuNhapSach = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DisplayDauSachPhieuNhap> _danhSachDauSachPhieuNhap = [];
        public ObservableCollection<DisplayDauSachPhieuNhap> DanhSachDauSachPhieuNhap
        {
            get => _danhSachDauSachPhieuNhap;
            set
            {
                _danhSachDauSachPhieuNhap = value;
                OnPropertyChanged();
            }
        }

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
        private void CloseWindow()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
            Application.Current.Windows.OfType<CapNhatPhieuNhapSachWindow>().FirstOrDefault()?.Close();
        }

        private void CapNhatPhieuNhapSach()
        {
            _ = CapNhatPhieuNhapSachAsync();
        }
        private async Task CapNhatPhieuNhapSachAsync()
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

                var phieuNhapSach = await _phieuNhapSachService.GetPhieuNhapById(_maPhieuNhapSachPassed);
                phieuNhapSach.NgayNhap = NgayNhap;
                await _phieuNhapSachService.UpdatePhieuNhap(phieuNhapSach);


                // Lấy danh sách chi tiết cũ để khôi phục tồn kho
                var oldChiTietList = await _phieuNhapSachChiTietService.GetChiTietPhieuNhapByPhieuNhapId(_maPhieuNhapSachPassed);
                foreach (var oldChiTiet in oldChiTietList)
                {
                    var sach = await _sachService.GetSachById(oldChiTiet.MaSach);
                    if (sach != null)
                    {
                        // Trừ lại số lượng tồn tương ứng với chi tiết cũ (khôi phục tồn trước khi cập nhật)
                        sach.SoLuongTon -= oldChiTiet.SoLuongNhap;
                        if (sach.SoLuongTon < 0) sach.SoLuongTon = 0;
                        await _sachService.UpdateSach(sach);
                    }
                }
                // Xóa chi tiết phiếu nhập cũ
                await _phieuNhapSachChiTietService.DeleteChiTietPhieuNhapByPhieuNhapId(_maPhieuNhapSachPassed);


                // Thêm chi tiết phiếu nhập mới
                foreach (var item in DanhSachDauSachPhieuNhap)
                {
                    var newChiTiet = new ChiTietPhieuNhap
                    {
                        MaPhieuNhapSach = phieuNhapSach.MaPhieuNhapSach,
                        MaSach = item.SelectedSach.MaSach,
                        SoLuongNhap = item.SoLuongNhap,
                    };

                    await _phieuNhapSachChiTietService.AddChiTietPhieuNhap(newChiTiet);

                    var sach = await _sachService.GetSachById(item.SelectedSach.MaSach);
                    if (sach != null)
                    {
                        sach.SoLuongTon += item.SoLuongNhap;
                        await _sachService.UpdateSach(sach);
                    }
                }

                MessageBox.Show("Cập nhật phiếu nhập sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật phiếu nhập sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            _danhSachSach.Remove(newItem.SelectedSach);
            _danhSachSachDaChon.Add(newItem.SelectedSach);

            UpdateAvailableLists();
        }

        private void XoaDauSach()
        {
            if (SelectedDauSachPhieuNhap == null!)
            {
                MessageBox.Show("Vui lòng chọn mặt hàng để xóa", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void BoChonDauSach()
        {
            SelectedDauSachPhieuNhap = null!;
        }
        #endregion

        public event EventHandler? DataChanged;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
