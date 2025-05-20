using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using QuanLyNhaSach.Models.dto;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;

namespace QuanLyNhaSach.ViewModels.PhieuNhapSachViewModel
{
    public partial class CapNhatPhieuNhapSachViewModel : ObservableObject, IRecipient<SelectedIdMessage>
    {
        // Services
        private readonly IPhieuNhapSachService _phieuNhapSachService;
        private readonly IChiTietPhieuNhapService _phieuNhapSachChiTietService;
        private readonly ISachService _sachService;
        private readonly IThamSoService _thamSoService;
        private int _phieuNhapSachId;

        public CapNhatPhieuNhapSachViewModel(
            IPhieuNhapSachService phieuNhapSachService,
            IChiTietPhieuNhapService phieuNhapSachChiTietService,
            ISachService sachService,
            IThamSoService thamSoService
        )
        {
            _phieuNhapSachService = phieuNhapSachService;
            _sachService = sachService;
            _phieuNhapSachChiTietService = phieuNhapSachChiTietService;
            _thamSoService = thamSoService;

            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        public void Receive(SelectedIdMessage message)
        {
            _phieuNhapSachId = message.Value;
            // Load data
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
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

            try
            {
                MaPhieuNhapSach = _phieuNhapSachId.ToString();
                var phieuNhapSach = await _phieuNhapSachService.GetPhieuNhapById(_phieuNhapSachId);
                NgayNhap = phieuNhapSach.NgayNhap;

                // Lấy toàn bộ sách trong kho
                var allSach = await _sachService.GetAllSach();

                _danhSachSach = new List<Sach>(allSach);
                _danhSachSachDaChon = new List<Sach>();
                DanhSachDauSachPhieuNhap.Clear();

                var listChiTietPhieuNhapSach = await _phieuNhapSachChiTietService.GetChiTietPhieuNhapByPhieuNhapId(_phieuNhapSachId);
                foreach (var chiTiet in listChiTietPhieuNhapSach)
                {
                    var sachDaChon = await _sachService.GetSachById(chiTiet.MaSach);
                    if (sachDaChon != null)
                    {
                        var newItem = new DisplayDauSachPhieuNhap(_danhSachSach)
                        {
                            SelectedSach = sachDaChon,
                            SoLuongNhap = chiTiet.SoLuongNhap,
                        };

                        DanhSachDauSachPhieuNhap.Add(newItem);
                        // Cập nhật danh sách đã chọn và chưa chọn
                        _danhSachSachDaChon.Add(sachDaChon);
                        _danhSachSach.Remove(sachDaChon);

                        // Đăng ký event lắng nghe thay đổi chọn sách
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
                    }
                }

                UpdateAvailableLists();
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
            Application.Current.Windows.OfType<CapNhatPhieuNhapSachWindow>().FirstOrDefault()?.Close();
        }
        [RelayCommand]
        private async Task CapNhatPhieuNhapSach()
        {
            try
            {
                if (DanhSachDauSachPhieuNhap.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một đầu sách", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var thamso = await _thamSoService.GetThamSo();

                // Validate quantities and prices
                foreach (var item in DanhSachDauSachPhieuNhap)
                {
                    if (thamso.QuyDinhSoLuongTonToiDa && item.SoLuongTon > thamso.SoLuongTonToiDa)
                    {
                        MessageBox.Show($"Chỉ nhập những đầu sách có số lượng tồn dưới {thamso.SoLuongTonToiDa}",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (item.SoLuongNhap <= 0)
                    {
                        MessageBox.Show($"Số lượng nhập phải lớn hơn 0",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (thamso.QuyDinhSoLuongNhapToiThieu && item.SoLuongNhap < thamso.SoLuongNhapToiThieu)
                    {
                        MessageBox.Show($"Số lượng nhập của đầu sách  phải lớn hơn {thamso.SoLuongNhapToiThieu}",
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                var phieuNhapSach = await _phieuNhapSachService.GetPhieuNhapById(_phieuNhapSachId);
                phieuNhapSach.NgayNhap = NgayNhap;
                await _phieuNhapSachService.UpdatePhieuNhap(phieuNhapSach);


                // Lấy danh sách chi tiết cũ để khôi phục tồn kho
                var oldChiTietList = await _phieuNhapSachChiTietService.GetChiTietPhieuNhapByPhieuNhapId(_phieuNhapSachId);
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
                await _phieuNhapSachChiTietService.DeleteChiTietPhieuNhapByPhieuNhapId(_phieuNhapSachId);


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
