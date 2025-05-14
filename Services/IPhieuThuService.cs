using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Services
{
    public interface IPhieuThuService
    {
        Task<PhieuThu> GetPhieuThuById(int id);
        Task<IEnumerable<PhieuThu>> GetPhieuThuByKhachHangId(int maKhachHang);
        Task<IEnumerable<PhieuThu>> GetAllPhieuThu();
        Task<IEnumerable<PhieuThu>> GetPhieuThuPage(int offset, int size = 20);
        Task<int> GetTotalPages(int size = 20);
        Task AddPhieuThu(PhieuThu phieuThu);
        Task UpdatePhieuThu(PhieuThu phieuThu);
        Task DeletePhieuThu(int id);
        Task<int> GenerateAvailableId();
    }
}
