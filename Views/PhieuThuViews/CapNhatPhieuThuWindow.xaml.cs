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
using QuanLyNhaSach.ViewModels.PhieuThuViewModel;

namespace QuanLyNhaSach.Views.PhieuThuViews
{
    /// <summary>
    /// Interaction logic for CapNhatPhieuThuWindow.xaml
    /// </summary>
    public partial class CapNhatPhieuThuWindow : Window
    {
        public CapNhatPhieuThuWindow(CapNhatPhieuThuViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
