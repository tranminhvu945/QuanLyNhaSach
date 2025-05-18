using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.KhachHangViews;
using QuanLyNhaSach.Views.PhieuThuViews;

namespace QuanLyNhaSach.ViewModels.KhachHangViewModel
{
    public partial class KhachHangViewModel : ObservableObject, IRecipient<SearchCompletedMessage<KhachHang>>, IRecipient<DataReloadMessage>
    {
        private readonly IKhachHangService _khachHangService;
        private readonly IServiceProvider _serviceProvider;
        public KhachHangViewModel(
                IKhachHangService khachHangService,
                IServiceProvider serviceProvider)
        {
            _khachHangService = khachHangService;
            _serviceProvider = serviceProvider;

            WeakReferenceMessenger.Default.RegisterAll(this);

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var list = await _khachHangService.GetAllKhachHang();
                DanhSachKhachHang = [.. list];
                SelectedKhachHang = null!;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu mặt hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [ObservableProperty]
        private ObservableCollection<KhachHang> _danhSachKhachHang = [];
        [ObservableProperty]
        private KhachHang _selectedKhachHang = null!;

        [RelayCommand]
        private void AddKhachHang()
        {
            SelectedKhachHang = null!;
            try
            {
                var addKhachHangWindow = _serviceProvider.GetRequiredService<ThemKhachHangWindow>();
                addKhachHangWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ thêm khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        private async Task DeleteKhachHang()
        {
            if (SelectedKhachHang == null! || string.IsNullOrEmpty(SelectedKhachHang.MaKhachHang.ToString()))
            {
                MessageBox.Show("Vui lòng chọn khách hàng để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa khách này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await _khachHangService.DeleteKhachHang(SelectedKhachHang.MaKhachHang);
                    MessageBox.Show("Đã xóa khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi xóa khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task SearchKhachHang()
        {
            SelectedKhachHang = null!;

            var traCuuKhachHangWindow = _serviceProvider.GetRequiredService<TraCuuKhachHangWindow>();
            traCuuKhachHangWindow.Show();
        }

        [RelayCommand]
        private async Task LoadData()
        {
            SelectedKhachHang = null!;
            await LoadDataAsync();
            MessageBox.Show("Tải lại danh sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        [RelayCommand]
        private void EditKhachHang()
        {
            if (SelectedKhachHang == null || string.IsNullOrEmpty(SelectedKhachHang.TenKhachHang))
            {
                MessageBox.Show("Vui lòng chọn khách hàng để chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var window = _serviceProvider.GetRequiredService<CapNhatKhachHangWindow>();
                window.Show();
                WeakReferenceMessenger.Default.Send(new SelectedIdMessage(SelectedKhachHang.MaKhachHang));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ chỉnh sửa khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Receive(SearchCompletedMessage<KhachHang> message)
        {
            var searchResults = message.Value;

            if (searchResults.Count > 0)
            {
                DanhSachKhachHang = searchResults;
            }
            else
            {
                _ = LoadDataAsync();
            }
        }

        public void Receive(DataReloadMessage message)
        {
            _ = LoadDataAsync();
        }
    }
}
