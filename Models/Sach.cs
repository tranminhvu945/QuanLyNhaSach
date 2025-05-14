using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaSach.Models
{
    public class Sach
    {
        [Key]
        public int MaSach { get; set; }                                 // PK

        [Required(ErrorMessage = "Tên sách không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sách không được vượt quá 200 ký tự")]
        public string TenSach { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Thể loại không được vượt quá 100 ký tự")]
        public string? TheLoai { get; set; }                            // Nullable – có thể để trống

        [StringLength(100, ErrorMessage = "Tác giả không được vượt quá 100 ký tự")]
        public string? TacGia { get; set; }                             // Nullable – có thể để trống

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải không âm")]
        public int SoLuongTon { get; set; } = 0;

        /* -------------------- Quan hệ (navigation) -------------------- */

        // 1 sách có thể xuất hiện trong nhiều chi tiết phiếu nhập
        public ICollection<ChiTietPhieuNhap> DsChiTietPhieuNhap { get; set; } = [];

        // 1 sách có thể xuất hiện trong nhiều chi tiết hoá đơn bán
        public ICollection<ChiTietHoaDon> DsChiTietHoaDon { get; set; } = [];
    }
}
