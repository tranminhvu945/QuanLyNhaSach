using Microsoft.EntityFrameworkCore;
using QuanLyNhaSach.Configs;
using QuanLyNhaSach.Data;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Services;

namespace QuanLyNhaSach.Repositories
{
    public class ThamSoRepository : IThamSoService
    {
        private readonly DataContext _context;

        public ThamSoRepository(DatabaseConfig databaseConfig)
        {
            _context = databaseConfig.DataContext;
            if (_context == null)
            {
                throw new ArgumentNullException(nameof(databaseConfig), "Database not initialized!");
            }
        }

        public async Task<ThamSo> GetThamSo()
        {
            ThamSo? thamSo = await _context.DsThamSo.FirstOrDefaultAsync();
            return thamSo ?? throw new Exception("ThamSo not found!");
        }

        public async Task UpdateThamSo(ThamSo thamSo)
        {
            _context.Entry(thamSo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<int> GenerateAvailableId()
        {
            int maxId = await _context.DsThamSo.MaxAsync(d => d.Id);
            return maxId + 1;
        }
    }
}
