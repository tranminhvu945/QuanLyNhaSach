using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Services
{
    public interface IChiTietPhieuNhapService
    {
        Task<ChiTietPhieuNhap> GetChiTietPhieuNhapById(int id);
        Task<IEnumerable<ChiTietPhieuNhap>> GetAllChiTietPhieuNhap();
        Task AddChiTietPhieuNhap(ChiTietPhieuNhap chiTietPhieuNhap);
        Task UpdateChiTietPhieuNhap(ChiTietPhieuNhap chiTietPhieuNhap);
        Task DeleteChiTietPhieuNhap(int id);
        Task<IEnumerable<ChiTietPhieuNhap>> GetChiTietPhieuNhapByPhieuNhapId(int maPhieuNhap);
    }
}
