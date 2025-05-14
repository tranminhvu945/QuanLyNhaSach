using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaSach.Models
{
    public class ChiTietPhieuNhap
    {
        [Key]
        public int MaChiTietPhieuNhap { get; set; } = 0;
        public int MaPhieuNhapSach { get; set; }
        public int MaSach { get; set; }

        [Range(1, int.MaxValue)]
        public int SoLuongNhap { get; set; } = 1;

        /* ---------- Navigation ---------- */
        public PhieuNhapSach PhieuNhapSach { get; set; } = null!;
        public Sach Sach { get; set; } = null!;
    }
}
