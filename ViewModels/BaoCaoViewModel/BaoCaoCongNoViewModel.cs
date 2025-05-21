using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models.dto;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.BaoCaoViews;
using QuestPDF.Fluent;

namespace QuanLyNhaSach.ViewModels.BaoCaoViewModel
{
    public partial class BaoCaoCongNoViewModel : ObservableObject, IRecipient<SelectedDateMessage>
    {
        private readonly IKhachHangService _khachHangService;
        private readonly IPhieuThuService _phieuThuService;
        private readonly IHoaDonService _hoaDonService;
        private readonly IChiTietHoaDonService _chiTietHoaDonService;
        public BaoCaoCongNoViewModel(
            IKhachHangService khachHangService,
            IPhieuThuService phieuThuService,
            IHoaDonService hoaDonService,
            IChiTietHoaDonService chiTietHoaDonService
            )
        {
            WeakReferenceMessenger.Default.RegisterAll(this);
            _khachHangService = khachHangService;
            _phieuThuService = phieuThuService;
            _hoaDonService = hoaDonService;
            _chiTietHoaDonService = chiTietHoaDonService;

            // Khởi tạo danh sách tháng
            MonthOptions = new ObservableCollection<string>
            {
                "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4",
                "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8",
                "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
            };

            // Khởi tạo danh sách năm
            int currentYear = DateTime.Now.Year;
            YearOptions = new ObservableCollection<int>();
            for (int i = currentYear - 4; i <= currentYear; i++)
            {
                YearOptions.Add(i);
            }

        }

        public ObservableCollection<string> MonthOptions { get; }
        public ObservableCollection<int> YearOptions { get; }

        [ObservableProperty]
        private string _selectedMonth = "Tháng 1";
        partial void OnSelectedMonthChanged(string value)
        {
            _ = LoadDataAsync();
        }

        [ObservableProperty]
        private int _selectedYear = 2025;
        partial void OnSelectedYearChanged(int value)
        {
            _ = LoadDataAsync();
        }

        [ObservableProperty]
        private ObservableCollection<BaoCaoCongNo> _baoCaoCongNoList = new ObservableCollection<BaoCaoCongNo>();
        partial void OnBaoCaoCongNoListChanged(ObservableCollection<BaoCaoCongNo> value)
        {
            _ = LoadDataAsync();
        }
        [ObservableProperty]
        private long _tongNoCuoiThang = 0;
        partial void OnTongNoCuoiThangChanged(long value)
        {
        }

        [RelayCommand]
        private void Close()
        {
            WeakReferenceMessenger.Default.Send(new DataReloadMessage());
            Application.Current.Windows.OfType<BaoCaoCongNoWindow>().FirstOrDefault()?.Close();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                BaoCaoCongNoList.Clear();
                var khachHangList = await _khachHangService.GetAllKhachHang();

                int month = int.Parse(SelectedMonth.Replace("Tháng ", ""));
                int year = SelectedYear;

                DateTime dauThang = new DateTime(year, month, 1);
                TongNoCuoiThang = 0;
                foreach (var kh in khachHangList)
                {
                    var hoaDonList = await _hoaDonService.GetAllHoaDon();
                    var hoaDonTheoKH = hoaDonList
                        .Where(hd => hd.MaKhachHang == kh.MaKhachHang)
                        .ToList();

                    var phieuThuList = await _phieuThuService.GetAllPhieuThu();
                    var phieuThuTheoKH = phieuThuList
                        .Where(hd => hd.MaKhachHang == kh.MaKhachHang)
                        .ToList();

                    long noDau = 0;
                    long phatSinh = 0;

                    foreach (var hd in hoaDonTheoKH)
                    {
                        if (hd.NgayLap < dauThang)
                            noDau += hd.TongTien;
                        else
                            if (hd.NgayLap.Month == month && hd.NgayLap.Year == year)
                            phatSinh += hd.TongTien;
                    }

                    foreach (var pt in phieuThuTheoKH)
                    {
                        if (pt.NgayThu < dauThang)
                            noDau -= pt.SoTienThu;
                        else
                            if (pt.NgayThu.Month == month && pt.NgayThu.Year == year)
                            phatSinh -= pt.SoTienThu;
                    }

                    TongNoCuoiThang += noDau + phatSinh;
                    BaoCaoCongNoList.Add(new BaoCaoCongNo
                    {
                        STT = BaoCaoCongNoList.Count + 1,
                        TenKhachHang = kh.TenKhachHang,
                        NoDauThang = noDau,
                        NoCuoiThang = noDau + phatSinh,
                        PhatSinh = phatSinh
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ExportToPDF()
        {
            if (BaoCaoCongNoList == null || BaoCaoCongNoList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất báo cáo.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string baseFileName = $"BaoCaoCongNo_{SelectedMonth.Replace(" ", "")}_{SelectedYear}";
            string extension = ".pdf";
            string pdfFilePath = Path.Combine(folderPath, baseFileName + extension);

            int count = 1;
            while (File.Exists(pdfFilePath))
            {
                pdfFilePath = Path.Combine(folderPath, $"{baseFileName} ({count}){extension}");
                count++;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = Path.GetFileName(pdfFilePath),
                InitialDirectory = folderPath
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                pdfFilePath = saveFileDialog.FileName;

                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);

                        // Header
                        page.Header().Element(header =>
                        {
                            header
                                .PaddingBottom(10)
                                .Text($"Báo Cáo Công Nợ {SelectedMonth}-{SelectedYear}")
                                .FontSize(16)
                                .Bold()
                                .AlignCenter();
                        });

                        // Nội dung
                        page.Content().Element(content =>
                        {
                            content.PaddingVertical(10).Column(column =>
                            {
                                column.Spacing(5);

                                // Thông tin người lập
                                column.Item().Text($"Người lập: Nguyễn Văn A").Italic();
                                column.Item().Text($"Ngày lập: {DateTime.Now:dd/MM/yyyy}").Italic();

                                // Bảng dữ liệu
                                column.Item().Element(tableContainer =>
                                {
                                    tableContainer.Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.ConstantColumn(50); 
                                            columns.RelativeColumn(2); 
                                            columns.RelativeColumn(); 
                                            columns.RelativeColumn(); 
                                            columns.RelativeColumn(); 
                                        });

                                        // Tiêu đề cột (Thêm màu nền xanh nhạt và căn giữa)
                                        table.Header(header =>
                                        {
                                            header.Cell().Element(CellStyle).AlignCenter().Text("STT").Bold();
                                            header.Cell().Element(CellStyle).AlignCenter().Text("Tên khách hàng").Bold();
                                            header.Cell().Element(CellStyle).AlignCenter().Text("Nợ Đầu").Bold();
                                            header.Cell().Element(CellStyle).AlignCenter().Text("Phát Sinh").Bold();
                                            header.Cell().Element(CellStyle).AlignCenter().Text("Nợ Cuối").Bold();
                                        });

                                        // Dữ liệu (Căn giữa các ô)
                                        foreach (var item in BaoCaoCongNoList)
                                        {
                                            table.Cell().Element(CellStyle).AlignCenter().Text(item.STT.ToString());
                                            table.Cell().Element(CellStyle).AlignCenter().Text(item.TenKhachHang ?? "");
                                            table.Cell().Element(CellStyle).AlignCenter().Text(item.NoDauThang.ToString("N0") + "VNĐ");
                                            table.Cell().Element(CellStyle).AlignCenter().Text(item.PhatSinh.ToString("N0") + "VNĐ");
                                            table.Cell().Element(CellStyle).AlignCenter().Text(item.NoCuoiThang.ToString("N0") + "VNĐ");
                                        }

                                        // Tổng doanh số
                                        table.Cell().ColumnSpan(3).Element(CellStyle).Text("Tổng nợ cuối trong tháng của tất cả khách hàng").Bold().AlignCenter();
                                        table.Cell().ColumnSpan(2).Element(CellStyle).Text(TongNoCuoiThang.ToString("N0") + "VNĐ").Bold().AlignCenter();
                                    });
                                });
                            });
                        });
                    });
                });

                try
                {
                    document.GeneratePdf(pdfFilePath);
                    MessageBox.Show($"Xuất PDF thành công:\n{pdfFilePath}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = pdfFilePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tạo PDF: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Style cho mỗi cell trong bảng
            QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
            {
                return container
                    .Border(1)
                    .BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2)
                    .Padding(5);
            }
        }
        public void Receive(SelectedDateMessage message)
        {
            (int month, int year) = message.Value;
            SelectedMonth = $"Tháng {month}";
            SelectedYear = year;
        }
    }
}
