﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QuanLyNhaSach.ViewModels.PhieuNhapSachViewModel;
using QuanLyNhaSach.ViewModels.PhieuThuViewModel;

namespace QuanLyNhaSach.Views
{
    /// <summary>
    /// Interaction logic for LapPhieuNhapSachWindow.xaml
    /// </summary>
    public partial class LapPhieuNhapSachWindow : Window
    {
        public LapPhieuNhapSachWindow(LapPhieuNhapSachViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
