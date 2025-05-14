using Microsoft.EntityFrameworkCore;
using QuanLyNhaSach.Configs;
using QuanLyNhaSach.Data;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;


namespace QuanLyNhaSach.Repositories
{
    public class SachRepository : ISachService
    {
        private readonly DataContext _context;

        public SachRepository(DatabaseConfig databaseConfig)
        {
            _context = databaseConfig.DataContext;
            if (_context == null)
            {
                throw new ArgumentNullException(nameof(databaseConfig), "Database not initialized!");
            }
        }

        public async Task<Sach> GetSachById(int id)
        {
            Sach? existingSach = await _context.DsSach
                                        .Include(s => s.DsChiTietPhieuNhap)
                                        .Include(s => s.DsChiTietHoaDon)
                                        .FirstOrDefaultAsync(s => s.MaSach == id);
            return existingSach ?? throw new Exception("Sach not found!");
        }

        public async Task<IEnumerable<Sach>> GetAllSach()
        {
            return await _context.DsSach
                            .Include(s => s.DsChiTietPhieuNhap)
                            .Include(s => s.DsChiTietHoaDon)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Sach>> GetSachPage(int offset, int size = 20)
        {
            return await _context.DsSach
                .Include(s => s.DsChiTietPhieuNhap)
                .Include(s => s.DsChiTietHoaDon)
                .Skip(offset*size)
                .Take(size)
                .ToListAsync();
        }

        public async Task<int> GetTotalPages(int size = 20)
        {
            int leftover = await _context.DsSach.CountAsync() % size;
            int totalPages = await _context.DsSach.CountAsync() / size;
            if (leftover > 0)
            {
                totalPages++;
            }
            return totalPages;
        }

        // Thêm một sách mới
        public async Task AddSach(Sach sach)
        {
            _context.DsSach.Add(sach);
            await _context.SaveChangesAsync();
        }

        // Cập nhật thông tin sách
        public async Task UpdateSach(Sach sach)
        {
            _context.Entry(sach).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Xóa một sách theo mã sách
        public async Task DeleteSach(int id)
        {
            var sach = await _context.DsSach.FindAsync(id);
            if (sach != null)
            {
                _context.DsSach.Remove(sach);
                await _context.SaveChangesAsync();
            }
        }

        // Lấy sách theo tên sách
        public async Task<Sach> GetSachByTenSach(string tenSach)
        {
            Sach? sach = await _context.DsSach.FirstAsync(s => s.TenSach == tenSach);
            return sach ?? throw new Exception("DaiLy not found!");
        }

        // Tạo ID sách mới (ví dụ: lấy mã lớn nhất và cộng thêm 1)
        public async Task<int> GenerateAvailableId()
        {
            int maxId = await _context.DsSach.MaxAsync(s => s.MaSach);
            return maxId + 1;
        }
    }
}
