using QuanLyNhaSach.Services;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using CommunityToolkit.Mvvm.Input;
namespace QuanLyNhaSach.ViewModels.ThamSoViewModel
{
    public partial class ThamSoPageViewModel :
        ObservableObject,
        IRecipient<DataReloadMessage>
    {
        // Services
        private readonly IThamSoService _thamSoService;

        public ThamSoPageViewModel(
            IThamSoService thamSoService
        )
        {
            _thamSoService = thamSoService;

            WeakReferenceMessenger.Default.Register<DataReloadMessage>(this);

            _ = LoadDataAsync();
        }

        public void Receive(DataReloadMessage message)
        {
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var thamSo = await _thamSoService.GetThamSo();
                if (thamSo != null)
                {
                    SoLuongNhapToiThieu = thamSo.SoLuongNhapToiThieu;
                    SoLuongTonToiThieu = thamSo.SoLuongTonToiThieu;
                    SoLuongTonToiDa = thamSo.SoLuongTonToiDa;
                    TienNoToiDa = thamSo.TienNoToiDa;
                    QuyDinhTienThuTienNo = thamSo.QuyDinhTienThuTienNo;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi khi tải tham số từ cơ sở dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Bindings Properties
        [ObservableProperty]
        private int _soLuongNhapToiThieu = 0;
        [ObservableProperty]
        private int _soLuongTonToiThieu = 0;
        [ObservableProperty]
        private int _soLuongTonToiDa = 0;
        [ObservableProperty]
        private int _tienNoToiDa = 0;
        [ObservableProperty]
        private bool _quyDinhTienThuTienNo = true;
        #endregion

        #region RelayCommands
        [RelayCommand]
        private void EditThamSo()
        {
            try
            {
                _ = SaveChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật tham số: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SaveChange()
        {
            try
            {
                var thamSo = await _thamSoService.GetThamSo();

                thamSo.SoLuongNhapToiThieu = SoLuongNhapToiThieu;
                thamSo.SoLuongTonToiThieu = SoLuongTonToiThieu;
                thamSo.SoLuongTonToiDa = SoLuongTonToiDa;
                thamSo.TienNoToiDa = TienNoToiDa;
                thamSo.QuyDinhTienThuTienNo = QuyDinhTienThuTienNo;

                await _thamSoService.UpdateThamSo(thamSo);
                MessageBox.Show("Cập nhật tham số thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi khi lưu tham số vào cơ sở dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}