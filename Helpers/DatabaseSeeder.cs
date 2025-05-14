using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaSach.Helpers
{
    public static class DatabaseSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            SeedThamSo(modelBuilder);
            SeedSach(modelBuilder);
            SeedPhieuNhapSach(modelBuilder);
            SeedChiTietPhieuNhap(modelBuilder);
            SeedHoaDonBanSach(modelBuilder);
            SeedChiTietHoaDon(modelBuilder);
            SeedKhachHang(modelBuilder);
            SeedPhieThu(modelBuilder);
        }

        private static void SeedPhieThu(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private static void SeedKhachHang(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private static void SeedChiTietHoaDon(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private static void SeedHoaDonBanSach(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private static void SeedChiTietPhieuNhap(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private static void SeedPhieuNhapSach(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private static void SeedSach(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private static void SeedThamSo(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();

        }
    }
}
