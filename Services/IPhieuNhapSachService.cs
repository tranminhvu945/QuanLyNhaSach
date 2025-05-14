using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Services
{
    public interface IPhieuNhapSachService
    {
        Task<PhieuNhapSach> GetPhieuNhapById(int id);
        Task<IEnumerable<PhieuNhapSach>> GetAllPhieuNhap();
        Task AddPhieuNhap(PhieuNhapSach phieuNhap);
        Task UpdatePhieuNhap(PhieuNhapSach phieuNhap);
        Task DeletePhieuNhap(int id);
        Task<int> GenerateAvailableId();
    }
}
