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
        public int TonDau { get; set; }
        public int TonCuoi { get; set; }
        public int PhatSinh { get; set; }
    }
}
