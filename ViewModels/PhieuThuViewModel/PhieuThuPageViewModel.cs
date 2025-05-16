using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.PhieuThuViews;

namespace QuanLyNhaSach.ViewModels.PhieuThuViewModel
{
    public partial class PhieuThuPageViewModel :
        ObservableObject,
        IRecipient<SearchCompletedMessage<PhieuThu>>,
        IRecipient<DataReloadMessage>
    {
        // Services
        private readonly IPhieuThuService _phieuThuService;
        private readonly IServiceProvider _serviceProvider;

        public PhieuThuPageViewModel(
                IPhieuThuService phieuThuService,
                IServiceProvider serviceProvider )
        {
            _phieuThuService = phieuThuService;
            _serviceProvider = serviceProvider;

            WeakReferenceMessenger.Default.RegisterAll(this);
            _ = LoadDataAsync();
        }

        public void Receive(DataReloadMessage message)
        {
            _ = LoadDataAsync();
        }
        public void Receive(SearchCompletedMessage<PhieuThu> message)
        {
            var searchResults = message.Value;

            if (searchResults.Count > 0)
            {
                DanhSachPhieuThu = searchResults;
            }
            else
            {
                _ = LoadDataAsync();
            }
        }
        private async Task LoadDataAsync()
        {
            try
            {
                var list = await _phieuThuService.GetAllPhieuThu();
                DanhSachPhieuThu = [.. list];
                SelectedPhieuThu = null!;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu mặt hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Bindings Properties
        [ObservableProperty]
        private ObservableCollection<PhieuThu> _filteredPhieuThus = [];
        [ObservableProperty]
        private ObservableCollection<PhieuThu> _danhSachPhieuThu = [];
        [ObservableProperty]
        private PhieuThu _selectedPhieuThu = null!;
        #endregion

        #region RelayCommand
        [RelayCommand]
        private void AddPhieuThu()
        {
            SelectedPhieuThu = null!;
            try
            {
                var addPhieuThuWindow = _serviceProvider.GetRequiredService<ThemPhieuThuWindow>();
                addPhieuThuWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ thêm phiếu thu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        private async Task DeletePhieuThu()
        {
            if (SelectedPhieuThu == null! || string.IsNullOrEmpty(SelectedPhieuThu.MaPhieuThu.ToString()))
            {
                MessageBox.Show("Vui lòng chọn phiếu thu để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu thu này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _phieuThuService.DeletePhieuThu(SelectedPhieuThu.MaPhieuThu);
                    MessageBox.Show("Đã xóa phiếu thu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi xóa phiếu thu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task SearchPhieuThu()
        {
            SelectedPhieuThu = null!;

            var traCuuPhieuThuWindow = _serviceProvider.GetRequiredService<TraCuuPhieuThuWindow>();
            traCuuPhieuThuWindow.Show();
        }

        [RelayCommand]
        private async Task LoadData()
        {
            SelectedPhieuThu = null!;
            await LoadDataAsync();
            MessageBox.Show("Tải lại danh sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

    }
}
