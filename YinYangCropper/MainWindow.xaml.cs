using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Mygod.Windows;

namespace Mygod.YinYangCropper
{
    public sealed partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop, true) ? e.AllowedEffects & DragDropEffects.Copy : DragDropEffects.None;
            DropTargetHelper.DragOver(e.GetPosition(this), e.Effects);
        }
        private void OnDragLeave(object sender, DragEventArgs e)
        {
            e.Handled = true;
            DropTargetHelper.DragLeave(e.Data);
        }

        private readonly OpenFileDialog imageBrowser = new OpenFileDialog
            { Filter = "图像文件 (*.bmp;*.gif;*.exif;*.jpg;*.jpeg;*.png;*.tif;*.tiff)|*.bmp;*.gif;*.exif;*.jpg;*.jpeg;*.png;*.tif;*.tiff" };
        private readonly SaveFileDialog imageSaver = new SaveFileDialog
            { Title = "请选择抠图后的图像要保存到哪里", Filter = "PNG 图像 (*.png)|*.png" };
        private string yinPath, yangPath;

        private void SetYin(string path)
        {
            YinImage.Source = new BitmapImage(new Uri(yinPath = path, UriKind.Absolute));
            YinDescription.Visibility = Visibility.Collapsed;
        }
        private void BrowseYin(object sender, RoutedEventArgs e)
        {
            imageBrowser.Title = "选择一张以黑色为背景的图片";
            if (imageBrowser.ShowDialog() == true) SetYin(imageBrowser.FileName);
        }
        private void OnYinDragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = e.AllowedEffects & DragDropEffects.Copy;
                DropTargetHelper.DragEnter(this, e.Data, e.GetPosition(this), e.Effects, "阴");
            }
            else
            {
                e.Effects = DragDropEffects.None;
                DropTargetHelper.DragEnter(this, e.Data, e.GetPosition(this), e.Effects);
            }
        }
        private void OnYinDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop, true) ? e.AllowedEffects & DragDropEffects.Copy : DragDropEffects.None;
            DropTargetHelper.Drop(e.Data, e.GetPosition(this), e.Effects);
            if (e.Effects != DragDropEffects.Copy) return;
            var files = e.Data.GetData(DataFormats.FileDrop, true) as string[];
            if (files == null || files.Length == 0) return;
            SetYin(files[0]);
        }

        private void SetYang(string path)
        {
            YangImage.Source = new BitmapImage(new Uri(yangPath = path, UriKind.Absolute));
            YangDescription.Visibility = Visibility.Collapsed;
        }
        private void BrowseYang(object sender, RoutedEventArgs e)
        {
            imageBrowser.Title = "选择一张以白色为背景的图片";
            if (imageBrowser.ShowDialog() == true) SetYang(imageBrowser.FileName);
        }
        private void OnYangDragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = e.AllowedEffects & DragDropEffects.Copy;
                DropTargetHelper.DragEnter(this, e.Data, e.GetPosition(this), e.Effects, "阳");
            }
            else
            {
                e.Effects = DragDropEffects.None;
                DropTargetHelper.DragEnter(this, e.Data, e.GetPosition(this), e.Effects);
            }
        }
        private void OnYangDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop, true) ? e.AllowedEffects & DragDropEffects.Copy : DragDropEffects.None;
            DropTargetHelper.Drop(e.Data, e.GetPosition(this), e.Effects);
            if (e.Effects != DragDropEffects.Copy) return;
            var files = e.Data.GetData(DataFormats.FileDrop, true) as string[];
            if (files == null || files.Length == 0) return;
            SetYang(files[0]);
        }

        private void Crop(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(yinPath) || string.IsNullOrWhiteSpace(yangPath) || imageSaver.ShowDialog() != true) return;
            IsEnabled = false;
            Loader.BeginAnimation(OpacityProperty, new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(0.5))));
            var failed = false;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                Cropper.Crop(yinPath, yangPath).Save(imageSaver.FileName);
            }
            catch (Exception exc)
            {
                failed = true;
                MessageBox.Show(exc.Message, "抠图失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsEnabled = true;
                Loader.BeginAnimation(OpacityProperty, new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.5))));
            }
            if (!failed) MessageBox.Show("成功搞定！耗时：" + stopwatch.ElapsedMilliseconds + "ms", "抠图成功",
                                         MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}