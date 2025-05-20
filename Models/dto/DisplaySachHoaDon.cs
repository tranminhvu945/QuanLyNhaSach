using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuanLyNhaSach.Models.dto
{
    public class DisplaySachHoaDon : INotifyPropertyChanged
    {
        public DisplaySachHoaDon(
            IEnumerable<Sach> danhSachSach)
        {
            DanhSachSach = [.. danhSachSach];
            if (DanhSachSach.Count > 0)
                SelectedSach = DanhSachSach[0];
        }

        #region Bindings Properties
        private ObservableCollection<Sach> _danhSachSach = [];
        public ObservableCollection<Sach> DanhSachSach
        {
            get => _danhSachSach;
            set
            {
                _danhSachSach = value;
                OnPropertyChanged();
            }
        }

        private Sach _selectedSach = null!;
        public Sach SelectedSach
        {
            get => _selectedSach;
            set
            {
                if (_selectedSach != value)
                {
                    var oldSach = _selectedSach;
                    _selectedSach = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TheLoai));
                    //OnPropertyChanged(nameof(QuyDinhSoLuongTonToiThieu));
                    OnPropertyChanged(nameof(SoLuongTon));
                    OnPropertyChanged(nameof(SoLuongTonTruocKhiXuat));
                    OnPropertyChanged(nameof(ThanhTien));
                    ThanhTienChanged?.Invoke(this, EventArgs.Empty);

                    // Gọi callback hoặc event để cập nhật danh sách đã chọn/chưa chọn
                    SelectedSachChanged?.Invoke(this, new SelectedSachChangedEventArgs(oldSach, _selectedSach));
                }                    
            }
        }

        public int SoLuongTonTruocKhiXuat => SelectedSach?.SoLuongTon + SoLuongBan ?? 0;

        public string TheLoai => SelectedSach?.TheLoai ?? string.Empty;

        //public int QuyDinhSoLuongTonToiThieu => SelectedSach?.QuyDinhSoLuongTonToiThieu ?? 0;

        public int SoLuongTon => SelectedSach?.SoLuongTon ?? 0;

        private int _soLuongBan = 0;
        public int SoLuongBan
        {
            get => _soLuongBan;
            set
            {
                _soLuongBan = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ThanhTien));
                ThanhTienChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private long _donGiaBan = 0;
        public long DonGiaBan
        {
            get => _donGiaBan;
            set
            {
                _donGiaBan = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ThanhTien));
                ThanhTienChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion
        public long ThanhTien => SoLuongBan * DonGiaBan;

        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler<SelectedSachChangedEventArgs> SelectedSachChanged;
        // Add an event to notify when ThanhTien changes
        public event EventHandler? ThanhTienChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
