using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaSach.Models
{
    public class ThamSo
    {
        [Key]
        public int Id { get; set; } = 0;
        public int SoLuongNhapToiThieu { get; set; } = 0;
        public int SoLuongTonToiDa { get; set; } = 0;
        public int SoLuongTonToiThieu { get; set; } = 0;
        public int TienNoToiDa { get; set; } = 0;
        public bool QuyDinhTienThuTienNo { get; set; } = true;
        //public bool QuyDinhSoLuongTonToiThieu { get; set; } = true;
    }
}
