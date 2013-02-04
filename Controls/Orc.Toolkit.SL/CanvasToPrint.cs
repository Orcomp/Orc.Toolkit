// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CanvasToPrint.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The canvas to print.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Printing;
    using Orc.Toolkit.Helpers;

    /// <summary>
    ///     The canvas to print.
    /// </summary>
    public static class CanvasToPrint
    {
        #region Public Methods and Operators

        /// <summary>
        /// The print to png file.
        /// </summary>
        /// <param name="canvas1">
        /// The canvas 1.
        /// </param>
        /// <param name="canvas2">
        /// The canvas 2.
        /// </param>
        /// <param name="canvas3">
        /// The canvas 3.
        /// </param>
        /// <param name="canvas4">
        /// The canvas 4.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="BitmapSource"/>.
        /// </returns>
        public static BitmapSource PrintToPngFile(
            Canvas canvas1, Canvas canvas2, Canvas canvas3, Canvas canvas4, Stream stream)
        {
            var wb =
                new WriteableBitmap(
                    (int)Math.Round(canvas1.ActualWidth, 0) + (int)Math.Round(canvas2.ActualWidth, 0), 
                    (int)Math.Round(canvas1.ActualHeight, 0) + (int)Math.Round(canvas3.ActualHeight, 0));

            wb.Render(canvas1, null);
            wb.Render(canvas2, new TranslateTransform { X = canvas1.ActualWidth, Y = 0 });
            wb.Render(canvas3, new TranslateTransform { X = 0, Y = canvas1.ActualHeight });
            wb.Render(canvas4, new TranslateTransform { X = canvas1.ActualWidth, Y = canvas2.ActualHeight });
            wb.Invalidate();

            var g = new PngGenerator(wb.PixelWidth, wb.PixelHeight);
            var colors = new Color[wb.PixelWidth * wb.PixelHeight];
            int i = 0;
            foreach (int p in wb.Pixels)
            {
                byte[] bytes = BitConverter.GetBytes(p);
                Color c = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
                colors[i] = c;
                i++;
            }

            g.SetPixelColorData(colors);
            Stream pngStream = g.CreateStream();
            pngStream.CopyTo(stream);
            stream.Close();

            return wb;
        }

        /// <summary>
        /// The print to printer.
        /// </summary>
        /// <param name="canvas1">
        /// The canvas 1.
        /// </param>
        /// <param name="canvas2">
        /// The canvas 2.
        /// </param>
        /// <param name="canvas3">
        /// The canvas 3.
        /// </param>
        /// <param name="canvas4">
        /// The canvas 4.
        /// </param>
        /// <returns>
        /// The <see cref="BitmapSource"/>.
        /// </returns>
        public static BitmapSource PrintToPrinter(Canvas canvas1, Canvas canvas2, Canvas canvas3, Canvas canvas4)
        {
            var wb =
                new WriteableBitmap(
                    (int)Math.Round(canvas1.ActualWidth, 0) + (int)Math.Round(canvas2.ActualWidth, 0), 
                    (int)Math.Round(canvas1.ActualHeight, 0) + (int)Math.Round(canvas3.ActualHeight, 0));

            wb.Render(canvas1, null);
            wb.Render(canvas2, new TranslateTransform { X = canvas1.ActualWidth, Y = 0 });
            wb.Render(canvas3, new TranslateTransform { X = 0, Y = canvas1.ActualHeight });
            wb.Render(canvas4, new TranslateTransform { X = canvas1.ActualWidth, Y = canvas2.ActualHeight });
            wb.Invalidate();

            var doc = new PrintDocument();
            doc.PrintPage += (s, e) =>
                {
                    e.HasMorePages = false;
                    var b = new Border { BorderThickness = new Thickness(0) };
                    b.Padding = new Thickness(96.0 / 2.54);

                    var img = new Image { Source = wb, Stretch = Stretch.Uniform };
                    b.Child = img;
                    b.Width = e.PrintableArea.Width;
                    b.Height = e.PrintableArea.Height;
                    e.PageVisual = b;
                };
            doc.PrintBitmap("Print Canvases");

            return wb;
        }

        #endregion
    }
}