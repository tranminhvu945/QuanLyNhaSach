using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyNhaSach.Models;

namespace QuanLyNhaSach.Services
{
    public interface IThamSoService
    {
        Task<ThamSo> GetThamSo();
        Task UpdateThamSo(ThamSo thamSo);
        Task<int> GenerateAvailableId();
    }
}
