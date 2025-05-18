using Microsoft.EntityFrameworkCore;
using QuanLyNhaSach.Configs;
using QuanLyNhaSach.Data;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;

namespace QuanLyNhaSach.Repositories
{
    public class HoaDonRepository: IHoaDonService
    {
        private readonly DataContext _context;
        public HoaDonRepository(DatabaseConfig databaseConfig)
        {
            _context = databaseConfig.DataContext;
            if (_context == null)
            {
                throw new ArgumentNullException(nameof(databaseConfig), "Database not initialized!");
            }
        }

        async public Task<IEnumerable<HoaDon>> GetHoaDonByKhachHangId(int maKhachHang)
        {
            return await _context.DsHoaDon
                .Include(p => p.KhachHang)
                .Where(p => p.MaKhachHang == maKhachHang)
                .ToListAsync();
        }

        public async Task<HoaDon> GetHoaDonById(int id)
        {
            HoaDon? phieuXuat = await _context.DsHoaDon
                                             .Include(p => p.KhachHang)
                                             .Include(p => p.DsChiTietHoaDon)
                                                 .ThenInclude(c => c.Sach)
                                             .FirstOrDefaultAsync(p => p.MaHoaDon == id);
            return phieuXuat ?? throw new Exception("HoaDon not found!");
        }
        public async Task<IEnumerable<HoaDon>> GetAllHoaDon()
        {
            return await _context.DsHoaDon
                            .Include(p => p.KhachHang)
                            .Include(p => p.DsChiTietHoaDon)
                                .ThenInclude(c => c.Sach)
                            .ToListAsync();
        }
        public async Task<IEnumerable<HoaDon>> GetHoaDonPage(int offset, int size = 20)
        {
            return await _context.DsHoaDon
                            .Include(m => m.KhachHang)
                            .Include(m => m.DsChiTietHoaDon)
                                .ThenInclude(c => c.Sach)
                            .Skip(offset * size)
                            .Take(size)
                            .ToListAsync();
        }
        public async Task<int> GetTotalPages(int size = 20)
        {
            int leftover = await _context.DsHoaDon.CountAsync() % size;
            int totalPages = await _context.DsHoaDon.CountAsync() / size;
            if (leftover > 0)
            {
                totalPages++;
            }
            return totalPages;
        }
        public async Task AddHoaDon(HoaDon hoaDon)
        {
            _context.DsHoaDon.Add(hoaDon);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateHoaDon(HoaDon hoaDon)
        {
            _context.Entry(hoaDon).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteHoaDon(int id)
        {
            var hoaDon = await _context.DsHoaDon.FindAsync(id);
            if (hoaDon != null)
            {
                _context.DsHoaDon.Remove(hoaDon);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<int> GenerateAvailableId()
        {
            int maxId = await _context.DsHoaDon.MaxAsync(d => d.MaHoaDon);
            return maxId + 1;
        }
    }
}
