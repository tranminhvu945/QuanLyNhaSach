using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using QuanLyNhaSach.Messages;
using QuanLyNhaSach.Models;
using QuanLyNhaSach.Models.dto;
using QuanLyNhaSach.Services;
using QuanLyNhaSach.Views.BaoCaoViews;
using QuestPDF.Fluent;

namespace QuanLyNhaSach.ViewModels.BaoCaoViewModel
{

    public partial class BaoCaoTonSachViewModel : ObservableObject, IRecipient<SelectedDateMessage>
    {
        private readonly ISachService _sachService;
        private readonly IPhieuNhapSachService _phieuNhapSachService;
        private readonly IChiTietPhieuNhapService _chiTietPhieuNhapService;
        private readonly IHoaDonService _hoaDonService;
        private readonly IChiTietHoaDonService _chiTietHoaDonService;



        public BaoCaoTonSachViewModel(
            ISachService sachService,
            IPhieuNhapSachService phieuNhapSachService,
            IHoaDonService hoaDonService,
            IChiTietPhieuNhapService chiTietPhieuNhapService,
            IChiTietHoaDonService chiTietHoaDonService
            )
        {

            WeakReferenceMessenger.Default.RegisterAll(this);
            _sachService = sachService;
            _phieuNhapSachService = phieuNhapSachService;
            _hoaDonService = hoaDonService;
            _chiTietPhieuNhapService = chiTietPhieuNhapService;
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
        private int _tongTonCuoiThang = 0;
        partial void OnTongTonCuoiThangChanged(int value)
        {
        }

        [ObservableProperty]
        private ObservableCollection<BaoCaoTonSach> _baoCaoTonSachList = new ObservableCollection<BaoCaoTonSach>();

        partial void OnBaoCaoTonSachListChanged(ObservableCollection<BaoCaoTonSach> value)
        {
            _ = LoadDataAsync();
        }


        // Relaycommand
        [RelayCommand]
        private void Close()
        {
            WeakReferenceMessenger.Default.Send(new DataReloadMessage());
            Application.Current.Windows.OfType<BaoCaoTonWindow>().FirstOrDefault()?.Close();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                BaoCaoTonSachList.Clear();

                // Lấy danh sách tất cả sách từ service
                var sachList = await _sachService.GetAllSach();

                int month = int.Parse(SelectedMonth.Replace("Tháng ", ""));
                int year = SelectedYear;

                DateTime dauThang = new DateTime(year, month, 1);

                foreach (var sach in sachList)
                {
                    var chiTietPhieuNhapList = await _chiTietPhieuNhapService.GetAllChiTietPhieuNhap();
                    var chiTietPhieuNhapTheoSach = chiTietPhieuNhapList
                        .Where(ct => ct.MaSach == sach.MaSach)
                        .ToList();

                    var chiTietHoaDonList = await _chiTietHoaDonService.GetAllChiTietHoaDon();
                    var chiTietHoaDonTheoSach = chiTietHoaDonList
                        .Where(ct => ct.MaSach == sach.MaSach)
                        .ToList();

                    int tonDau = 0;
                    int phatSinh = 0;
                    foreach (var chiTiet in chiTietPhieuNhapTheoSach)
                    {
                        var phieuNhap = await _phieuNhapSachService.GetPhieuNhapById(chiTiet.MaPhieuNhapSach);

                        // Tính tồn đầu tháng (tất cả tồn trước tháng được tính)
                        if (phieuNhap.NgayNhap < dauThang)
                            tonDau += chiTiet.SoLuongNhap;
                        else 
                            if (phieuNhap.NgayNhap.Month == month && phieuNhap.NgayNhap.Year == year)
                                phatSinh += chiTiet.SoLuongNhap;


                    }

                    foreach (var chiTiet in chiTietHoaDonTheoSach)
                    {
                        var hoaDon = await _hoaDonService.GetHoaDonById(chiTiet.MaHoaDon);

                        // Tính tồn đầu tháng (tất cả tồn trước tháng được tính)
                        if (hoaDon.NgayLap < dauThang)
                            tonDau -= chiTiet.SoLuongBan;
                        else
                            if (hoaDon.NgayLap.Month == month && hoaDon.NgayLap.Year == year)
                                phatSinh -= chiTiet.SoLuongBan;
                    }
                    TongTonCuoiThang += tonDau + phatSinh;
                    BaoCaoTonSachList.Add(new BaoCaoTonSach
                    {
                        STT = BaoCaoTonSachList.Count + 1,
                        TenSach = sach.TenSach,
                        TonDau = tonDau,
                        TonCuoi = tonDau + phatSinh,
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
            if (BaoCaoTonSachList == null || BaoCaoTonSachList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất báo cáo.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string baseFileName = $"BaoCaoTonSach_{SelectedMonth.Replace(" ", "")}_{SelectedYear}";
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
                                .Text($"Báo Cáo Tồn Sách {SelectedMonth}-{SelectedYear}")
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
                                            header.Cell().Element(CellStyle).AlignCenter().Text("Tên Sách").Bold();
                                            header.Cell().Element(CellStyle).AlignCenter().Text("Tồn Đầu").Bold();
                                            header.Cell().Element(CellStyle).AlignCenter().Text("Phát Sinh").Bold();
                                            header.Cell().Element(CellStyle).AlignCenter().Text("Tồn Cuối").Bold();
                                        });

                                        // Dữ liệu (Căn giữa các ô)
                                        foreach (var item in BaoCaoTonSachList)
                                        {
                                            table.Cell().Element(CellStyle).AlignCenter().Text(item.STT.ToString());
                                            table.Cell().Element(CellStyle).AlignCenter().Text(item.TenSach ?? "");
                                            table.Cell().Element(CellStyle).AlignCenter().Text(item.TonDau.ToString());
                                            table.Cell().Element(CellStyle).AlignCenter().Text(item.PhatSinh.ToString());
                                            table.Cell().Element(CellStyle).AlignCenter().Text(item.TonCuoi.ToString());
                                        }

                                        // Tổng doanh số
                                        table.Cell().ColumnSpan(3).Element(CellStyle).Text("Tổng tồn cuối trong tháng của tất cả các sách").Bold().AlignCenter();
                                        table.Cell().ColumnSpan(2).Element(CellStyle).Text(TongTonCuoiThang.ToString()).Bold().AlignCenter();
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
