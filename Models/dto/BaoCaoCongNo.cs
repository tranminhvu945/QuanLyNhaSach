using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaSach.Models.dto
{
    public partial class BaoCaoCongNo : ObservableObject
    {
        public int STT { get; set; }
        public string TenKhachHang { get; set; }
        public decimal NoDauThang { get; set; }
        public decimal NoCuoiThang { get; set; }
        public decimal PhatSinh { get; set; }
    }
}
