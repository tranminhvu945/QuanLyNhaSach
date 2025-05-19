using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Services
{
    public interface IChiTietHoaDonService
    {
        Task<ChiTietHoaDon> GetChiTietHoaDonById(int id);
        Task<IEnumerable<ChiTietHoaDon>> GetAllChiTietHoaDon();
        Task AddChiTietHoaDon(ChiTietHoaDon chiTietHoaDon);
        Task UpdateChiTietHoaDon(ChiTietHoaDon chiTietHoaDon);
        Task DeleteChiTietHoaDon(ChiTietHoaDon chiTietHoaDon);
        Task<IEnumerable<ChiTietHoaDon>> GetChiTietHoaDonByHoaDonId(int maHoaDon);
    }
}
