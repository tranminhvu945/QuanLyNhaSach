using Microsoft.EntityFrameworkCore;
using QuanLyNhaSach.Configs;
using QuanLyNhaSach.Data;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;

namespace QuanLyNhaSach.Repositories
{
    public class ChiTietHoaDonRepository: IChiTietHoaDonService
    {
        private readonly DataContext _context;

        public ChiTietHoaDonRepository(DatabaseConfig databaseConfig)
        {
            _context = databaseConfig.DataContext;
            if (_context == null)
            {
                throw new ArgumentNullException(nameof(databaseConfig), "Database not initialized!");
            }
        }

        public async Task<ChiTietHoaDon> GetChiTietHoaDonById(int id)
        {
            ChiTietHoaDon? chiTietHoaDon = await _context.DsChiTietHoaDon
                                                         .Include(c => c.HoaDon)
                                                         .Include(c => c.Sach)
                                                         .FirstOrDefaultAsync(c => c.MaChiTietHoaDon == id);
            return chiTietHoaDon ?? throw new Exception("chiTietHoaDon not found!");
        }
        public async Task<IEnumerable<ChiTietHoaDon>> GetAllChiTietHoaDon()
        {
            return await _context.DsChiTietHoaDon
                .Include(c => c.HoaDon)
                .Include(c => c.Sach)
                .ToListAsync();
        }
        public async Task AddChiTietHoaDon(ChiTietHoaDon chiTietHoaDon)
        {
            _context.DsChiTietHoaDon.Add(chiTietHoaDon);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateChiTietHoaDon(ChiTietHoaDon chiTietHoaDon)
        {
            _context.Entry(chiTietHoaDon).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteChiTietHoaDon(ChiTietHoaDon chiTietHoaDon)
        {
            var entity = await _context.DsChiTietHoaDon.FindAsync(chiTietHoaDon.MaHoaDon, chiTietHoaDon.MaSach);
            if (entity != null)
            {
                _context.DsChiTietHoaDon.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<ChiTietHoaDon>> GetChiTietHoaDonByHoaDonId(int maHoaDon)
        {
            return await _context.DsChiTietHoaDon
                .Include(c => c.HoaDon)
                .Include(c => c.Sach)
                .Where(c => c.MaHoaDon == maHoaDon)
                .ToListAsync();
        }
    }
}
