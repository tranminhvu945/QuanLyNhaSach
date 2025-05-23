using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.KhachHangHoaDonViews;
using QuanLyNhaSach.Views.KhachHangViews;
using QuanLyNhaSach.Views.PhieuThuViews;

namespace QuanLyNhaSach.ViewModels.PhieuThuViewModel
{
    public partial class ThemPhieuThuWindowViewModel : ObservableObject
    {
        // Services
        private readonly IPhieuThuService _phieuThuService;
        private readonly IKhachHangService _khachHangService;
        private readonly IThamSoService _thamSoService;
        private readonly IServiceProvider _serviceProvider;

        public ThemPhieuThuWindowViewModel(
            IPhieuThuService phieuThuService,
            IKhachHangService khachHangService,
            IThamSoService thamSoService,
            IServiceProvider serviceProvider
)
        {
            _phieuThuService = phieuThuService;
            _khachHangService = khachHangService;
            _thamSoService = thamSoService;
            _serviceProvider = serviceProvider;

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var listKhachHang = await _khachHangService.GetAllKhachHang();
            KhachHanges = [.. listKhachHang];
            if (KhachHanges.Count() > 0)
            {
                TenKhachHang = string.Empty;
                SoDienThoai = string.Empty;
                Email = string.Empty;
                DiaChi = string.Empty;
                TienNo = 0;
                var thamso = await _thamSoService.GetThamSo();
                _quyDinhTienThuTienNo = thamso.QuyDinhTienThuTienNo;
                if (_quyDinhTienThuTienNo == true)
                    NoiDung = "Đang áp dụng";
                else
                    NoiDung = "Không áp dụng";
            }
        }

        #region Binding Properties
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
                MessageBox.Show("Khách hàng không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            Application.Current.Windows.OfType<ThemPhieuThuWindow>().FirstOrDefault()?.Close();
        }

        [RelayCommand]
        private async Task LapPhieuThu()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SoDienThoai) || SoDienThoai.Length != 10)
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại đúng định dạng (10 số).", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var khachHang = KhachHanges?.FirstOrDefault(kh => kh.DienThoai == SoDienThoai);
                if (khachHang == null)
                {
                    MessageBox.Show("Khách hàng không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrEmpty(MaPhieuThu))
                {
                    int newId = await _phieuThuService.GenerateAvailableId();
                    MaPhieuThu = newId.ToString();
                }

                var thamso = await _thamSoService.GetThamSo();
                var QuyDinhTienThuTienNo = thamso.QuyDinhTienThuTienNo;


                if (QuyDinhTienThuTienNo && SoTienThu > khachHang.TienNo)
                {
                    MessageBox.Show("Số tiền thu không được lớn hơn số tiền nợ của khách hàng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var phieuThu = new PhieuThu
                {
                    MaPhieuThu = int.Parse(MaPhieuThu),
                    MaKhachHang = khachHang.MaKhachHang,
                    NgayThu = NgayThuTien,
                    SoTienThu = SoTienThu
                };

                await _phieuThuService.AddPhieuThu(phieuThu);
                khachHang.TienNo -= SoTienThu;
                await _khachHangService.UpdateKhachHang(khachHang);

                MessageBox.Show($"Lập phiếu thu thành công. Mã phiếu thu: {MaPhieuThu}",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi lập phiếu thu: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void TimKhachHang()
        {
            try
            {
                var window = _serviceProvider.GetRequiredService<KhachHangHoaDonWindow>();
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ thêm phiếu thu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
