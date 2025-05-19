using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Services
{
    public interface IHoaDonService
    {
        Task<HoaDon> GetHoaDonById(int id);
        Task<IEnumerable<HoaDon>> GetAllHoaDon();
<<<<<<< HEAD
=======
        Task<IEnumerable<HoaDon>> GetHoaDonByKhachHangId(int maKhachHang);
        Task<IEnumerable<HoaDon>> GetHoaDonPage(int offset, int size = 20);
        Task<int> GetTotalPages(int size = 20);
>>>>>>> 7fbcc2a47ed33390367620730d36b0f2d621ea02
        Task AddHoaDon(HoaDon hoaDon);
        Task UpdateHoaDon(HoaDon hoaDon);
        Task DeleteHoaDon(int id);
        Task<int> GenerateAvailableId();
    }
}
