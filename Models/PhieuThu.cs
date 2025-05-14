using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaSach.Models
{
    public class PhieuThu
    {
        [Key]
        public int MaPhieuThu { get; set; }

        public int MaKhachHang { get; set; }

        public DateTime NgayThu { get; set; } = DateTime.Now;

        [Range(0, long.MaxValue)]
        public long SoTienThu { get; set; } = 0;

        /* ---------- Navigation ---------- */
        public KhachHang KhachHang { get; set; } = null!;
    }
}
