using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaSach.Models
{
    public class HoaDon
    {
        [Key]
        public int MaHoaDon { get; set; }

        public int MaKhachHang { get; set; }

        public DateTime NgayLap { get; set; } = DateTime.Now;

        [Range(0, long.MaxValue)]
        public long TongTien { get; set; } = 0;

        /* ---------- Navigation ---------- */
        public KhachHang KhachHang { get; set; } = null!;
        public ICollection<ChiTietHoaDon> DsChiTietHoaDon { get; set; } = [];
    }
}
