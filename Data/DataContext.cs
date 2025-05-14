using Microsoft.EntityFrameworkCore;
using QuanLyNhaSach.Helpers;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    /* -------------------- DbSet -------------------- */
    public DbSet<Sach> DsSach { get; set; } = null!;
    public DbSet<KhachHang> DsKhachHang { get; set; } = null!;
    public DbSet<ThamSo> DsThamSo { get; set; } = null!;

    public DbSet<PhieuNhapSach> DsPhieuNhapSach { get; set; } = null!;
    public DbSet<ChiTietPhieuNhap> DsChiTietNhap { get; set; } = null!;

    public DbSet<HoaDon> DsHoaDon { get; set; } = null!;
    public DbSet<ChiTietHoaDon> DsChiTietHoaDon { get; set; } = null!;

    public DbSet<PhieuThu> DsPhieuThu { get; set; } = null!;

    /* -------------------- Fluent-API -------------------- */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        /* === PHIEUNHAPSACH (1) — (N) CHITIETNHAP === */
        modelBuilder.Entity<ChiTietPhieuNhap>()
            .HasKey(c => new { c.MaPhieuNhapSach, c.MaSach });            // khóa chính tổ hợp

        modelBuilder.Entity<ChiTietPhieuNhap>()
            .HasOne(c => c.PhieuNhapSach)
            .WithMany(p => p.DsChiTietNhap)
            .HasForeignKey(c => c.MaPhieuNhapSach)
            .OnDelete(DeleteBehavior.Cascade);

        /* === CHITIETNHAP (N) — (1) SACH === */
        modelBuilder.Entity<ChiTietPhieuNhap>()
            .HasOne(c => c.Sach)
            .WithMany(s => s.DsChiTietPhieuNhap)
            .HasForeignKey(c => c.MaSach)
            .OnDelete(DeleteBehavior.Cascade);

        /* === HoaDon (1) — (N) CHITIETHOADON === */
        modelBuilder.Entity<ChiTietHoaDon>()
            .HasKey(c => new { c.MaHoaDon, c.MaSach });                    // khóa chính tổ hợp

        modelBuilder.Entity<ChiTietHoaDon>()
            .HasOne(c => c.HoaDon)
            .WithMany(h => h.DsChiTietHoaDon)
            .HasForeignKey(c => c.MaHoaDon)
            .OnDelete(DeleteBehavior.Cascade);

        /* === CHITIETHOADON (N) — (1) SACH === */
        modelBuilder.Entity<ChiTietHoaDon>()
            .HasOne(c => c.Sach)
            .WithMany(s => s.DsChiTietHoaDon)
            .HasForeignKey(c => c.MaSach)
            .OnDelete(DeleteBehavior.Cascade);

        /* === KHACHHANG (1) — (N) HoaDon === */
        modelBuilder.Entity<HoaDon>()
            .HasOne(h => h.KhachHang)
            .WithMany(k => k.DsHoaDon)
            .HasForeignKey(h => h.MaKhachHang)
            .OnDelete(DeleteBehavior.Cascade);

        /* === KHACHHANG (1) — (N) PHIEUTHU === */
        modelBuilder.Entity<PhieuThu>()
            .HasOne(p => p.KhachHang)
            .WithMany(k => k.DsPhieuThu)
            .HasForeignKey(p => p.MaKhachHang)
            .OnDelete(DeleteBehavior.Cascade);

        DatabaseSeeder.Seed(modelBuilder);
    }
}
