using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyNhaSach.Models.dto
{
    public partial class BaoCaoTonSach : ObservableObject
    {
        public int STT { get; set; }
        public string TenSach { get; set; }
        public decimal TonDau { get; set; }
        public decimal TonCuoi { get; set; }
        public decimal PhatSinh { get; set; }
    }
}
