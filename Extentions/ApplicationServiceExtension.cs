﻿using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Configs;
using QuanLyNhaSach.Helpers;
using QuanLyNhaSach.Repositories;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.ViewModels.SachViewModel;
using QuanLyNhaSach.ViewModels.BaoCaoViewModel;
using QuanLyNhaSach.ViewModels.KhachHangViewModel;
using QuanLyNhaSach.ViewModels.KhachHangHoaDonViewModel;
using QuanLyNhaSach.ViewModels.PhieuThuViewModel;
using QuanLyNhaSach.ViewModels.HoaDonBanViewModel;
using QuanLyNhaSach.Views.BaoCaoViews;

namespace QuanLyNhaSach.Extentions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            // Register database
            services.AddSingleton<DatabaseConfig>();

            // Register repositories and services
            services.AddScoped<ISachService, SachRepository>();
            services.AddScoped<IChiTietHoaDonService, ChiTietHoaDonRepository>();
            services.AddScoped<IChiTietPhieuNhapService, ChiTietPhieuNhapRepository>();
            services.AddScoped<IHoaDonService, HoaDonRepository>();
            services.AddScoped<IKhachHangService, KhachHangRepository>();
            services.AddScoped<IPhieuNhapSachService, PhieuNhapSachRepository>();
            services.AddScoped<IPhieuThuService, PhieuThuRepository>();
            services.AddScoped<IThamSoService, ThamSoRepository>();

            // Register helpers
            services.AddSingleton<ComboBoxItemConverter>();

            // Register ViewModels
            services.AddTransient<SachPageViewModel>();
            services.AddTransient<CapNhatSachViewModel>();
            services.AddTransient<ThemSachViewModel>();
            services.AddTransient<TraCuuSachViewModel>();

            services.AddTransient<BaoCaoChiTietViewModel>();
            services.AddTransient<BaoCaoCongNoViewModel>();
            services.AddTransient<BaoCaoTonSachViewModel>();

            services.AddTransient<KhachHangViewModel>();
            services.AddTransient<CapNhatKhachHangViewModel>();
            services.AddTransient<ThemKhachHangViewModel>();
            services.AddTransient<TraCuuKhachHangViewModel>();

            services.AddTransient<KhachHangHoaDonWindowViewModel>();

            services.AddTransient<PhieuThuPageViewModel>();
            services.AddTransient<ThemPhieuThuWindowViewModel>();
            services.AddTransient<TraCuuPhieuThuWindowViewModel>();
            services.AddTransient<CapNhatPhieuThuViewModel>();

            services.AddTransient<HoaDonBanPageViewModel>();
            services.AddTransient<ThemHoaDonBanViewModel>();
            services.AddTransient<CapNhatHoaDonBanViewModel>();
            services.AddTransient<TraCuuHoaDonBanViewModel>();

            services.AddTransient<ViewModels.ThamSoViewModel.ThamSoPageViewModel>();

            services.AddTransient<ViewModels.PhieuNhapSachViewModel.MainWindowViewModel>();
            services.AddTransient<ViewModels.PhieuNhapSachViewModel.LapPhieuNhapSachViewModel>();
            services.AddTransient<ViewModels.PhieuNhapSachViewModel.CapNhatPhieuNhapSachViewModel>();
            services.AddTransient<ViewModels.PhieuNhapSachViewModel.TraCuuPhieuNhapSachViewModel>();

            // Register Views
            services.AddTransient<Views.TraCuuPhieuNhapSachWindow>();
            services.AddTransient<Views.MainWindow>();
            services.AddTransient<Views.LapPhieuNhapSachWindow>();
            services.AddTransient<Views.CapNhatPhieuNhapSachWindow>();

            // Register Page Views
            services.AddTransient<Views.SachViews.SachPage>();
            services.AddTransient<Views.SachViews.CapNhatSachWindow>();
            services.AddTransient<Views.SachViews.ThemSachWindow>();
            services.AddTransient<Views.SachViews.TraCuuSachWindow>();

            services.AddTransient<Views.KhachHangViews.KhachHangPage>();
            services.AddTransient<Views.KhachHangViews.CapNhatKhachHangWindow>();
            services.AddTransient<Views.KhachHangViews.ThemKhachHangWindow>();
            services.AddTransient<Views.KhachHangViews.TraCuuKhachHangWindow>();

            services.AddTransient<Views.KhachHangHoaDonViews.KhachHangHoaDonWindow>();

            services.AddTransient<Views.PhieuThuViews.PhieuThuPage>();
            services.AddTransient<Views.PhieuThuViews.ThemPhieuThuWindow>();
            services.AddTransient<Views.PhieuThuViews.TraCuuPhieuThuWindow>();
            services.AddTransient<Views.PhieuThuViews.CapNhatPhieuThuWindow>();

            services.AddTransient<Views.HoaDonBanViews.HoaDonBanPage>();
            services.AddTransient<Views.HoaDonBanViews.CapNhatHoaDonBanWindow>();
            services.AddTransient<Views.HoaDonBanViews.ThemHoaDonBanWindow>();
            services.AddTransient<Views.HoaDonBanViews.TraCuuHoaDonBanWindow>();

            services.AddTransient<Views.ThamSoViews.ThamSoPage>();

            services.AddTransient<Views.BaoCaoViews.BaoCaoChiTietPage>();
            services.AddTransient<Views.BaoCaoViews.BaoCaoCongNoWindow>();
            services.AddTransient<Views.BaoCaoViews.BaoCaoTonWindow>();

            services.AddTransient<Views.CustomAnimation.GridLengthAnimation>();



            // Register navigation service
            services.AddSingleton<INavigationService, NavigationService>();


            return services;
        }
    }
}
