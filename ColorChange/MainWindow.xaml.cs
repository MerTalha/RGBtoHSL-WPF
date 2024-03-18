using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ColorChange
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string input = txtInput.Text;
            string[] colors = input.Split(',');

            int[] rgb = new int[3];
            for (int i = 0; i < 3; i++)
            {
                rgb[i] = Convert.ToInt32(colors[i]);
            }

            Random random = new Random();
            int[] array1 = ReadArrayFromFile("data1.txt");
            int[] array2 = ReadArrayFromFile("data2.txt");
            int[] array3 = ReadArrayFromFile("data3.txt");

            
            System.Windows.Media.Color inputColor = System.Windows.Media.Color.FromRgb((byte)rgb[0], (byte)rgb[1], (byte)rgb[2]);
            inputColorBorder.Background = new SolidColorBrush(inputColor);
            rgb[0] = array1[rgb[0]];
            rgb[1] = array2[rgb[1]];
            rgb[2] = array3[rgb[2]];

            txtOutput.Text = $"Değiştirilmiş RGB renk kodu: {rgb[0]},{rgb[1]},{rgb[2]}";

            
            System.Windows.Media.Color outputColor = System.Windows.Media.Color.FromRgb((byte)rgb[0], (byte)rgb[1], (byte)rgb[2]);
            outputColorBorder.Background = new SolidColorBrush(outputColor);
        }

        private int[] ReadArrayFromFile(string filename)
        {
            int[] array = new int[255];
            try
            {
                string[] lines = File.ReadAllLines(filename);
                string[] values = lines[new Random().Next(lines.Length)].Split(',');

                for (int i = 0; i < 255; i++)
                {
                    array[i] = Convert.ToInt32(values[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya okuma hatası: {ex.Message}");
            }
            return array;
        }


    }
}
