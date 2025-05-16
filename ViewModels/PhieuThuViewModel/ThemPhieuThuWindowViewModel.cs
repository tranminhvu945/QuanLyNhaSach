using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.PhieuThuViews;

namespace QuanLyNhaSach.ViewModels.PhieuThuViewModel
{
    public partial class ThemPhieuThuWindowViewModel : ObservableObject
    {
        // Services
        private readonly IPhieuThuService _phieuThuService;
        private readonly IKhachHangService _khachHangService;
        private readonly IThamSoService _thamSoService;

        public ThemPhieuThuWindowViewModel(
            IPhieuThuService phieuThuService,
            IKhachHangService khachHangService,
            IThamSoService thamSoService
)
        {
            _phieuThuService = phieuThuService;
            _khachHangService = khachHangService;
            _thamSoService = thamSoService;

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var listKhachHang = await _khachHangService.GetAllKhachHang();
            KhachHanges = [.. listKhachHang];
            if (KhachHanges.Count() > 0)
            {
                SelectedKhachHang = KhachHanges.First();
                SoDienThoai = SelectedKhachHang.DienThoai;
                Email = SelectedKhachHang.Email;
                DiaChi = SelectedKhachHang.DiaChi;
                TienNo = SelectedKhachHang.TienNo;
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

        private KhachHang _selectedKhachHang = null!;
        public KhachHang SelectedKhachHang
        {
            get => _selectedKhachHang;
            set
            {
                SetProperty(ref _selectedKhachHang, value);
                if (_selectedKhachHang != null)
                {
                    SoDienThoai = _selectedKhachHang.DienThoai;
                    Email = _selectedKhachHang.Email;
                    DiaChi = _selectedKhachHang.DiaChi;
                    TienNo = _selectedKhachHang.TienNo;
                }
            }
        }
        [ObservableProperty]
        private string _soDienThoai = string.Empty;
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
                if (SelectedKhachHang == null)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrEmpty(MaPhieuThu))
                {
                    int newId = await _phieuThuService.GenerateAvailableId();
                    MaPhieuThu = newId.ToString();
                }

                //var thamso = await _thamSoService.GetThamSo();
                //var QuyDinhTienThuTienNo = thamso.QuyDinhTienThuTienNo;
                //if (QuyDinhTienThuTienNo && SoTienThu > TienNo)
                //{
                //    MessageBox.Show("Số tiền thu không được lớn hơn số tiền nợ của đại lý", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                //    return;
                //}

                var phieuThu = new PhieuThu
                {
                    MaPhieuThu = int.Parse(MaPhieuThu),
                    MaKhachHang = SelectedKhachHang.MaKhachHang,
                    NgayThu = NgayThuTien,
                    SoTienThu = SoTienThu
                };

                await _phieuThuService.AddPhieuThu(phieuThu);
                await _khachHangService.UpdateKhachHang(SelectedKhachHang);

                MessageBox.Show($"Lập phiếu thu thành công. Mã phiếu thu: {MaPhieuThu}",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi lập phiếu thu: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    #endregion
}
