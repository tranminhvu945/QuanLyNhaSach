using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views;

namespace QuanLyNhaSach.ViewModels.PhieuNhapSachViewModel
{
    public partial class MainWindowViewModel :
        ObservableObject,
        IRecipient<SearchCompletedMessage<PhieuNhapSach>>,
        IRecipient<DataReloadMessage>
    {
        // Services
        private readonly IPhieuNhapSachService _phieuNhapSachService;
        private readonly IChiTietPhieuNhapService _phieuNhapSachChiTietService;
        private readonly ISachService _sachService;
        private readonly IServiceProvider _serviceProvider;

        public MainWindowViewModel(
                IPhieuNhapSachService phieuNhapSachService,
                IChiTietPhieuNhapService phieuNhapSachChiTietService,
                ISachService sachService,
                IServiceProvider serviceProvider
        )
        {
            _phieuNhapSachService = phieuNhapSachService;
            _phieuNhapSachChiTietService = phieuNhapSachChiTietService;
            _sachService = sachService;
            _serviceProvider = serviceProvider;

            WeakReferenceMessenger.Default.RegisterAll(this);

            _ = LoadDataAsync();
        }

        public void Receive(DataReloadMessage message)
        {
            _ = LoadDataAsync();
        }

        public void Receive(SearchCompletedMessage<PhieuNhapSach> message)
        {
            var searchResults = message.Value;
            if (searchResults.Count > 0)
            {
                DanhSachPhieuNhapSach = searchResults;
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
        [ObservableProperty]
        private ObservableCollection<PhieuNhapSach> _filteredPhieuNhapSachs = [];
        [ObservableProperty]
        private ObservableCollection<PhieuNhapSach> _danhSachPhieuNhapSach = [];
        [ObservableProperty]
        private PhieuNhapSach _selectedPhieuNhapSach = null!;
        #endregion

        #region RelayCommand
        [RelayCommand]
        private void LapPhieuNhapSach()
        {
            SelectedPhieuNhapSach = null!;
            try
            {
                var LapPhieuNhapSachWindow = _serviceProvider.GetRequiredService<LapPhieuNhapSachWindow>();
                LapPhieuNhapSachWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ lập phiếu nhập sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeletePhieuNhapSach()
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

        [RelayCommand]
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
                var window = _serviceProvider.GetRequiredService<CapNhatPhieuNhapSachWindow>();
                window.Show();
                WeakReferenceMessenger.Default.Send(new SelectedIdMessage(SelectedPhieuNhapSach.MaPhieuNhapSach));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ chỉnh sửa phiếu nhập sách: {ex.Message}", "Lỗi", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void SearchPhieuNhapSach()
        {
            SelectedPhieuNhapSach = null!;
            var traCuuPhieuThuWindow = _serviceProvider.GetRequiredService<TraCuuPhieuNhapSachWindow>();
            traCuuPhieuThuWindow.Show();
        }

        [RelayCommand]
        private async Task LoadData()
        {
            SelectedPhieuNhapSach = null!;
            await LoadDataAsync();
            MessageBox.Show("Tải lại danh sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion
    }
}
