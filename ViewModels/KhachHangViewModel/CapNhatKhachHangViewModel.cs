using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.KhachHangViews;

namespace QuanLyNhaSach.ViewModels.KhachHangViewModel
{
    public partial class CapNhatKhachHangViewModel : ObservableObject, IRecipient<SelectedIdMessage>
    {
        private readonly IKhachHangService _khachHangService;
        private int _khachHangId = 1;
        public CapNhatKhachHangViewModel(IKhachHangService khachHangService)
        {
            _khachHangService = khachHangService;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [ObservableProperty]
        private int _maKhachHang;
        [ObservableProperty]
        private string _tenKhachHang = string.Empty;
        [ObservableProperty]
        private string _dienThoai = string.Empty;
        [ObservableProperty]
        private string _email = string.Empty;
        [ObservableProperty]
        private string _diaChi = string.Empty;

        private async Task LoadDataAsync()
        {
            try
            {
                var khachHang = await _khachHangService.GetKhachHangById(_khachHangId);
                MaKhachHang = khachHang.MaKhachHang;
                TenKhachHang = khachHang.TenKhachHang;
                DienThoai = khachHang.DienThoai;
                Email = khachHang.Email;
                DiaChi = khachHang.DiaChi;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu đại lý: {ex.Message}", "Lỗi", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

        }

        [RelayCommand]
        private void CloseWindow()
        {
            WeakReferenceMessenger.Default.Send(new DataReloadMessage());
            Application.Current.Windows.OfType<CapNhatKhachHangWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private async Task CapNhatKhachHang()
        {
            if (string.IsNullOrWhiteSpace(TenKhachHang))
            {
                MessageBox.Show("Tên khách hàng không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Check SDT
            if (string.IsNullOrWhiteSpace(DienThoai))
            {
                MessageBox.Show("Số điện thoại không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (!DienThoai.All(char.IsDigit) || !(DienThoai.Length == 10 || DienThoai.Length == 11) || !DienThoai.StartsWith("0"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Check email
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(emailPattern);
            if (string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("Email không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (char.IsDigit(Email[0]))
            {
                MessageBox.Show("Email không được bắt đầu bằng số.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (!regex.IsMatch(Email))
            {
                MessageBox.Show("Email không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int atIndex = Email.IndexOf('@');
            string localPart = Email.Substring(0, atIndex);
            if (localPart.Contains("."))
            {
                MessageBox.Show("Email không được có dấu chấm trong phần trước '@'.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(DiaChi))
            {
                MessageBox.Show("Địa chỉ không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var existingDaiLy = await _khachHangService.GetKhachHangById(_khachHangId);
                existingDaiLy.TenKhachHang = TenKhachHang;
                existingDaiLy.DienThoai = DienThoai;
                existingDaiLy.Email = Email;
                existingDaiLy.DiaChi = DiaChi;

                await _khachHangService.UpdateKhachHang(existingDaiLy);
                MessageBox.Show("Cập nhật khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Receive(SelectedIdMessage message)
        {
            _khachHangId = message.Value;
            _ = LoadDataAsync();
        }
    }
}
