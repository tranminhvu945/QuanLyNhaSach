using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models.dto;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.Input;
using QuanLyNhaSach.Views.PhieuThuViews;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Views.SachViews;
using QuanLyNhaSach.Views.KhachHangViews;

namespace QuanLyNhaSach.ViewModels.PhieuThuViewModel
{
    public partial class CapNhatPhieuThuViewModel : ObservableObject, IRecipient<SelectedIdMessage>
    {
        // Services
        private readonly IPhieuThuService _phieuNhapSachService;
        private readonly IKhachHangService _khachHangService;
        private readonly IThamSoService _thamSoService;
        private readonly IServiceProvider _serviceProvider;
        private int _phieuThuId;

        public CapNhatPhieuThuViewModel(
            IPhieuThuService phieuNhapSachService,
            IKhachHangService khachHangService,
            IThamSoService thamSoService,
            IServiceProvider serviceProvider
)
        {
            _phieuNhapSachService = phieuNhapSachService;
            _khachHangService = khachHangService;
            _thamSoService = thamSoService;
            _serviceProvider = serviceProvider;

            WeakReferenceMessenger.Default.RegisterAll(this);
        }
        public void Receive(SelectedIdMessage message)
        {
            _phieuThuId = message.Value;
            // Load data
            _ = LoadDataAsync();
        }

        private PhieuThu phieuThuCu = null!;

        private async Task LoadDataAsync()
        {
            var thamso = await _thamSoService.GetThamSo();
            _quyDinhTienThuTienNo = thamso.QuyDinhTienThuTienNo;
            if (_quyDinhTienThuTienNo == true)
                NoiDung = "Đang áp dụng";
            else
                NoiDung = "Không áp dụng";

            var listKhachHang = await _khachHangService.GetAllKhachHang();
            KhachHanges = [.. listKhachHang];

            try
            {
                phieuThuCu = await _phieuNhapSachService.GetPhieuThuById(_phieuThuId);
                if (phieuThuCu != null)
                {
                    MaPhieuThu = phieuThuCu.MaPhieuThu.ToString();
                    SoDienThoai = phieuThuCu.KhachHang.DienThoai ?? string.Empty;
                    TenKhachHang = phieuThuCu.KhachHang.TenKhachHang ?? string.Empty;
                    Email = phieuThuCu.KhachHang.Email ?? string.Empty;
                    DiaChi = phieuThuCu.KhachHang.DiaChi ?? string.Empty;
                    NgayThuTien = phieuThuCu.NgayThu;
                    SoTienThu = phieuThuCu.SoTienThu;

                    TienNo = phieuThuCu.KhachHang.TienNo + phieuThuCu.SoTienThu;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu phiếu thu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Bindings Properties
        [ObservableProperty]
        private string _maPhieuThu = string.Empty;
        [ObservableProperty]
        private ObservableCollection<KhachHang> _khachHanges = [];

        private string _soDienThoai = string.Empty;
        public string SoDienThoai
        {
            get => _soDienThoai;
            set
            {
                if (_soDienThoai != value)
                {
                    _soDienThoai = value;
                    OnPropertyChanged(nameof(SoDienThoai));
                    UpdateKhachHangInfoBySoDienThoai(value);
                }
            }
        }

        private void UpdateKhachHangInfoBySoDienThoai(string soDienThoai)
        {
            // Kiểm tra chỉ khi đủ 10 số mới tìm khách hàng
            if (string.IsNullOrWhiteSpace(soDienThoai) || soDienThoai.Length != 10)
            {
                // Chưa đủ 10 số thì reset các thuộc tính
                TenKhachHang = string.Empty;
                Email = string.Empty;
                DiaChi = string.Empty;
                TienNo = 0;
                return;
            }

            // Tìm khách hàng theo số điện thoại
            var khachHang = KhachHanges?.FirstOrDefault(kh => kh.DienThoai == soDienThoai);
            if (khachHang != null)
            {
                TenKhachHang = khachHang.TenKhachHang ?? string.Empty;
                Email = khachHang.Email ?? string.Empty;
                DiaChi = khachHang.DiaChi ?? string.Empty;
                TienNo = khachHang.TienNo;
            }
            else
            {
                TenKhachHang = string.Empty;
                Email = string.Empty;
                DiaChi = string.Empty;
                TienNo = 0;
            }
        }

        [ObservableProperty]
        private string _tenKhachHang = string.Empty;
        [ObservableProperty]
        private string _email = string.Empty;
        [ObservableProperty]
        private string _diaChi = string.Empty;
        [ObservableProperty]
        private long _tienNo = 0;
        [ObservableProperty]
        private DateTime _ngayThuTien = DateTime.Now;
        [ObservableProperty]
        private long _soTienThu = 0;
        [ObservableProperty]
        private string _noiDung = string.Empty;

        private bool _quyDinhTienThuTienNo = true;
        #endregion

        #region RelayCommand
        [RelayCommand]
        private void CloseWindow()
        {
            WeakReferenceMessenger.Default.Send(new DataReloadMessage());
            Application.Current.Windows.OfType<CapNhatPhieuThuWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private async Task CapNhatPhieuThu()
        {
            try
            {
                var khachHang = KhachHanges?.FirstOrDefault(kh => kh.DienThoai == SoDienThoai);
                if (khachHang == null)
                {
                    MessageBox.Show("Khách hàng không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var thamso = await _thamSoService.GetThamSo();
                var QuyDinhTienThuTienNo = thamso.QuyDinhTienThuTienNo;

                if (QuyDinhTienThuTienNo && SoTienThu > khachHang.TienNo + phieuThuCu.SoTienThu)
                {
                    MessageBox.Show("Số tiền thu không được lớn hơn số tiền nợ của khách hàng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                khachHang.TienNo = khachHang.TienNo + phieuThuCu.SoTienThu - SoTienThu;
                phieuThuCu.SoTienThu = SoTienThu;
                await _phieuNhapSachService.UpdatePhieuThu(phieuThuCu);
                await _khachHangService.UpdateKhachHang(khachHang);

                MessageBox.Show($"Cập nhật phiếu thu thành công. Mã phiếu thu: {MaPhieuThu}",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi cập nhật phiếu thu: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
