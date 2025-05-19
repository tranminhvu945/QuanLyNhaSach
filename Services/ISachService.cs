using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Services
{
    public interface ISachService
    {
        Task<Sach> GetSachById(int id);
        Task<IEnumerable<Sach>> GetAllSach();
        Task AddSach(Sach sach);
        Task UpdateSach(Sach sach);
        Task DeleteSach(int id);
        Task<Sach> GetSachByTenSach(string tenSach);
        Task<int> GenerateAvailableId();
    }
}
