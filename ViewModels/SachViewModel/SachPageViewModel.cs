using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.ViewModels.SachViewModel;
using QuanLyNhaSach.Views.SachViews;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace QuanLyNhaSach.ViewModels.SachViewModel
{
    public partial class SachPageViewModel :
        ObservableObject,
        IRecipient<DataReloadMessage>
    {
        private readonly ISachService _sachService;
        private readonly IServiceProvider _serviceProvider;

        public SachPageViewModel(
            ISachService SachService,
            IServiceProvider serviceProvider)
        {
            _sachService = SachService;
            _serviceProvider = serviceProvider;

            WeakReferenceMessenger.Default.Register<DataReloadMessage>(this);

            _ = LoadDataAsync();
        }

        public void Receive(DataReloadMessage message)
        {
            _ = LoadDataAsync();
        }

        [ObservableProperty]
        private ObservableCollection<Sach> _danhSachSach = [];

        [ObservableProperty]
        private Sach _selectedSach = null!;

        private async Task LoadDataAsync()
        {
            var list = await _sachService.GetAllSach();
            DanhSachSach = [.. list];
            SelectedSach = null!;
        }

        [RelayCommand]
        private void SearchSach()
        {
            SelectedSach = null!;

            var traCuuSachWindow = _serviceProvider.GetRequiredService<TraCuuSachWindow>();

            if (traCuuSachWindow.DataContext is TraCuuSachViewModel viewModel)
            {
                viewModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(viewModel.SearchResults) && viewModel.SearchResults != null)
                    {
                        DanhSachSach = viewModel.SearchResults;
                    }
                };
            }

            traCuuSachWindow.Show();
        }

        [RelayCommand]
        private void AddSach()
        {
            try
            {
                var addSachWindow = _serviceProvider.GetRequiredService<ThemSachWindow>();
                addSachWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ thêm đầu sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            SelectedSach = null!;
        }

        [RelayCommand]
        private void EditSach()
        {
            if (SelectedSach == null || SelectedSach.MaSach == 0)
            {
                MessageBox.Show("Vui lòng chọn đầu sách để chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var window = _serviceProvider.GetRequiredService<CapNhatSachWindow>();
                window.Show();
                WeakReferenceMessenger.Default.Send(new SelectedIdMessage(SelectedSach.MaSach));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ chỉnh sửa đầu sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteSach()
        {
            if (SelectedSach == null)
            {
                MessageBox.Show("Vui lòng chọn đầu sách để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa đầu sách '{SelectedSach.MaSach}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _sachService.DeleteSach(SelectedSach.MaSach);
                    MessageBox.Show("Đã xóa đầu sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa đầu sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task LoadData()
        {
            SelectedSach = null!;
            await LoadDataAsync();
            MessageBox.Show("Tải lại danh sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
