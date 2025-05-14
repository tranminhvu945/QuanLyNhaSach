using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaSach.Models
{
    public class KhachHang
    {
        [Key]
        public int MaKhachHang { get; set; }

        [Required, StringLength(100)]
        public string TenKhachHang { get; set; } = string.Empty;

        [StringLength(200)]
        public string? DiaChi { get; set; }

        [StringLength(15), Phone]
        public string? DienThoai { get; set; }

        [StringLength(100), EmailAddress]
        public string? Email { get; set; }

        public long TienNo { get; set; } = 0;

        /* ---------- Navigation ---------- */
        public ICollection<HoaDon> DsHoaDon { get; set; } = [];
        public ICollection<PhieuThu> DsPhieuThu { get; set; } = [];
    }
}
