using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuanLyNhaSach.Models.dto
{
    public class DisplayDauSachPhieuNhap : INotifyPropertyChanged
    {
        public DisplayDauSachPhieuNhap(IEnumerable<Sach> danhSachDauSach)
        {
            DanhSachSach = new ObservableCollection<Sach>(danhSachDauSach);
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
                    OnPropertyChanged(nameof(SelectedSach));
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TenSach));
                    OnPropertyChanged(nameof(TacGia));
                    OnPropertyChanged(nameof(TheLoai));
                    OnPropertyChanged(nameof(SoLuongTon));
                    OnPropertyChanged(nameof(SoLuongTonTruocKhiNhap));

                    SelectedSachChanged?.Invoke(this, new SelectedSachChangedEventArgs(oldSach, _selectedSach));
                }
            }
        }

        public string TenSach => SelectedSach?.TenSach ?? string.Empty;
        public string TacGia => SelectedSach?.TacGia ?? string.Empty;
        public string TheLoai => SelectedSach?.TheLoai ?? string.Empty;

        public int SoLuongTon => SelectedSach?.SoLuongTon ?? 0;

        public int SoLuongTonTruocKhiNhap => SelectedSach?.SoLuongTon - SoLuongNhap ?? 0;


        private int _soLuongNhap = 0;
        public int SoLuongNhap
        {
            get => _soLuongNhap;
            set
            {
                _soLuongNhap = value;
                OnPropertyChanged();
            }
        }

        #endregion


        public event EventHandler<SelectedSachChangedEventArgs>? SelectedSachChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
