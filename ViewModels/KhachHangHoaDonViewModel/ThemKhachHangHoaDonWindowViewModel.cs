using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.KhachHangHoaDonViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace QuanLyNhaSach.ViewModels.KhachHangHoaDonViewModel
{
    public partial class ThemKhachHangHoaDonWindowViewModel(IKhachHangService khachHangService) : ObservableObject
    {
        private readonly IKhachHangService _khachHangService = khachHangService;

        #region binding properties
        [ObservableProperty]
        private string _maKhachHang = string.Empty;
        [ObservableProperty]
        private string _tenKhachHang = string.Empty;
        [ObservableProperty]
        private string _dienThoai = string.Empty;
        [ObservableProperty]
        private string _email = string.Empty;
        [ObservableProperty]
        private string _diaChi = string.Empty;
        #endregion

        #region functional
        [RelayCommand]
        private void CloseWindow()
        {
            WeakReferenceMessenger.Default.Send(new DataReloadMessage());
            Application.Current.Windows.OfType<ThemKhachHangHoaDonWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private void KhachHangMoi()
        {
            MaKhachHang = string.Empty;
            TenKhachHang = string.Empty;
            DienThoai = string.Empty;
            Email = string.Empty;
            DiaChi = string.Empty;
        }

        [RelayCommand]
        private async Task TiepNhanKhachHang()
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
                MaKhachHang = (await _khachHangService.GenerateAvailableId()).ToString();
                KhachHang khachHang = new()
                {
                    MaKhachHang = int.Parse(MaKhachHang),
                    TenKhachHang = TenKhachHang,
                    DienThoai = DienThoai,
                    Email = Email,
                    DiaChi = DiaChi,
                };

                await _khachHangService.AddKhachHang(khachHang);
                MessageBox.Show("Tiếp nhận khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                KhachHangMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lưu khách hàng không thành công", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
