using Microsoft.EntityFrameworkCore;
using QuanLyNhaSach.Models;

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
            modelBuilder.Entity<PhieuThu>().HasData(
                new PhieuThu
                {
                    MaPhieuThu = 1,
                    MaKhachHang = 1,
                    NgayThu = DateTime.Now,
                    SoTienThu = 1000000
                }
            );
        }

        private static void SeedKhachHang(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KhachHang>().HasData(
                new KhachHang
                {
                    MaKhachHang = 1,
                    TenKhachHang = "Nguyễn Anh Đặng",
                    DiaChi = "12 An Dương Vương",
                    DienThoai = "0912345678",
                    Email = "nguyen.anhdang1@example.com",
                    TienNo = 5000000L
                },
                new KhachHang
                {
                    MaKhachHang = 2,
                    TenKhachHang = "Trần Bích Hạnh",
                    DiaChi = "34 Bạch Đằng",
                    DienThoai = "0923456789",
                    Email = "tran.bichhanh2@example.com",
                    TienNo = 6000000L
                },
                new KhachHang
                {
                    MaKhachHang = 3,
                    TenKhachHang = "Lê Cường Minh",
                    DiaChi = "56 Cách Mạng Tháng 8",
                    DienThoai = "0934567890",
                    Email = "le.cuongminh3@example.com",
                    TienNo = 7000000L
                },
                new KhachHang
                {
                    MaKhachHang = 4,
                    TenKhachHang = "Ngô Dương Quang",
                    DiaChi = "78 Đặng Văn Ngữ",
                    DienThoai = "0945678901",
                    Email = "ngo.duongquang4@example.com",
                    TienNo = 8000000L
                },
                new KhachHang
                {
                    MaKhachHang = 5,
                    TenKhachHang = "Phạm Hà Vy",
                    DiaChi = "90 Hàn Thuyên",
                    DienThoai = "0956789012",
                    Email = "pham.havy5@example.com",
                    TienNo = 9000000L
                },
                new KhachHang
                {
                    MaKhachHang = 6,
                    TenKhachHang = "Phan Hải Long",
                    DiaChi = "21 Hoàng Diệu",
                    DienThoai = "0967890123",
                    Email = "phan.hailong6@example.com",
                    TienNo = 10000000L
                },
                new KhachHang
                {
                    MaKhachHang = 7,
                    TenKhachHang = "Vũ Hương Giang",
                    DiaChi = "43 Huỳnh Thúc Kháng",
                    DienThoai = "0978901234",
                    Email = "vu.huonggiang7@example.com",
                    TienNo = 11000000L
                },
                new KhachHang
                {
                    MaKhachHang = 8,
                    TenKhachHang = "Võ Khoa Nam",
                    DiaChi = "65 Kim Mã",
                    DienThoai = "0989012345",
                    Email = "vo.khoanam8@example.com",
                    TienNo = 12000000L
                },
                new KhachHang
                {
                    MaKhachHang = 9,
                    TenKhachHang = "Trần Linh Chi",
                    DiaChi = "87 Lê Lợi",
                    DienThoai = "0990123456",
                    Email = "tran.linhchi9@example.com",
                    TienNo = 13000000L
                },
                new KhachHang
                {
                    MaKhachHang = 10,
                    TenKhachHang = "Phạm Minh Tuấn",
                    DiaChi = "109 Nguyễn Huệ",
                    DienThoai = "0901234567",
                    Email = "pham.minhtuan10@example.com",
                    TienNo = 14000000L
                }

);
        }

        private static void SeedChiTietHoaDon(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChiTietHoaDon>().HasData(
                new ChiTietHoaDon
                {
                    MaChiTietHoaDon = 1,
                    MaHoaDon = 1001,
                    MaSach = 1,
                    SoLuongBan = 3,
                    DonGiaBan = 150000
                }
            );
        }

        private static void SeedHoaDonBanSach(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HoaDon>().HasData(
                new HoaDon
                {
                    MaHoaDon = 1001,
                    MaKhachHang = 1,
                    NgayLap = DateTime.Now,
                    TongTien = 450000
                }
            );
        }

        private static void SeedChiTietPhieuNhap(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChiTietPhieuNhap>().HasData(
                new ChiTietPhieuNhap
                {
                    MaChiTietPhieuNhap = 1,
                    MaPhieuNhapSach = 1,
                    MaSach = 1,
                    SoLuongNhap = 5
                }
            );
        }

        private static void SeedPhieuNhapSach(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhieuNhapSach>().HasData(
                new PhieuNhapSach
                {
                    MaPhieuNhapSach = 1,
                    NgayNhap = DateTime.Now
                }
            );
        }

        private static void SeedSach(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sach>().HasData(
                new Sach
                {
                    MaSach = 1,
                    TenSach = "Lập Trình C# Cơ Bản",
                    TheLoai = "Công Nghệ Thông Tin",
                    TacGia = "Nguyễn Văn A",
                    SoLuongTon = 10
                }
            );
        }

        private static void SeedThamSo(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ThamSo>().HasData(
                new ThamSo
                {
                    Id = 1,
                    SoLuongNhapToiThieu = 0,
                    SoLuongTonToiDa = 1,
                    SoLuongTonToiThieu = 2,
                    TienNoToiDa = 3,
                    QuyDinhTienThuTienNo = true
                }
            );

        }
    }
}