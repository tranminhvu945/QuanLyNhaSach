using Microsoft.EntityFrameworkCore;
using QuanLyNhaSach.Configs;
using QuanLyNhaSach.Data;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;

namespace QuanLyNhaSach.Repositories
{
    public class PhieuNhapSachRepository: IPhieuNhapSachService
    {
        private readonly DataContext _context;

        public PhieuNhapSachRepository(DatabaseConfig databaseConfig)
        {
            _context = databaseConfig.DataContext;
            if (_context == null)
            {
                throw new ArgumentNullException(nameof(databaseConfig), "Database not initialized!");
            }
        }
        public async Task<PhieuNhapSach> GetPhieuNhapById(int id)
        {
            PhieuNhapSach? phieuNhap = await _context.DsPhieuNhapSach
                                                .Include(p => p.DsChiTietNhap)
                                                    .ThenInclude(c => c.Sach)
                                                .FirstOrDefaultAsync(p => p.MaPhieuNhapSach == id);
            return phieuNhap ?? throw new Exception("PhieuXuat not found!");
        }
        public async Task<IEnumerable<PhieuNhapSach>> GetAllPhieuNhap()
        {
            return await _context.DsPhieuNhapSach
                            .Include(p => p.DsChiTietNhap)
                                .ThenInclude(c => c.Sach)
                            .ToListAsync();
        }
        public async Task AddPhieuNhap(PhieuNhapSach phieuNhap)
        {
            _context.DsPhieuNhapSach.Add(phieuNhap);
            await _context.SaveChangesAsync();
        }
        public async Task UpdatePhieuNhap(PhieuNhapSach phieuNhap)
        {
            _context.Entry(phieuNhap).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeletePhieuNhap(int id)
        {
            var phieuNhap = await _context.DsPhieuNhapSach.FindAsync(id);
            if (phieuNhap != null)
            {
                _context.DsPhieuNhapSach.Remove(phieuNhap);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<int> GenerateAvailableId()
        {
            int maxId = await _context.DsPhieuNhapSach.MaxAsync(d => d.MaPhieuNhapSach);
            return maxId + 1;
        }
    }
}
