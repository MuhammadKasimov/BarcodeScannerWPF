using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BarcodeReaer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;
        public MainWindow()
        {
            InitializeComponent();
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (FilterInfo filter in filterInfoCollection)
                CameraChooser.Items.Add(filter.Name);
            CameraChooser.SelectedIndex = 0;
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[CameraChooser.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += CaptureDeviceNewFrame;
            videoCaptureDevice.Start();
        }

        private void CaptureDeviceNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

            //List<BarcodeFormat> formats = new List<BarcodeFormat>() { BarcodeFormat.All_1D};
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Bmp);
            var reader = new ZXing.Windows.Compatibility.BarcodeReader();
            //{
            //    AutoRotate = true,
            //    Options =
            //    {
            //        TryInverted = true,
            //        PossibleFormats = formats,
            //        TryHarder = true,
            //        ReturnCodabarStartEnd = true,
            //        PureBarcode = false
            //    }
            //};

            var result = reader.Decode(bitmap);

            if (result != null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    BarcodeTxt.Text = result.ToString();
                });
            }

            if (videoCaptureDevice.IsRunning)
            {
                this.Dispatcher.Invoke(() =>
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    ContentViewer.Source = image;
                });

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            videoCaptureDevice.Stop();
        }
    }
}
