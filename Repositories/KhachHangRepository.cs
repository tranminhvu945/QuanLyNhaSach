using Microsoft.EntityFrameworkCore;
using QuanLyNhaSach.Configs;
using QuanLyNhaSach.Data;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;

namespace QuanLyNhaSach.Repositories
{
    public class KhachHangRepository: IKhachHangService
    {
        private readonly DataContext _context;

        public KhachHangRepository(DatabaseConfig databaseConfig)
        {
            _context = databaseConfig.DataContext;
            if (_context == null)
            {
                throw new ArgumentNullException(nameof(databaseConfig), "Database not initialized!");
            }
        }
        public async Task<KhachHang> GetKhachHangById(int id)
        {
            KhachHang? matHang = await _context.DsKhachHang
                                        .Include(m => m.DsPhieuThu)
                                        .Include(m => m.DsHoaDon)
                                        .FirstOrDefaultAsync(m => m.MaKhachHang == id);
            return matHang ?? throw new Exception("KhachHang not found!");
        }
        public async Task<IEnumerable<KhachHang>> GetAllKhachHang()
        {
            return await _context.DsKhachHang
                            .Include(m => m.DsPhieuThu)
                            .Include(m => m.DsHoaDon)
                            .ToListAsync();
        }
        public async Task<IEnumerable<KhachHang>> GetKhachHangPage(int offset, int size = 20)
        {
            return await _context.DsKhachHang
                             .Include(m => m.DsPhieuThu)
                             .Include(m => m.DsHoaDon)
                             .Skip(offset * size)
                             .Take(size)
                             .ToListAsync();
        }
        public async Task<int> GetTotalPages(int size = 20)
        {
            int leftover = await _context.DsKhachHang.CountAsync() % size;
            int totalPages = await _context.DsKhachHang.CountAsync() / size;
            if (leftover > 0)
            {
                totalPages++;
            }
            return totalPages;
        }
        public async Task<KhachHang> GetMatHangByTenKhachHang(string tenKhachHang)
        {
            KhachHang? matHang = await _context.DsKhachHang
                                        .Include(m => m.DsPhieuThu)
                                        .Include(m => m.DsHoaDon)
                                        .FirstOrDefaultAsync(m => m.TenKhachHang == tenKhachHang);
            return matHang ?? throw new Exception("Khach hang not found!");
        }
        public async Task AddKhachHang(KhachHang khachHang)
        {
            _context.DsKhachHang.Add(khachHang);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateKhachHang(KhachHang khachHang)
        {
            _context.Entry(khachHang).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteKhachHang(int id)
        {
            var khachHang = await _context.DsKhachHang.FindAsync(id);
            if (khachHang != null)
            {
                _context.DsKhachHang.Remove(khachHang);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<int> GenerateAvailableId()
        {
            int maxId = await _context.DsKhachHang.MaxAsync(d => d.MaKhachHang);
            return maxId + 1;
        }
    }
}
