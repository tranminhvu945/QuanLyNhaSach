using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.PhieuThuViews;

namespace QuanLyNhaSach.ViewModels.PhieuThuViewModel
{
    public partial class TraCuuPhieuThuWindowViewModel : ObservableObject
    {
        // Services
        private IPhieuThuService _phieuThuService;
        private IKhachHangService _khachHangService;
        // Constructor
        public TraCuuPhieuThuWindowViewModel(
            IPhieuThuService phieuThuService, 
            IKhachHangService khachHangService)
        {
            _phieuThuService = phieuThuService;
            _khachHangService = khachHangService;

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var listKhachHang = await _khachHangService.GetAllKhachHang();

                KhachHanges.Clear();
                KhachHanges = [.. listKhachHang];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Binding Properties
        [ObservableProperty]
        private ObservableCollection<KhachHang> _khachHanges = [];
        [ObservableProperty]
        private string _tenKhachHang = string.Empty;
        [ObservableProperty]
        private string _maPhieuThu = string.Empty;
        [ObservableProperty]
        private string _dienThoai = string.Empty;
        [ObservableProperty]
        private string _diaChi = string.Empty;
        [ObservableProperty]
        private string _email = string.Empty;
        [ObservableProperty]
        private long _tienNoFrom = 0;
        [ObservableProperty]
        private long _tienNoTo = long.MaxValue;
        [ObservableProperty]
        private DateTime _ngayThuTienFrom = DateTime.MinValue;
        [ObservableProperty]
        private DateTime _ngayThuTienTo = DateTime.Now;
        [ObservableProperty]
        private long _soTienThuFrom = 0;
        [ObservableProperty]
        private long _soTienThuTo = long.MaxValue;
        [ObservableProperty]
        private ObservableCollection<PhieuThu> _searchResults = [];
        #endregion

        #region RelayCommand
        [RelayCommand]
        private void CloseWindow()
        {
            Application.Current.Windows.OfType<TraCuuPhieuThuWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private async Task SearchPhieuThu()
        {
            try
            {
                var phieuThus = await _phieuThuService.GetAllPhieuThu();
                ObservableCollection<PhieuThu> filteredResults = [.. phieuThus];

                if (!string.IsNullOrEmpty(MaPhieuThu))
                {
                    filteredResults = [.. filteredResults.Where(d => d.MaPhieuThu.ToString().Contains(MaPhieuThu))];
                }
                if (!string.IsNullOrEmpty(TenKhachHang))
                {
                    filteredResults = new ObservableCollection<PhieuThu>(filteredResults.Where(d => d.KhachHang.TenKhachHang != null && d.KhachHang.TenKhachHang.Contains(TenKhachHang)));
                }
                if (!string.IsNullOrEmpty(DienThoai))
                {
                    filteredResults = [.. filteredResults.Where(d => d.KhachHang.DienThoai != null  && d.KhachHang.DienThoai.Contains(DienThoai))];
                }
                if (!string.IsNullOrEmpty(DiaChi))
                {
                    filteredResults = [.. filteredResults.Where(d => d.KhachHang.DiaChi != null && d.KhachHang.DiaChi.Contains(DiaChi))];
                }
                if (!string.IsNullOrEmpty(Email))
                {
                    filteredResults = [.. filteredResults.Where(d => d.KhachHang.Email != null && d.KhachHang.Email.Contains(Email))];
                }
                if (TienNoFrom != 0 || TienNoTo != long.MaxValue)
                {
                    filteredResults = [.. filteredResults.Where(d => d.KhachHang.TienNo >= TienNoFrom && d.KhachHang.TienNo <= TienNoTo)];
                }
                if (NgayThuTienFrom != DateTime.MinValue || NgayThuTienTo != DateTime.Now)
                {
                    filteredResults = [.. filteredResults.Where(d => d.NgayThu >= NgayThuTienFrom && d.NgayThu <= NgayThuTienTo)];
                }
                if (SoTienThuFrom != 0 || SoTienThuTo != long.MaxValue)
                {
                    filteredResults = [.. filteredResults.Where(d => d.SoTienThu >= SoTienThuFrom && d.SoTienThu <= SoTienThuTo)];
                }

                SearchResults = [.. filteredResults];

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
            WeakReferenceMessenger.Default.Send(new SearchCompletedMessage<PhieuThu>(SearchResults));
            CloseWindow();
        }

        #endregion


    }
}
