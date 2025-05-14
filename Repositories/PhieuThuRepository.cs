using Microsoft.EntityFrameworkCore;
using QuanLyNhaSach.Configs;
using QuanLyNhaSach.Data;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;

namespace QuanLyNhaSach.Repositories
{
    public class PhieuThuRepository : IPhieuThuService
    {
        private readonly DataContext _context;
        public PhieuThuRepository(DatabaseConfig databaseConfig)
        {
            _context = databaseConfig.DataContext;
            if (_context == null)
            {
                throw new ArgumentNullException(nameof(databaseConfig), "Database not initialized!");
            }
        }

        public async Task<PhieuThu> GetPhieuThuById(int id)
        {
            PhieuThu? phieuThu = await _context.DsPhieuThu
                .Include(p => p.KhachHang)
                .FirstOrDefaultAsync(p => p.MaPhieuThu == id);
            return phieuThu ?? throw new Exception("PhieuThu not found!");
        }
        async public Task<IEnumerable<PhieuThu>> GetPhieuThuByKhachHangId(int maKhachHang)
        {
            return await _context.DsPhieuThu
                .Include(p => p.KhachHang)
                .Where(p => p.MaKhachHang == maKhachHang)
                .ToListAsync();
        }
        public async Task<IEnumerable<PhieuThu>> GetAllPhieuThu()
        {
            return await _context.DsPhieuThu
                            .Include(p => p.KhachHang)
                            .ToListAsync();
        }
        public async Task<IEnumerable<PhieuThu>> GetPhieuThuPage(int offset, int size = 20)
        {
            return await _context.DsPhieuThu
                            .Include(m => m.KhachHang)
                            .Skip(offset * size)
                            .Take(size)
                            .ToListAsync();
        }
        public async Task<int> GetTotalPages(int size = 20)
        {
            int leftover = await _context.DsPhieuThu.CountAsync() % size;
            int totalPages = await _context.DsPhieuThu.CountAsync() / size;
            if (leftover > 0)
            {
                totalPages++;
            }
            return totalPages;
        }
        public async Task AddPhieuThu(PhieuThu phieuThu)
        {
            _context.DsPhieuThu.Add(phieuThu);
            await _context.SaveChangesAsync();
        }
        public async Task UpdatePhieuThu(PhieuThu phieuThu)
        {
            _context.Entry(phieuThu).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeletePhieuThu(int id)
        {
            var phieuThu = await _context.DsPhieuThu.FindAsync(id);
            if (phieuThu != null)
            {
                _context.DsPhieuThu.Remove(phieuThu);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<int> GenerateAvailableId()
        {
            int maxId = await _context.DsPhieuThu.MaxAsync(d => d.MaPhieuThu);
            return maxId + 1;
        }
    }
}
