using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views;

namespace QuanLyNhaSach.ViewModels.PhieuNhapSachViewModel
{
    public partial class TraCuuPhieuNhapSachViewModel : ObservableObject
    {
        // Services
        private readonly IPhieuNhapSachService _phieuNhapSachService;
        private readonly IChiTietPhieuNhapService _phieuNhapSachChiTietService;
        private readonly ISachService _sachService;

        public TraCuuPhieuNhapSachViewModel(
             IPhieuNhapSachService phieuNhapSachService,
             IChiTietPhieuNhapService phieuNhapSachChiTietService,
             ISachService sachService
        )
        {
            _phieuNhapSachService = phieuNhapSachService;
            _sachService = sachService;
            _phieuNhapSachChiTietService = phieuNhapSachChiTietService;

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var listSach = await _sachService.GetAllSach();
                var sortedSach = listSach.OrderBy(s => s.TenSach, StringComparer.CurrentCultureIgnoreCase).ToList();
                Saches = new ObservableCollection<Sach>(sortedSach);

                var distinctTheLoai = listSach
                    .Select(s => s.TheLoai?.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(s => s!)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(s => s, StringComparer.CurrentCultureIgnoreCase)
                    .ToList();

                ListTheLoai = new ObservableCollection<string>(distinctTheLoai);

                var distinctTacGia = listSach
                    .Select(s => s.TacGia?.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(s => s!)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(s => s, StringComparer.CurrentCultureIgnoreCase)
                    .ToList();

                ListTacGia = new ObservableCollection<string>(distinctTacGia);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Bindings Properties
        [ObservableProperty]
        private ObservableCollection<ChiTietPhieuNhap> _chiTietPhieuNhapSaches = [];
        [ObservableProperty]
        private ObservableCollection<Sach> _saches = [];
        [ObservableProperty]
        private Sach _selectedSach = new();
        [ObservableProperty]
        private string _selectedTheLoai = string.Empty;
        [ObservableProperty]
        private string _selectedTacGia = string.Empty;
        [ObservableProperty]
        private ObservableCollection<string> _listTheLoai = new();
        [ObservableProperty]
        private ObservableCollection<string> _listTacGia = new();
        [ObservableProperty]
        private string _maPhieuNhapSach = string.Empty;
        [ObservableProperty]
        private DateTime _ngayNhapFrom = DateTime.MinValue;
        [ObservableProperty]
        private DateTime _ngayNhapTo = DateTime.Now;
        [ObservableProperty]
        private int _tongSoLuongNhapFrom = 0;
        [ObservableProperty]
        private int _tongSoLuongNhapTo = int.MaxValue;
        [ObservableProperty]
        private int _soLuongNhapFrom = 0;
        [ObservableProperty]
        private int _soLuongNhapTo = int.MaxValue;
        [ObservableProperty]
        private int _soLuongTonFrom = 0;
        [ObservableProperty]
        private int _soLuongTonTo = int.MaxValue;
        [ObservableProperty]
        private ObservableCollection<PhieuNhapSach> _searchResults = [];
        #endregion

        #region RelayCommands
        [RelayCommand]
        private void CloseWindow()
        {
            Application.Current.Windows.OfType<TraCuuPhieuNhapSachWindow>().FirstOrDefault()?.Close();
        }
        [RelayCommand]
        private async Task SearchPhieuNhapSach()
        {
            try
            {
                var allPhieuNhap = await _phieuNhapSachService.GetAllPhieuNhap();
                var phieuNhapSachsFiltered = allPhieuNhap.ToList();

                if (!string.IsNullOrEmpty(MaPhieuNhapSach))
                {
                    phieuNhapSachsFiltered = [.. phieuNhapSachsFiltered.Where(p => p.MaPhieuNhapSach.ToString().Contains(MaPhieuNhapSach))];
                }
                if (NgayNhapFrom != DateTime.MinValue)
                {
                    phieuNhapSachsFiltered = phieuNhapSachsFiltered.Where(p => p.NgayNhap >= NgayNhapFrom).ToList();
                }

                if (NgayNhapTo != DateTime.MinValue)
                {
                    phieuNhapSachsFiltered = phieuNhapSachsFiltered.Where(p => p.NgayNhap <= NgayNhapTo).ToList();
                }

                var allSach = await _sachService.GetAllSach();
                var allChiTiet = await _phieuNhapSachChiTietService.GetAllChiTietPhieuNhap();

                if (SelectedSach.MaSach != 0)
                {
                    // Lọc theo mã sách cụ thể
                    phieuNhapSachsFiltered = phieuNhapSachsFiltered
                        .Where(p => p.DsChiTietNhap.Any(ct => ct.MaSach == SelectedSach.MaSach))
                        .ToList();
                }
                if (!string.IsNullOrWhiteSpace(SelectedTheLoai))
                {
                    var maSachTheoTheLoai = allSach
                        .Where(s => s.TheLoai?.Trim().Equals(SelectedTheLoai.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                        .Select(s => s.MaSach)
                        .ToHashSet();

                    phieuNhapSachsFiltered = phieuNhapSachsFiltered
                        .Where(p => p.DsChiTietNhap.Any(ct => maSachTheoTheLoai.Contains(ct.MaSach)))
                        .ToList();
                }
                if (!string.IsNullOrWhiteSpace(SelectedTacGia))
                {
                    var maSachTheoTacGia = allSach
                        .Where(s => s.TacGia?.Trim().Equals(SelectedTacGia.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                        .Select(s => s.MaSach)
                        .ToHashSet();

                    phieuNhapSachsFiltered = phieuNhapSachsFiltered
                        .Where(p => p.DsChiTietNhap.Any(ct => maSachTheoTacGia.Contains(ct.MaSach)))
                        .ToList();
                }
                if (TongSoLuongNhapFrom != 0 || TongSoLuongNhapTo != int.MaxValue)
                {
                    var phieuNhapIdsWithSoLuong = allChiTiet
                        .GroupBy(ct => ct.MaPhieuNhapSach)
                        .Where(g =>
                        {
                            var sumSoLuong = g.Sum(ct => ct.SoLuongNhap);
                            return sumSoLuong >= TongSoLuongNhapFrom && sumSoLuong <= TongSoLuongNhapTo;
                        })
                        .Select(g => g.Key)
                        .ToHashSet();

                    phieuNhapSachsFiltered = phieuNhapSachsFiltered
                        .Where(p => phieuNhapIdsWithSoLuong.Contains(p.MaPhieuNhapSach))
                        .ToList();
                }
                if (SoLuongNhapFrom > 0 || SoLuongNhapTo < int.MaxValue)
                {
                    var phieuNhapIdsWithSoLuong = allChiTiet
                        .Where(ct => ct.SoLuongNhap >= SoLuongNhapFrom && ct.SoLuongNhap <= SoLuongNhapTo)
                        .Select(ct => ct.MaPhieuNhapSach)
                        .ToHashSet();
                    phieuNhapSachsFiltered = phieuNhapSachsFiltered
                        .Where(p => phieuNhapIdsWithSoLuong.Contains(p.MaPhieuNhapSach))
                        .ToList();
                }
                if (SoLuongTonFrom > 0 || SoLuongTonTo < int.MaxValue)
                {
                    var sachIdsWithSoLuongTon = allSach
                        .Where(s => s.SoLuongTon >= SoLuongTonFrom && s.SoLuongTon <= SoLuongTonTo)
                        .Select(s => s.MaSach)
                        .ToHashSet();
                    phieuNhapSachsFiltered = phieuNhapSachsFiltered
                        .Where(p => p.DsChiTietNhap.Any(ct => sachIdsWithSoLuongTon.Contains(ct.MaSach)))
                        .ToList();
                }


                SearchResults = [.. phieuNhapSachsFiltered];
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
            //SearchCompleted?.Invoke(this, SearchResults);
            WeakReferenceMessenger.Default.Send(new SearchCompletedMessage<PhieuNhapSach>(SearchResults));
            CloseWindow();  
        }
        #endregion
    }
}

