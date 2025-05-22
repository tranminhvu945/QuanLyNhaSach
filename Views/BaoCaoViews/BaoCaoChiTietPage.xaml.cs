using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuanLyNhaSach.ViewModels.BaoCaoViewModel;

namespace QuanLyNhaSach.Views.BaoCaoViews
{

    public partial class BaoCaoChiTietPage : Page
    {
        public BaoCaoChiTietPage(BaoCaoChiTietViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            UpdateChartLabels(vm.TonSachLabels.ToList());

            // Lắng nghe thay đổi property
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.TonSachLabels))
                {
                    UpdateChartLabels(vm.TonSachLabels.ToList());
                }
            };
        }
        private void UpdateChartLabels(List<string> labels)
        {
            var processedLabels = labels.Select(label => WrapAndCenterLabel(label, 20)).ToList();
            TonSachChart.AxisX[0].Labels = processedLabels;
        }
        private string WrapAndCenterLabel(string input, int maxCharsPerLine = 20)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            var words = input.Split(' ');
            var lines = new List<string>();
            var currentLine = new StringBuilder();
            int currentLineLength = 0;

            foreach (var word in words)
            {
                if (currentLineLength + word.Length + 1 > maxCharsPerLine)
                {
                    lines.Add(currentLine.ToString());
                    currentLine.Clear();
                    currentLineLength = 0;
                }

                if (currentLineLength > 0)
                {
                    currentLine.Append(' ');
                    currentLineLength++;
                }

                currentLine.Append(word);
                currentLineLength += word.Length;
            }

            if (currentLine.Length > 0)
                lines.Add(currentLine.ToString());

            // Tìm độ dài dòng dài nhất để căn giữa các dòng còn lại
            double maxLineWidth = lines.Max(line => MeasureTextWidth(line, "Nunito", 12));

            var centeredLines = lines.Select(line =>
            {
                double lineWidth = MeasureTextWidth(line, "Nunito", 12);
                double paddingWidth = (maxLineWidth - lineWidth) / 2;

                // Tạm ước lượng mỗi space có chiều rộng ~ MeasureTextWidth(" ")
                double spaceWidth = MeasureTextWidth(" ", "Nunito", 12);
                int spaceCount = (int)(paddingWidth / spaceWidth);

                return new string(' ', spaceCount) + line;
            });


            return string.Join("\n", centeredLines);
        }
        private double MeasureTextWidth(string text, string fontFamily = "Nunito", double fontSize = 12)
        {
            var formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily),
                fontSize,
                Brushes.Black,
                new NumberSubstitution(),
                1.0);

            return formattedText.WidthIncludingTrailingWhitespace;
        }

    }
}
