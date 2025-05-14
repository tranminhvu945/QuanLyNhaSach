using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Services
{
    public interface ISachService
    {
        Task<Sach> GetSachById(int id);
        Task<IEnumerable<Sach>> GetAllSach();
        Task<IEnumerable<Sach>> GetSachPage(int offset, int size = 20);
        Task<int> GetTotalPages(int size = 20);
        Task AddSach(Sach sach);
        Task UpdateSach(Sach sach);
        Task DeleteSach(int id);
        Task<Sach> GetSachByTenSach(string tenSach);
        Task<int> GenerateAvailableId();
    }
}
