using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore.Diagnostics;
using QuanLyDaiLy.Messages;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.BaoCaoViews;

namespace QuanLyNhaSach.ViewModels.BaoCaoViewModel
{
    public partial class BaoCaoCongNoViewModel : ObservableObject, IRecipient<SelectedDateMessage>
    {
        public BaoCaoCongNoViewModel()
        {
            WeakReferenceMessenger.Default.RegisterAll(this);

            // Khởi tạo danh sách tháng
            MonthOptions = new ObservableCollection<string>
            {
                "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4",
                "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8",
                "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
            };

            // Khởi tạo danh sách năm
            int currentYear = DateTime.Now.Year;
            YearOptions = new ObservableCollection<int>();
            for (int i = currentYear - 4; i <= currentYear; i++)
            {
                YearOptions.Add(i);
            }

        }

        public ObservableCollection<string> MonthOptions { get; }
        public ObservableCollection<int> YearOptions { get; }

        [ObservableProperty]
        private string _selectedMonth;
        partial void OnSelectedMonthChanged(string value)
        {
        }

        [ObservableProperty]
        private int _selectedYear;
        partial void OnSelectedYearChanged(int value)
        {
        }

        [RelayCommand]
        private void Close()
        {
            WeakReferenceMessenger.Default.Send(new DataReloadMessage());
            Application.Current.Windows.OfType<BaoCaoCongNoWindow>().FirstOrDefault()?.Close();
        }

        public void Receive(SelectedDateMessage message)
        {
            (int month, int year) = message.Value;
            SelectedMonth = $"Tháng {month}";
            SelectedYear = year;
        }
    }
}
