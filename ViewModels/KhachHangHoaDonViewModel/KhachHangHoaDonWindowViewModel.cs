using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.HoaDonBanViews;
using QuanLyNhaSach.Views.KhachHangHoaDonViews;
using QuanLyNhaSach.Views.KhachHangViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuanLyNhaSach.ViewModels.KhachHangHoaDonViewModel
{
    public partial class KhachHangHoaDonWindowViewModel : ObservableObject,
        IRecipient<SearchCompletedMessage<KhachHang>>, IRecipient<DataReloadMessage>
    {
        private readonly IKhachHangService _khachHangService;
        private readonly IServiceProvider _serviceProvider;

        public KhachHangHoaDonWindowViewModel(
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
                DanhSachKhachHang = new ObservableCollection<KhachHang>(list);
                SelectedKhachHang = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [ObservableProperty]
        private ObservableCollection<KhachHang> _danhSachKhachHang = new ObservableCollection<KhachHang>();

        [ObservableProperty]
        private KhachHang _selectedKhachHang = null!;

        [RelayCommand]
        private void AddKhachHang()
        {
            SelectedKhachHang = null;
            try
            {
                var addKhachHangWindow = _serviceProvider.GetRequiredService<ThemKhachHangHoaDonWindow>();
                addKhachHangWindow.ShowDialog();

                // Reload data sau khi thêm
                _ = LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ thêm khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private async Task TroVeHoaDon()
        {
            WeakReferenceMessenger.Default.Send(new DataReloadMessage());
            Application.Current.Windows.OfType<KhachHangHoaDonWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private async Task LoadData()
        {
            SelectedKhachHang = null;
            await LoadDataAsync();
            MessageBox.Show("Tải lại danh sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Receive(SearchCompletedMessage<KhachHang> message)
        {
            var searchResults = message.Value;

            if (searchResults != null && searchResults.Count > 0)
            {
                DanhSachKhachHang = new ObservableCollection<KhachHang>(searchResults);
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
