using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace ColorChange
{
    public partial class MainWindow : Window
    {
        private BitmapImage selectedImage;
        private Bitmap selectedBitmap;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;
                selectedImage = new BitmapImage(new Uri(selectedFile));
                imgSelectedImage.Source = selectedImage;
                selectedBitmap = new Bitmap(selectedFile);
            }
        }

        private System.Windows.Point clickedPoint;

        private void imgSelectedImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            clickedPoint = e.GetPosition(imgSelectedImage);
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBitmap == null)
            {
                System.Windows.MessageBox.Show("Please select an image first.");
                return;
            }

            // Check if a point on the image has been clicked
            if (clickedPoint == null)
            {
                System.Windows.MessageBox.Show("Please click on the image to select a color.");
                return;
            }

            // Get the size of the image
            int imageWidth = selectedBitmap.Width;
            int imageHeight = selectedBitmap.Height;

            // Get the cursor position
            System.Windows.Point cursor = Mouse.GetPosition(imgSelectedImage);
            int cursorX = (int)(cursor.X * imageWidth / imgSelectedImage.ActualWidth);
            int cursorY = (int)(cursor.Y * imageHeight / imgSelectedImage.ActualHeight);

            // Define the region size to sample around the clicked pixel
            int regionSize = 5; // Adjust as needed

            // Get the average color of the region around the clicked pixel
            Color averageColor = GetAverageColor(selectedBitmap, cursorX, cursorY, regionSize);

            // Convert RGB to HSL
            double hue = averageColor.GetHue();

            // Apply filter
            Bitmap filteredBitmap = new Bitmap(selectedBitmap);
            for (int x = 0; x < filteredBitmap.Width; x++)
            {
                for (int y = 0; y < filteredBitmap.Height; y++)
                {
                    Color pixelColor = filteredBitmap.GetPixel(x, y);
                    double pixelHue = pixelColor.GetHue();
                    if (Math.Abs(pixelHue - hue) > 10) // Adjust threshold as needed
                    {
                        filteredBitmap.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        filteredBitmap.SetPixel(x, y, Color.Black);
                    }
                }
            }

            imgFiltered.Source = ConvertBitmapToBitmapImage(filteredBitmap);
        }

        private Color GetAverageColor(Bitmap bitmap, int centerX, int centerY, int regionSize)
        {
            int totalRed = 0;
            int totalGreen = 0;
            int totalBlue = 0;
            int count = 0;

            // Calculate the region boundaries
            int startX = Math.Max(0, centerX - regionSize);
            int startY = Math.Max(0, centerY - regionSize);
            int endX = Math.Min(bitmap.Width - 1, centerX + regionSize);
            int endY = Math.Min(bitmap.Height - 1, centerY + regionSize);

            // Sum up the color components within the region
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    totalRed += pixelColor.R;
                    totalGreen += pixelColor.G;
                    totalBlue += pixelColor.B;
                    count++;
                }
            }

            // Check if count is zero to avoid divide by zero exception
            if (count == 0)
            {
                // Return a default color (e.g., white)
                return Color.White;
            }

            // Calculate the average color
            int averageRed = totalRed / count;
            int averageGreen = totalGreen / count;
            int averageBlue = totalBlue / count;

            return Color.FromArgb(averageRed, averageGreen, averageBlue);
        }

        private void imgSelectedImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point clickedPoint = e.GetPosition(imgSelectedImage);
        }


        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }


    }
}
