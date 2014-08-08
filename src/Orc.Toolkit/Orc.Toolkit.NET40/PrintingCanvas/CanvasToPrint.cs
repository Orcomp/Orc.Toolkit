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

    /// <summary>
    ///     The canvas to print.
    /// </summary>
    public static class CanvasToPrint
    {
        // corrects a wpf RenderTargetBitmap rendering bug (does not assume the layout rendering offset);
        #region Public Methods and Operators

        public static BitmapSource SaveToPng(Canvas canvas1, Canvas canvas2, Canvas canvas3, Canvas canvas4)
        {
            var saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();

            saveFileDialog1.Filter = "png files (*.png)|*.png|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == true)
            {
                BitmapSource bs = CanvasToPrint.PrintToPngFile(
                    canvas1, canvas2, canvas3, canvas4, saveFileDialog1.FileName);

                return bs;
            }

            return null;
        }

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
        /// <param name="pngFilePath">
        /// The png file path.
        /// </param>
        /// <returns>
        /// The <see cref="BitmapSource"/>.
        /// </returns>
        public static BitmapSource PrintToPngFile(
            Canvas canvas1, Canvas canvas2, Canvas canvas3, Canvas canvas4, string pngFilePath)
        {
            var rtb1 = new RenderTargetBitmap(
                (int)Math.Round(canvas1.ActualWidth, 0), 
                (int)Math.Round(canvas1.ActualHeight, 0), 
                96.0, 
                96.0, 
                PixelFormats.Pbgra32);
            ModifyPosition(canvas1);
            rtb1.Render(canvas1);
            ModifyPositionBack(canvas1);
            var pixels1 = new byte[rtb1.PixelWidth * rtb1.PixelHeight * rtb1.Format.BitsPerPixel / 8];
            rtb1.CopyPixels(pixels1, (rtb1.PixelWidth * rtb1.Format.BitsPerPixel) / 8, 0);

            var rtb2 = new RenderTargetBitmap(
                (int)Math.Round(canvas2.ActualWidth, 0), 
                (int)Math.Round(canvas2.ActualHeight, 0), 
                96.0, 
                96.0, 
                PixelFormats.Pbgra32);
            ModifyPosition(canvas2);
            rtb2.Render(canvas2);
            ModifyPositionBack(canvas2);
            var pixels2 = new byte[rtb2.PixelWidth * rtb2.PixelHeight * rtb2.Format.BitsPerPixel / 8];
            rtb2.CopyPixels(pixels2, (rtb2.PixelWidth * rtb2.Format.BitsPerPixel) / 8, 0);

            var rtb3 = new RenderTargetBitmap(
                (int)Math.Round(canvas3.ActualWidth, 0), 
                (int)Math.Round(canvas3.ActualHeight, 0), 
                96.0, 
                96.0, 
                PixelFormats.Pbgra32);
            ModifyPosition(canvas3);
            rtb3.Render(canvas3);
            ModifyPositionBack(canvas3);
            var pixels3 = new byte[rtb3.PixelWidth * rtb3.PixelHeight * rtb3.Format.BitsPerPixel / 8];
            rtb3.CopyPixels(pixels3, (rtb3.PixelWidth * rtb3.Format.BitsPerPixel) / 8, 0);

            var rtb4 = new RenderTargetBitmap(
                (int)Math.Round(canvas4.ActualWidth, 0), 
                (int)Math.Round(canvas4.ActualHeight, 0), 
                96.0, 
                96.0, 
                PixelFormats.Pbgra32);
            ModifyPosition(canvas4);
            rtb4.Render(canvas4);
            ModifyPositionBack(canvas4);
            var pixels4 = new byte[rtb4.PixelWidth * rtb4.PixelHeight * rtb4.Format.BitsPerPixel / 8];
            rtb4.CopyPixels(pixels4, (rtb4.PixelWidth * rtb4.Format.BitsPerPixel) / 8, 0);

            var wb = new WriteableBitmap(
                rtb1.PixelWidth + rtb2.PixelWidth, 
                rtb1.PixelHeight + rtb3.PixelHeight, 
                96.0, 
                96.0, 
                PixelFormats.Pbgra32, 
                null);

            wb.WritePixels(
                new Int32Rect(0, 0, rtb1.PixelWidth, rtb1.PixelHeight), 
                pixels1, 
                (rtb1.PixelWidth * rtb1.Format.BitsPerPixel) / 8, 
                0);

            wb.WritePixels(
                new Int32Rect(rtb1.PixelWidth, 0, rtb2.PixelWidth, rtb2.PixelHeight), 
                pixels2, 
                (rtb2.PixelWidth * rtb2.Format.BitsPerPixel) / 8, 
                0);

            wb.WritePixels(
                new Int32Rect(0, rtb1.PixelHeight, rtb3.PixelWidth, rtb3.PixelHeight), 
                pixels3, 
                (rtb3.PixelWidth * rtb3.Format.BitsPerPixel) / 8, 
                0);

            wb.WritePixels(
                new Int32Rect(rtb3.PixelWidth, rtb2.PixelHeight, rtb4.PixelWidth, rtb4.PixelHeight), 
                pixels4, 
                (rtb4.PixelWidth * rtb4.Format.BitsPerPixel) / 8, 
                0);

            var en = new PngBitmapEncoder();
            en.Frames.Add(BitmapFrame.Create(wb));

            var s = new FileStream(pngFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            en.Save(s);
            s.Close();

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
            var rtb1 = new RenderTargetBitmap(
                (int)Math.Round(canvas1.ActualWidth, 0), 
                (int)Math.Round(canvas1.ActualHeight, 0), 
                96.0, 
                96.0, 
                PixelFormats.Pbgra32);
            ModifyPosition(canvas1);
            rtb1.Render(canvas1);
            ModifyPositionBack(canvas1);
            var pixels1 = new byte[rtb1.PixelWidth * rtb1.PixelHeight * rtb1.Format.BitsPerPixel / 8];
            rtb1.CopyPixels(pixels1, (rtb1.PixelWidth * rtb1.Format.BitsPerPixel) / 8, 0);

            var rtb2 = new RenderTargetBitmap(
                (int)Math.Round(canvas2.ActualWidth, 0), 
                (int)Math.Round(canvas2.ActualHeight, 0), 
                96.0, 
                96.0, 
                PixelFormats.Pbgra32);
            ModifyPosition(canvas2);
            rtb2.Render(canvas2);
            ModifyPositionBack(canvas2);
            var pixels2 = new byte[rtb2.PixelWidth * rtb2.PixelHeight * rtb2.Format.BitsPerPixel / 8];
            rtb2.CopyPixels(pixels2, (rtb2.PixelWidth * rtb2.Format.BitsPerPixel) / 8, 0);

            var rtb3 = new RenderTargetBitmap(
                (int)Math.Round(canvas3.ActualWidth, 0), 
                (int)Math.Round(canvas3.ActualHeight, 0), 
                96.0, 
                96.0, 
                PixelFormats.Pbgra32);
            ModifyPosition(canvas3);
            rtb3.Render(canvas3);
            ModifyPositionBack(canvas3);
            var pixels3 = new byte[rtb3.PixelWidth * rtb3.PixelHeight * rtb3.Format.BitsPerPixel / 8];
            rtb3.CopyPixels(pixels3, (rtb3.PixelWidth * rtb3.Format.BitsPerPixel) / 8, 0);

            var rtb4 = new RenderTargetBitmap(
                (int)Math.Round(canvas4.ActualWidth, 0), 
                (int)Math.Round(canvas4.ActualHeight, 0), 
                96.0, 
                96.0, 
                PixelFormats.Pbgra32);
            ModifyPosition(canvas4);
            rtb4.Render(canvas4);
            ModifyPositionBack(canvas4);
            var pixels4 = new byte[rtb4.PixelWidth * rtb4.PixelHeight * rtb4.Format.BitsPerPixel / 8];
            rtb4.CopyPixels(pixels4, (rtb4.PixelWidth * rtb4.Format.BitsPerPixel) / 8, 0);

            var wb = new WriteableBitmap(
                rtb1.PixelWidth + rtb2.PixelWidth, 
                rtb1.PixelHeight + rtb3.PixelHeight, 
                96.0, 
                96.0, 
                PixelFormats.Pbgra32, 
                null);

            wb.WritePixels(
                new Int32Rect(0, 0, rtb1.PixelWidth, rtb1.PixelHeight), 
                pixels1, 
                (rtb1.PixelWidth * rtb1.Format.BitsPerPixel) / 8, 
                0);

            wb.WritePixels(
                new Int32Rect(rtb1.PixelWidth, 0, rtb2.PixelWidth, rtb2.PixelHeight), 
                pixels2, 
                (rtb2.PixelWidth * rtb2.Format.BitsPerPixel) / 8, 
                0);

            wb.WritePixels(
                new Int32Rect(0, rtb1.PixelHeight, rtb3.PixelWidth, rtb3.PixelHeight), 
                pixels3, 
                (rtb3.PixelWidth * rtb3.Format.BitsPerPixel) / 8, 
                0);

            wb.WritePixels(
                new Int32Rect(rtb3.PixelWidth, rtb2.PixelHeight, rtb4.PixelWidth, rtb4.PixelHeight), 
                pixels4, 
                (rtb4.PixelWidth * rtb4.Format.BitsPerPixel) / 8, 
                0);

            var d = new PrintDialog();
            if (d.ShowDialog() == true)
            {
                var img = new Image();
                var b = new Border();
                b.Padding = new Thickness(1 * 96.0 / 2.54);
                b.Child = img;
                img.Source = wb;
                b.Measure(new Size(d.PrintableAreaWidth, d.PrintableAreaHeight));
                b.Arrange(new Rect(new Point(0, 0), new Size(d.PrintableAreaWidth, d.PrintableAreaHeight)));

                d.PrintVisual(b, "Print canvases");
            }

            return wb;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The modify position.
        /// </summary>
        /// <param name="fe">
        /// The fe.
        /// </param>
        private static void ModifyPosition(FrameworkElement fe)
        {
            var fs = new Size(
                fe.ActualWidth + fe.Margin.Left + fe.Margin.Right, fe.ActualHeight + fe.Margin.Top + fe.Margin.Bottom);

            fe.Measure(fs);

            fe.Arrange(new Rect(-fe.Margin.Left, -fe.Margin.Top, fs.Width, fs.Height));
        }

        /// <summary>
        /// The modify position back.
        /// </summary>
        /// <param name="fe">
        /// The fe.
        /// </param>
        private static void ModifyPositionBack(FrameworkElement fe)
        {
            fe.Measure(new Size());
        }

        #endregion
    }
}