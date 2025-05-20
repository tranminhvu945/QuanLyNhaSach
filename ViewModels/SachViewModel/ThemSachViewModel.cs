using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Commands;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.SachViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuanLyNhaSach.ViewModels.SachViewModel
{
    public class ThemSachViewModel : INotifyPropertyChanged
    {
        private readonly ISachService _sachService;

        public ThemSachViewModel(ISachService sachService)
        {
            _sachService = sachService;

            // Initialize commands
            CloseWindowCommand = new RelayCommand(CloseWindow);
            TiepNhanSachCommand = new RelayCommand(async () => await TiepNhanSach());
            SachMoiCommand = new RelayCommand(SachMoi);
        }

        
        private string _maSach = "";
        public string MaSach
        {
            get => _maSach;
            set
            {
                _maSach = value;
                OnPropertyChanged();
            }
        }

        private string _tenSach = "";
        public string TenSach
        {
            get => _tenSach;
            set
            {
                _tenSach = value;
                OnPropertyChanged();
            }
        }

        private string _theLoai = "";
        public string TheLoai
        {
            get => _theLoai;
            set
            {
                _theLoai = value;
                OnPropertyChanged();
            }
        }

        private string _tacGia = "";
        public string TacGia
        {
            get => _tacGia;
            set
            {
                _tacGia = value;
                OnPropertyChanged();
            }
        }

        private int _soLuongTon = 0;
        public int SoLuongTon
        {
            get => _soLuongTon;
            set
            {
                _soLuongTon = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand CloseWindowCommand { get; }
        public ICommand TiepNhanSachCommand { get; }
        public ICommand SachMoiCommand { get; }

        private void CloseWindow()
        {
            WeakReferenceMessenger.Default.Send(new DataReloadMessage());
            Application.Current.Windows.OfType<ThemSachWindow>().FirstOrDefault()?.Close();
        }
        
        private async Task TiepNhanSach()
        {
            if (string.IsNullOrWhiteSpace(TenSach))
            {
                MessageBox.Show("Tên đầu sách không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TheLoai))
            {
                MessageBox.Show("Thể loại không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TacGia))
            {
                MessageBox.Show("Tác giả không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SoLuongTon < 0)
            {
                MessageBox.Show("Số lượng tồn không được âm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                MaSach = (await _sachService.GenerateAvailableId()).ToString();

                Sach Sach = new()
                {
                    MaSach = int.Parse(MaSach),
                    TenSach = TenSach,
                    TheLoai = TheLoai,
                    TacGia = TacGia,
                    SoLuongTon = SoLuongTon
                };

                await _sachService.AddSach(Sach);
                MessageBox.Show("Thêm đầu sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // → Đóng cửa sổ (nếu muốn)
                CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm đầu sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SachMoi()
        {
            try
            {
                TenSach = string.Empty;
                TheLoai = string.Empty;
                TacGia = string.Empty;
                SoLuongTon = 0;
                MaSach = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo đầu sách mới: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event to notify parent view when data changes
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
