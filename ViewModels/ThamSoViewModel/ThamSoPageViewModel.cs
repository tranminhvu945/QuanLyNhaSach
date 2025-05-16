using System.ComponentModel;
using System.Runtime.CompilerServices;
using QuanLyNhaSach.Services;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.ViewModels.ThamSoViewModel
{
    public class ThamSoPageViewModel : INotifyPropertyChanged
    {
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
            _ = Load();
        }

        #region Bindings Properties
        private bool _quyDinhSoLuongDaiLyToiDa = true;
        public bool QuyDinhSoLuongDaiLyToiDa
        {
            get => _quyDinhSoLuongDaiLyToiDa;
            set
            {
                if (_quyDinhSoLuongDaiLyToiDa != value)
                {
                    _quyDinhSoLuongDaiLyToiDa = value;
                    OnPropertyChanged();
                    _ = SaveChangesToDatabase();
                }
            }
        }

        private bool _quyDinhTienThuTienNo = true;
        public bool QuyDinhTienThuTienNo
        {
            get => _quyDinhTienThuTienNo;
            set
            {
                if (_quyDinhTienThuTienNo != value)
                {
                    _quyDinhTienThuTienNo = value;
                    OnPropertyChanged();
                    _ = SaveChangesToDatabase();
                }
            }
        }

        private int _soLuongDaiLyToiDa = 4;
        public int SoLuongDaiLyToiDa
        {
            get => _soLuongDaiLyToiDa;
            set
            {
                if (_soLuongDaiLyToiDa != value)
                {
                    _soLuongDaiLyToiDa = value;
                    OnPropertyChanged();
                    _ = SaveChangesToDatabase();
                }
            }
        }

        private async Task Load()
        {
            try
            {
                var thamSo = await _thamsoService.GetThamSo();
                if (thamSo != null)
                {
                    QuyDinhSoLuongDaiLyToiDa = thamSo.QuyDinhSoLuongDaiLyToiDa;
                    QuyDinhTienThuTienNo = thamSo.QuyDinhTienThuTienNo;
                    SoLuongDaiLyToiDa = thamSo.SoLuongDaiLyToiDa;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi khi tải tham số từ cơ sở dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức để lưu thay đổi vào cơ sở dữ liệu
        private async Task SaveChangesToDatabase()
        {
            try
            {
                if (isFirst)
                {
                    var thamSo = await _thamsoService.GetThamSo();
                    if (thamSo != null)
                    {
                        SoLuongDaiLyToiDa = thamSo.SoLuongDaiLyToiDa;
                        QuyDinhSoLuongDaiLyToiDa = thamSo.QuyDinhSoLuongDaiLyToiDa;
                        QuyDinhTienThuTienNo = thamSo.QuyDinhTienThuTienNo;
                    }
                    isFirst = false;
                }
                else
                {
                    var thamSo = await _thamsoService.GetThamSo();

                    thamSo.QuyDinhSoLuongDaiLyToiDa = QuyDinhSoLuongDaiLyToiDa;
                    thamSo.QuyDinhTienThuTienNo = QuyDinhTienThuTienNo;
                    thamSo.SoLuongDaiLyToiDa = SoLuongDaiLyToiDa;

                    await _thamsoService.UpdateThamSo(thamSo);
                    //await Load();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi khi lưu tham số vào cơ sở dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
}
