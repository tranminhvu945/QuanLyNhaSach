using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.HoaDonBanViews;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace QuanLyNhaSach.ViewModels.HoaDonBanViewModel
{
    public partial class HoaDonBanPageViewModel :
        ObservableObject,
        IRecipient<DataReloadMessage>
    {
        private readonly IHoaDonService _hoaDonService;
        private readonly IServiceProvider _serviceProvider;

        public HoaDonBanPageViewModel(
            IHoaDonService HoaDonService,
            IServiceProvider serviceProvider)
        {
            _hoaDonService = HoaDonService;
            _serviceProvider = serviceProvider;

            WeakReferenceMessenger.Default.Register<DataReloadMessage>(this);

            _ = LoadDataAsync();
        }

        public void Receive(DataReloadMessage message)
        {
            _ = LoadDataAsync();
        }

        [ObservableProperty]
        private ObservableCollection<HoaDon> _filteredHoaDons = [];

        [ObservableProperty]
        private ObservableCollection<HoaDon> _danhSachHoaDon = [];

        [ObservableProperty]
        private HoaDon _selectedHoaDon = null!;

        // Methods
        private async Task LoadDataAsync()
        {
            var list = await _hoaDonService.GetAllHoaDon();
            DanhSachHoaDon = [.. list];
            SelectedHoaDon = null!;
        }

        [RelayCommand]
        private void SearchHoaDon()
        {
            SelectedHoaDon = null!;

            var traCuuHoaDonWindow = _serviceProvider.GetRequiredService<TraCuuHoaDonBanWindow>();

            if (traCuuHoaDonWindow.DataContext is TraCuuHoaDonBanViewModel viewModel)
            {
                viewModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(viewModel.SearchResults) && viewModel.SearchResults != null)
                    {
                        DanhSachHoaDon = viewModel.SearchResults;
                    }
                };
            }

            traCuuHoaDonWindow.Show();
        }
        
        [RelayCommand]
        private void AddHoaDon()
        {
            try
            {
                var addHoaDonWindow = _serviceProvider.GetRequiredService<ThemHoaDonBanWindow>();
                addHoaDonWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ thêm hoá đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SelectedHoaDon = null!;
        }

        [RelayCommand]
        private void EditHoaDon()
        {
            if (SelectedHoaDon == null || SelectedHoaDon.MaHoaDon == 0)
            {
                MessageBox.Show("Vui lòng chọn hoá đơn để chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var window = _serviceProvider.GetRequiredService<CapNhatHoaDonBanWindow>();
                window.Show();
                WeakReferenceMessenger.Default.Send(new SelectedIdMessage(SelectedHoaDon.MaHoaDon));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ chỉnh sửa hoá đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteHoaDon()
        {
            if (SelectedHoaDon == null)
            {
                MessageBox.Show("Vui lòng chọn hoá đơn để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa hoá đơn '{SelectedHoaDon.MaHoaDon}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _hoaDonService.DeleteHoaDon(SelectedHoaDon.MaHoaDon);
                    MessageBox.Show("Đã xóa hoá đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa hoá đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task LoadData()
        {
            SelectedHoaDon = null!;
            await LoadDataAsync();
            MessageBox.Show("Tải lại danh sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
