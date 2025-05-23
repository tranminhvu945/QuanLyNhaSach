﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Services
{
    public interface IHoaDonService
    {
        Task<HoaDon> GetHoaDonById(int id);
        Task<IEnumerable<HoaDon>> GetAllHoaDon();
        Task<IEnumerable<HoaDon>> GetHoaDonByKhachHangId(int maKhachHang);

        Task AddHoaDon(HoaDon hoaDon);
        Task UpdateHoaDon(HoaDon hoaDon);
        Task DeleteHoaDon(int id);
        Task<int> GenerateAvailableId();
    }
}
