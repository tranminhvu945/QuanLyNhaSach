using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaSach.Models
{
    public class PhieuNhapSach
    {
        [Key]
        public int MaPhieuNhapSach { get; set; }

        public DateTime NgayNhap { get; set; } = DateTime.Now;

        /* ---------- Navigation ---------- */
        public ICollection<ChiTietPhieuNhap> DsChiTietNhap { get; set; } = [];
    }
}
