using Microsoft.Win32;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

namespace ColorChange
{
    public partial class MainWindow : Window
    {
        private Color _girilenRenk;
        public Color GirilenRenk
        {
            get { return _girilenRenk; }
            set
            {
                _girilenRenk = value;
                OnPropertyChanged();
            }
        }

        private Color _cikanRenk;
        public Color CikanRenk
        {
            get { return _cikanRenk; }
            set
            {
                _cikanRenk = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            GirilenRenk = Colors.White;
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte r = Convert.ToByte(txtRed.Text);
                byte g = Convert.ToByte(txtGreen.Text);
                byte b = Convert.ToByte(txtBlue.Text);

                GirilenRenk = Color.FromRgb(r, g, b);

                double hue = GetHueFromRGB(r, g, b);
                CikanRenk = ColorFromHSL(hue, 0, 0);
                txtHSLResult.Text = "Hue (Ton): " + hue.ToString("0.##");

            }
            catch (Exception ex)
            {
                MessageBox.Show("RGB değerlerini doğru bir şekilde giriniz.");
            }
        }

        static double GetHueFromRGB(byte r, byte g, byte b)
        {
            double R = (double)r / 255;
            double G = (double)g / 255;
            double B = (double)b / 255;

            double max = Math.Max(R, Math.Max(G, B));
            double min = Math.Min(R, Math.Min(G, B));

            double hue = 0;

            if (max == R)
                hue = (60 * ((G - B) / (max - min)) + 360) % 360;
            else if (max == G)
                hue = (60 * ((B - R) / (max - min)) + 120);
            else if (max == B)
                hue = (60 * ((R - G) / (max - min)) + 240);

            return hue;
        }

        static Color ColorFromHSL(double hue, double saturation, double lightness)
        {
            if (saturation == 0)
                return Color.FromRgb(Convert.ToByte(lightness * 255), Convert.ToByte(lightness * 255), Convert.ToByte(lightness * 255));

            double q = lightness < 0.5 ? lightness * (1 + saturation) : lightness + saturation - lightness * saturation;
            double p = 2 * lightness - q;

            double hk = hue / 360;

            double tr = hk + 1.0 / 3.0;
            double tg = hk;
            double tb = hk - 1.0 / 3.0;

            tr = AdjustColor(tr, p, q);
            tg = AdjustColor(tg, p, q);
            tb = AdjustColor(tb, p, q);

            byte r = Convert.ToByte(tr * 255);
            byte g = Convert.ToByte(tg * 255);
            byte b = Convert.ToByte(tb * 255);

            return Color.FromRgb(r, g, b);
        }

        static double AdjustColor(double tc, double p, double q)
        {
            if (tc < 0) tc += 1;
            if (tc > 1) tc -= 1;
            if (tc < 1.0 / 6.0) return p + (q - p) * 6 * tc;
            if (tc < 0.5) return q;
            if (tc < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - tc) * 6;
            return p;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
