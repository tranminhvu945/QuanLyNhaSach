using QuanLyNhaSach.Services;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Views.ThamSoViews;
namespace QuanLyNhaSach.ViewModels.ThamSoViewModel
{
    public partial class ThamSoPageViewModel :
        ObservableObject,
        IRecipient<DataReloadMessage>
    {
        // Services
        private readonly IThamSoService _thamsoService;
        private readonly IServiceProvider _serviceProvider;
        bool isFirst = true;

        public ThamSoPageViewModel(
            IThamSoService thamSoService,
            IServiceProvider serviceProvider
        )
        {
            _thamsoService = thamSoService;
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            // Load tham số từ cơ sở dữ liệu
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
                var thamSo = await _thamsoService.GetThamSo();
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
                var window = _serviceProvider.GetRequiredService<CapNhatThamSoWindow>();
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ chỉnh sửa tham số: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SaveChangesToDatabase()
        {
            try
            {
                if (isFirst)
                {
                    var thamSo = await _thamsoService.GetThamSo();
                    if (thamSo != null)
                    {
                        SoLuongNhapToiThieu = thamSo.SoLuongNhapToiThieu;
                        SoLuongTonToiThieu = thamSo.SoLuongTonToiThieu;
                        SoLuongTonToiDa = thamSo.SoLuongTonToiDa;
                        TienNoToiDa = thamSo.TienNoToiDa;
                        QuyDinhTienThuTienNo = thamSo.QuyDinhTienThuTienNo;
                    }
                    isFirst = false;
                }
                else
                {
                    var thamSo = await _thamsoService.GetThamSo();

                    thamSo.SoLuongNhapToiThieu = SoLuongNhapToiThieu;
                    thamSo.SoLuongTonToiThieu = SoLuongTonToiThieu;
                    thamSo.SoLuongTonToiDa = SoLuongTonToiDa;
                    thamSo.TienNoToiDa = TienNoToiDa;
                    thamSo.QuyDinhTienThuTienNo = QuyDinhTienThuTienNo;

                    await _thamsoService.UpdateThamSo(thamSo);
                    //await Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi khi lưu tham số vào cơ sở dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}