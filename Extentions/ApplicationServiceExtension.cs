using Microsoft.Extensions.DependencyInjection;
using QuanLyNhaSach.Configs;
using QuanLyNhaSach.Helpers;
using QuanLyNhaSach.Repositories;
using QuanLyNhaSach.Services;

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


            // Register Views

            // Register navigation service
            services.AddSingleton<INavigationService, NavigationService>();


            return services;
        }
    }
}
