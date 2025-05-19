using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaSach.Models.dto
{
    public class DisplayDauSachPhieuNhap : INotifyPropertyChanged
    {
        public DisplayDauSachPhieuNhap(IEnumerable<Sach> danhSachDauSach)
        {
            DanhSachSach = [.. danhSachDauSach];
            if (DanhSachSach.Count > 0)
                SelectedSach = DanhSachSach[0];
            //_parentList = parentList;
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
                _selectedSach = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TenSach));
                OnPropertyChanged(nameof(SoLuongTon));
                ThanhTienChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string TenSach => SelectedSach?.TenSach ?? string.Empty;

        public int SoLuongTon => SelectedSach?.SoLuongTon ?? 0;

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


        public event PropertyChangedEventHandler? PropertyChanged;
        // Add an event to notify when ThanhTien changes
        public event EventHandler? ThanhTienChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
