using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views;

namespace QuanLyNhaSach.ViewModels.PhieuNhapSachViewModel
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        // Services
        private readonly IPhieuNhapSachService _phieuNhapSachService;
        private readonly IChiTietPhieuNhapService _phieuNhapSachChiTietService;
        private readonly ISachService _sachService;
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<int, CapNhatPhieuNhapSachViewModel> _capNhatPhieuNhapSachFactory;
        // Commands
        public ICommand LoadDataCommand { get; }
        public ICommand LapPhieuNhapSachCommand { get; }
        public ICommand EditPhieuNhapSachCommand { get; }
        public ICommand DeletePhieuNhapSachCommand { get; }
        public ICommand SearchPhieuNhapSachCommnad { get; }

        public MainWindowViewModel(
                IPhieuNhapSachService phieuNhapSachService,
                IChiTietPhieuNhapService phieuNhapSachChiTietService,
                ISachService sachService,
                IServiceProvider serviceProvider,
                Func<int, CapNhatPhieuNhapSachViewModel> capNhatPhieuNhapSachFactory
        )
        {
            _phieuNhapSachService = phieuNhapSachService;
            _phieuNhapSachChiTietService = phieuNhapSachChiTietService;
            _sachService = sachService;
            _serviceProvider = serviceProvider;
            _capNhatPhieuNhapSachFactory = capNhatPhieuNhapSachFactory;

            // Initialize commands
            LoadDataCommand = new RelayCommand(async () => await LoadDataExecuteAsync());
            LapPhieuNhapSachCommand = new RelayCommand(LapPhieuNhapSach);
            EditPhieuNhapSachCommand = new RelayCommand(EditPhieuNhapSach);
            DeletePhieuNhapSachCommand = new RelayCommand(DeletePhieuNhapSach);
            SearchPhieuNhapSachCommnad = new RelayCommand(SearchPhieuNhapSach);

            // Load initial data
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var list = await _phieuNhapSachService.GetAllPhieuNhap();
                DanhSachPhieuNhapSach = [.. list];
                SelectedPhieuNhapSach = null!;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu mặt hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Bindings Properties
        private ObservableCollection<PhieuNhapSach> _filteredPhieuNhapSachs = [];
        public ObservableCollection<PhieuNhapSach> FilteredPhieuNhapSachs
        {
            get => _filteredPhieuNhapSachs;
            set
            {
                _filteredPhieuNhapSachs = value;
                OnPropertyChanged(nameof(FilteredPhieuNhapSachs));
            }
        }

        private ObservableCollection<PhieuNhapSach> _danhSachPhieuNhapSach = [];
        public ObservableCollection<PhieuNhapSach> DanhSachPhieuNhapSach
        {
            get => _danhSachPhieuNhapSach;
            set
            {
                _danhSachPhieuNhapSach = value;
                OnPropertyChanged(nameof(DanhSachPhieuNhapSach));
            }
        }

        private PhieuNhapSach _selectedPhieuNhapSach = null!;
        public PhieuNhapSach SelectedPhieuNhapSach
        {
            get => _selectedPhieuNhapSach;
            set
            {
                _selectedPhieuNhapSach = value;
                OnPropertyChanged(nameof(SelectedPhieuNhapSach));
            }
        }
        #endregion

        #region RelayCommand
        private void LapPhieuNhapSach()
        {
            SelectedPhieuNhapSach = null!;
            try
            {
                var LapPhieuNhapSachWindow = _serviceProvider.GetRequiredService<LapPhieuNhapSachWindow>();
                if (LapPhieuNhapSachWindow.DataContext is LapPhieuNhapSachViewModel viewModel)
                {
                    viewModel.DataChanged += async (sender, e) => await LoadDataAsync();
                }
                LapPhieuNhapSachWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ lập phiếu nhập sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeletePhieuNhapSach()
        {
            _ = DeletePhieuNhapSachAsync();
        }

        private async Task DeletePhieuNhapSachAsync()
        {
            if (SelectedPhieuNhapSach == null! || string.IsNullOrEmpty(SelectedPhieuNhapSach.MaPhieuNhapSach.ToString()))
            {
                MessageBox.Show("Vui lòng chọn phiếu nhạp sách để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu nhập sách này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Lấy danh sách chi tiết phiếu nhập theo mã phiếu nhập
                    var chiTietPhieuNhapList = await _phieuNhapSachChiTietService.GetChiTietPhieuNhapByPhieuNhapId(SelectedPhieuNhapSach.MaPhieuNhapSach);

                    // Khôi phục lại số lượng tồn cho từng sách
                    foreach (var chiTiet in chiTietPhieuNhapList)
                    {
                        var sach = await _sachService.GetSachById(chiTiet.MaSach);
                        if (sach != null)
                        {
                            sach.SoLuongTon -= chiTiet.SoLuongNhap;
                            if (sach.SoLuongTon < 0) sach.SoLuongTon = 0;
                            await _sachService.UpdateSach(sach);
                        }
                    }
                    // Xóa chi tiết phiếu nhập cũ
                    await _phieuNhapSachChiTietService.DeleteChiTietPhieuNhapByPhieuNhapId(SelectedPhieuNhapSach.MaPhieuNhapSach);

                    // Xóa phiếu nhập sách
                    await _phieuNhapSachService.DeletePhieuNhap(SelectedPhieuNhapSach.MaPhieuNhapSach);

                    MessageBox.Show("Đã xóa phiếu nhập sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi xóa phiếu nhập sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void EditPhieuNhapSach()
        {
            if (SelectedPhieuNhapSach == null!)
            {
                MessageBox.Show("Vui lòng chọn phiếu nhập sách để chỉnh sửa!", "Thông báo", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            try
            {
                var viewmodel = _capNhatPhieuNhapSachFactory(SelectedPhieuNhapSach.MaPhieuNhapSach);
                viewmodel.DataChanged += async (sender, e) => await LoadDataAsync();

                var window = new CapNhatPhieuNhapSachWindow(viewmodel);
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ chỉnh sửa phiếu nhập sách: {ex.Message}", "Lỗi", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SearchPhieuNhapSach()
        {
            SelectedPhieuNhapSach = null!;

            var traCuuPhieuThuWindow = _serviceProvider.GetRequiredService<TraCuuPhieuNhapSachWindow>();
            traCuuPhieuThuWindow.Show();
        }

        private async Task LoadDataExecuteAsync()
        {
            SelectedPhieuNhapSach = null!;
            await LoadDataAsync();
            MessageBox.Show("Tải lại danh sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
