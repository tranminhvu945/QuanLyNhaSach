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

            // Lấy style từ ResourceDictionary
            _readOnlyStyle = (Style)Application.Current.Resources["ReadOnlyTextBoxStyle"];
            _editStyle = (Style)Application.Current.Resources["StandardTextBoxStyle"];

            // Đặt style mặc định
            TextBoxStyle = _readOnlyStyle;

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
                    SoLuongNhapToiThieu = thamSo.SoLuongNhapToiThieu.ToString();
                    SoLuongTonToiThieu = thamSo.SoLuongTonToiThieu.ToString();
                    SoLuongTonToiDa = thamSo.SoLuongTonToiDa.ToString();
                    TienNoToiDa = thamSo.TienNoToiDa.ToString();

                    QuyDinhSoLuongNhapToiThieu = thamSo.QuyDinhSoLuongNhapToiThieu;
                    QuyDinhSoLuongTonToiThieu = thamSo.QuyDinhSoLuongTonToiThieu;
                    QuyDinhSoLuongTonToiDa = thamSo.QuyDinhSoLuongTonToiDa;
                    QuyDinhTienNoToiDa = thamSo.QuyDinhTienNoToiDa;
                    QuyDinhTienThuTienNo = thamSo.QuyDinhTienThuTienNo;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi khi tải tham số từ cơ sở dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #region Binding Styles
        private Style _readOnlyStyle;
        private Style _editStyle;

        // Binding property IsEditing
        private bool _isEditing = false;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (SetProperty(ref _isEditing, value))
                {
                    OnPropertyChanged(nameof(IsReadOnly));
                    UpdateTextBoxStyle();
                }
            }
        }

        public bool IsReadOnly => !IsEditing;

        // Binding Style cho TextBox
        private Style _textBoxStyle = null!;
        public Style TextBoxStyle
        {
            get => _textBoxStyle;
            private set => SetProperty(ref _textBoxStyle, value);
        }

        private void UpdateTextBoxStyle()
        {
            TextBoxStyle = IsEditing ? _editStyle : _readOnlyStyle;
        }
        #endregion

        private string _buttonText = " Sửa tham số";
        public string ButtonText
        {
            get => _buttonText;
            set => SetProperty(ref _buttonText, value);
        }

        #region Binding Properties
        private string _soLuongNhapToiThieu = string.Empty;
        public string SoLuongNhapToiThieu
        {
            get => _soLuongNhapToiThieu;
            set => SetProperty(ref _soLuongNhapToiThieu, value);
        }

        private string _soLuongTonToiThieu = string.Empty;
        public string SoLuongTonToiThieu
        {
            get => _soLuongTonToiThieu;
            set => SetProperty(ref _soLuongTonToiThieu, value);
        }

        private string _soLuongTonToiDa = string.Empty;
        public string SoLuongTonToiDa
        {
            get => _soLuongTonToiDa;
            set => SetProperty(ref _soLuongTonToiDa, value);
        }

        private string _tienNoToiDa = string.Empty;
        public string TienNoToiDa
        {
            get => _tienNoToiDa;
            set => SetProperty(ref _tienNoToiDa, value);
        }

        [ObservableProperty]
        private bool _quyDinhSoLuongNhapToiThieu = true;
        [ObservableProperty]
        private bool _quyDinhSoLuongTonToiThieu = true;
        [ObservableProperty]
        private bool _quyDinhSoLuongTonToiDa = true;
        [ObservableProperty]
        private bool _quyDinhTienNoToiDa = true;
        [ObservableProperty]
        private bool _quyDinhTienThuTienNo = true;
        #endregion

        #region RelayCommands
        [RelayCommand]
        private async Task<bool> SaveChanges()
        {
            if (!ValidateInputs(out int soLuongNhapToiThieu,
                                out int soLuongTonToiThieu,
                                out int soLuongTonToiDa,
                                out int tienNoToiDa))
            {
                return false;
            }

            try
            {
                var thamSo = await _thamSoService.GetThamSo();
                if (thamSo == null)
                {
                    MessageBox.Show("Không tìm thấy tham số để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // Gán giá trị số sau khi parse
                thamSo.SoLuongNhapToiThieu = soLuongNhapToiThieu;
                thamSo.SoLuongTonToiThieu = soLuongTonToiThieu;
                thamSo.SoLuongTonToiDa = soLuongTonToiDa;
                thamSo.TienNoToiDa = tienNoToiDa;

                thamSo.QuyDinhSoLuongNhapToiThieu = QuyDinhSoLuongNhapToiThieu;
                thamSo.QuyDinhSoLuongTonToiThieu = QuyDinhSoLuongTonToiThieu;
                thamSo.QuyDinhSoLuongTonToiDa = QuyDinhSoLuongTonToiDa;
                thamSo.QuyDinhTienNoToiDa = QuyDinhTienNoToiDa;

                await _thamSoService.UpdateThamSo(thamSo);
                MessageBox.Show("Cập nhật tham số thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật tham số: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private bool ValidateInputs(out int soLuongNhap, out int soLuongTonMin, out int soLuongTonMax, out int tienNoMax)
        {
            soLuongNhap = soLuongTonMin = soLuongTonMax = tienNoMax = 0;

            if (string.IsNullOrWhiteSpace(SoLuongNhapToiThieu))
            {
                MessageBox.Show("Số lượng nhập tối thiểu không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(SoLuongNhapToiThieu, out soLuongNhap) || soLuongNhap <= 0)
            {
                MessageBox.Show("Số lượng nhập tối thiểu phải là số nguyên lớn hơn 0.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(SoLuongTonToiThieu))
            {
                MessageBox.Show("Số lượng tồn tối thiểu không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(SoLuongTonToiThieu, out soLuongTonMin) || soLuongTonMin < 0)
            {
                MessageBox.Show("Số lượng tồn tối thiểu phải là số nguyên lớn hơn hoặc bằng 0.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(SoLuongTonToiDa))
            {
                MessageBox.Show("Số lượng tồn tối đa không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(SoLuongTonToiDa, out soLuongTonMax) || soLuongTonMax < 0)
            {
                MessageBox.Show("Số lượng tồn tối đa phải là số nguyên lớn hơn hoặc bằng 0.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TienNoToiDa))
            {
                MessageBox.Show("Tiền nợ tối đa không được để trống.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(TienNoToiDa, out tienNoMax) || tienNoMax < 0)
            {
                MessageBox.Show("Tiền nợ tối đa phải là số nguyên lớn hơn hoặc bằng 0.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        [RelayCommand]
        private async Task EditOrSaveAsync()
        {
            if (!IsEditing)
            {
                IsEditing = true;
                ButtonText = "Lưu thay đổi";
            }
            else
            {
                await SaveChanges();

                IsEditing = false;
                ButtonText = "Sửa tham số";
            }
        }
        #endregion
    }
}