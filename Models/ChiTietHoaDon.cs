using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaSach.Models
{
    public class ChiTietHoaDon
    {
        [Key]
        public int MaChiTietHoaDon { get; set; } = 0;
        public int MaHoaDon { get; set; }
        public int MaSach { get; set; }

        [Range(1, int.MaxValue)]
        public int SoLuongBan { get; set; } = 1;

        [Range(0, long.MaxValue)]
        public long DonGiaBan { get; set; } = 0;

        [Range(0, long.MaxValue)]
        public long ThanhTien { get; set; } = 0;

        /* ---------- Navigation ---------- */
        public HoaDon HoaDon { get; set; } = null!;
        public Sach Sach { get; set; } = null!;
    }
}
