using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Services
{
    public interface IKhachHangService
    {
        Task<KhachHang> GetKhachHangById(int id);
        Task<IEnumerable<KhachHang>> GetAllKhachHang();
        Task<IEnumerable<KhachHang>> GetKhachHangPage(int offset, int size = 20);
        Task<int> GetTotalPages(int size = 20);
        Task<KhachHang> GetMatHangByTenKhachHang(string tenKhachHang);
        Task AddKhachHang(KhachHang khachHang);
        Task UpdateKhachHang(KhachHang khachHang);
        Task DeleteKhachHang(int id);
        Task<int> GenerateAvailableId();
    }
}
