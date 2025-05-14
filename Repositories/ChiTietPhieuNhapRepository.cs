using Microsoft.EntityFrameworkCore;
using QuanLyNhaSach.Configs;
using QuanLyNhaSach.Data;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;

namespace QuanLyNhaSach.Repositories
{
    public class ChiTietPhieuNhapRepository: IChiTietPhieuNhapService
    {
        private readonly DataContext _context;

        public ChiTietPhieuNhapRepository(DatabaseConfig databaseConfig)
        {
            _context = databaseConfig.DataContext;
            if (_context == null)
            {
                throw new ArgumentNullException(nameof(databaseConfig), "Database not initialized!");
            }
        }
        public async Task<ChiTietPhieuNhap> GetChiTietPhieuNhapById(int id)
        {
            ChiTietPhieuNhap? chiTietPhieuNhap = await _context.DsChiTietNhap
                                                         .Include(c => c.PhieuNhapSach)
                                                         .Include(c => c.Sach)
                                                         .FirstOrDefaultAsync(c => c.MaChiTietPhieuNhap == id);
            return chiTietPhieuNhap ?? throw new Exception("chiTietPhieuNhap not found!");
        }
        public async Task<IEnumerable<ChiTietPhieuNhap>> GetAllChiTietPhieuNhap()
        {
            return await _context.DsChiTietNhap
                            .Include(c => c.PhieuNhapSach)
                            .Include(c => c.Sach)
                            .ToListAsync();
        }
        public async Task AddChiTietPhieuNhap(ChiTietPhieuNhap chiTietPhieuNhap)
        {
            _context.DsChiTietNhap.Add(chiTietPhieuNhap);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateChiTietPhieuNhap(ChiTietPhieuNhap chiTietPhieuNhap)
        {
            _context.Entry(chiTietPhieuNhap).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteChiTietPhieuNhap(int id)
        {
            var chiTietPhieuNhap = await _context.DsChiTietNhap.FindAsync(id);
            if (chiTietPhieuNhap != null)
            {
                _context.DsChiTietNhap.Remove(chiTietPhieuNhap);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<ChiTietPhieuNhap>> GetChiTietPhieuNhapByPhieuNhapId(int maPhieuNhap)
        {
            return await _context.DsChiTietNhap
                            .Include(c => c.PhieuNhapSach)
                            .Include(c => c.Sach)
                            .Where(c => c.MaPhieuNhapSach == maPhieuNhap)
                            .ToListAsync();
        }
    }
}
