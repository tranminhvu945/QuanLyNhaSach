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
            DanhSachSach = new ObservableCollection<Sach>(danhSachSach);
            if (DanhSachSach.Count > 0)
                SelectedSach = DanhSachSach[0];
        }

        #region Bindings Properties
        private ObservableCollection<Sach> _danhSachSach = new ObservableCollection<Sach>();
        public ObservableCollection<Sach> DanhSachSach
        {
            get => _danhSachSach;
            set
            {
                if (_danhSachSach != value)
                {
                    _danhSachSach = value;
                    OnPropertyChanged();
                }
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
                    OnPropertyChanged(nameof(SoLuongTon));
                    OnPropertyChanged(nameof(SoLuongTonTruocKhiXuat));
                    OnPropertyChanged(nameof(ThanhTien));
                    ThanhTienChanged?.Invoke(this, EventArgs.Empty);

                    SelectedSachChanged?.Invoke(this, new SelectedSachChangedEventArgs(oldSach, _selectedSach));
                }
            }
        }

        public int SoLuongTonTruocKhiXuat => (SelectedSach?.SoLuongTon ?? 0) + SoLuongBan;

        public string TheLoai => SelectedSach?.TheLoai ?? string.Empty;

        public int SoLuongTon => SelectedSach?.SoLuongTon ?? 0;

        private int _soLuongBan = 0;
        public int SoLuongBan
        {
            get => _soLuongBan;
            set
            {
                if (_soLuongBan != value)
                {
                    _soLuongBan = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ThanhTien));
                    ThanhTienChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private long _donGiaBan = 0;
        public long DonGiaBan
        {
            get => _donGiaBan;
            set
            {
                if (_donGiaBan != value)
                {
                    _donGiaBan = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ThanhTien));
                    ThanhTienChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public long ThanhTien => SoLuongBan * DonGiaBan;
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler<SelectedSachChangedEventArgs>? SelectedSachChanged;
        public event EventHandler? ThanhTienChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
