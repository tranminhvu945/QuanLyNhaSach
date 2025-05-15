using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using QuanLyDaiLy.Messages;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.BaoCaoViews;

namespace QuanLyNhaSach.ViewModels.BaoCaoViewModel
{
    public partial class BaoCaoChiTietViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        public BaoCaoChiTietViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            WeakReferenceMessenger.Default.RegisterAll(this);
            InitializeMonthYearOptions();
        }

        public void InitializeMonthYearOptions()
        {
            var currentDate = DateTime.Now;
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;

            MonthOptions =
            [
                "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4",
                "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8",
                "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
            ];

            YearOptions = [];
            for (int i = currentYear - 4; i <= currentYear; i++)
            {
                YearOptions.Add(i);
            }

            SelectedTonSachMonth = MonthOptions[currentMonth - 1];
            SelectedTonSachYear = currentYear;
            SelectedCongNoMonth = MonthOptions[currentMonth - 1];
            SelectedCongNoYear = currentYear;
        }

        [ObservableProperty]
        private List<string> _monthOptions = [];

        [ObservableProperty]
        private List<int> _yearOptions = [];

        [ObservableProperty]
        private string _selectedTonSachMonth = $"Tháng {DateTime.Now.Month}";

        partial void OnSelectedTonSachMonthChanged(string value)
        {
            //_ = UpdateDoanhSoData();
        }

        [ObservableProperty]
        private int _selectedTonSachYear = DateTime.Now.Year;
        partial void OnSelectedTonSachYearChanged(int value)
        {
            //_ = UpdateDoanhSoData();
        }

        [ObservableProperty]
        private string _selectedCongNoMonth = $"Tháng {DateTime.Now.Month}";

        partial void OnSelectedCongNoMonthChanged(string value)
        {
            //_ = UpdateCongNoData();
        }

        [ObservableProperty]
        private int _selectedCongNoYear = DateTime.Now.Year;

        partial void OnSelectedCongNoYearChanged(int value)
        {
            //_ = UpdateCongNoData();
        }

        [RelayCommand]
        private void TonSach()
        {
            try
            {
                var window = _serviceProvider.GetRequiredService<BaoCaoTonWindow>();
                int month = int.Parse(SelectedTonSachMonth.Replace("Tháng ", ""));
                int year = SelectedTonSachYear;
                WeakReferenceMessenger.Default.Send(new SelectedDateMessage(month, year));
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ báo cáo tồn sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void CongNo()
        {
            try
            {
                var window = _serviceProvider.GetRequiredService<BaoCaoCongNoWindow>();
                int month = int.Parse(SelectedCongNoMonth.Replace("Tháng ", ""));
                int year = SelectedCongNoYear;
                WeakReferenceMessenger.Default.Send(new SelectedDateMessage(month, year));
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ báo cáo công nợ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
