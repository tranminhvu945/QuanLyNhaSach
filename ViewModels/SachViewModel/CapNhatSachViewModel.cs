using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.SachViews;

namespace QuanLyNhaSach.ViewModels.SachViewModel
{
    public partial class CapNhatSachViewModel :
        ObservableObject,
        IRecipient<SelectedIdMessage>
    {
        private readonly ISachService _sachService;
        private int _sachId;

        public CapNhatSachViewModel(
            ISachService sachService)
        {
            _sachService = sachService;
            WeakReferenceMessenger.Default.Register<SelectedIdMessage>(this);
        }

        public void Receive(SelectedIdMessage message)
        {
            _sachId = message.Value;
            _ = LoadDataAsync();
        }

        [ObservableProperty]
        private string _maSach = "";

        [ObservableProperty]
        private string _tenSach = "";

        [ObservableProperty]
        private string _theLoai = "";

        [ObservableProperty]
        private string _tacGia = "";

        [ObservableProperty]
        private int _soLuongTon = 0;

        // Methods
        private async Task LoadDataAsync()
        {
            try
            {
                var sach = await _sachService.GetSachById(_sachId);
                MaSach = sach.MaSach.ToString();
                TenSach = sach.TenSach;
                TacGia = sach.TacGia;
                TheLoai = sach.TheLoai;
                SoLuongTon = sach.SoLuongTon;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Close()
        {
            WeakReferenceMessenger.Default.Send(new DataReloadMessage());
            Application.Current.Windows.OfType<CapNhatSachWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private async Task CapNhatSach()
        {
            if (string.IsNullOrWhiteSpace(TenSach))
            {
                MessageBox.Show("Tên sách không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TheLoai))
            {
                MessageBox.Show("Thể loại không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TacGia))
            {
                MessageBox.Show("Tác giả không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SoLuongTon < 0)
            {
                MessageBox.Show("Số lượng tồn không được âm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Get existing mat hang
                var existingSach = await _sachService.GetSachById(_sachId);

                // Update properties
                existingSach.TenSach = TenSach;
                existingSach.TheLoai = TheLoai;
                existingSach.TacGia = TacGia;
                existingSach.SoLuongTon = SoLuongTon;

                // Save changes
                await _sachService.UpdateSach(existingSach);

                MessageBox.Show("Cập nhật đầu sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật đầu sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
