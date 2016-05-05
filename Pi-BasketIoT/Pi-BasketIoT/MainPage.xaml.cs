using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using Newtonsoft.Json;
using Windows.Devices.Gpio;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pi_BasketIoT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        int timesTicked;

        UltrasonicSensor sensor;

        public MainPage()
        {
            this.InitializeComponent();

            //inisialisasi id
            TBID.Text = IoTInfo.Id.ToString();
            if (string.IsNullOrEmpty(IoTInfo.Id.ToString()) || IoTInfo.Id <= 0)
            {
                ButGetID.IsEnabled = true;
            }
            else
            {
                ButGetID.IsEnabled = false;
            }

            ButGetID.Click += ButGetID_Click;


            ////inisialisasi sensor
            //sensor = new UltrasonicSensor(23, 24);
            //FirstRequest();
        }

        public void FirstRequest()
        {
            IoTInfo.DistanceBefore = sensor.GetDistance();
            TBBefore.Text = IoTInfo.DistanceBefore.ToString();
        }

        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += T_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            //IsEnabled should now be true after calling start
        }

        private void T_Tick(object sender, object e)
        {
            timesTicked++;
            textBlock.Text = "Before : "+ IoTInfo.DistanceBefore+"       After : "+IoTInfo.DistanceAfter;

            IoTInfo.DistanceAfter = sensor.GetDistance();
            TBAfter.Text = IoTInfo.DistanceAfter.ToString();
            if (IoTInfo.DistanceBefore > IoTInfo.DistanceAfter*100)
            {
                IoTInfo.Score++;
                SetScore();
                TBScore.Text = IoTInfo.Score.ToString();
            }
        }

        private void ButGetID_Click(object sender, RoutedEventArgs e)
        {
            RegisterIoT();
        }
        private void RegisterIoT()
        {
            var client = new HttpClient();
            var iot = new IoT();
            iot.Score = 0;
            var result = client.PostAsJsonAsync($"http://apibasket.azurewebsites.net/api/IoTs", iot).Result;

            IoT GetIoT = JsonConvert.DeserializeObject<IoT>(result.Content.ReadAsStringAsync().Result);

            IoTInfo.Id = GetIoT.Id;
            TBID.Text = IoTInfo.Id.ToString();
            IoTInfo.Score = GetIoT.Score;

        
            //mulai menghitung
            DispatcherTimerSetup();
            return;
        }
        private void SetScore()
        {
            var client = new HttpClient();
            var iot = new IoT();
            iot.Id = IoTInfo.Id;
            iot.Score = IoTInfo.Score;

            var response = client.PutAsJsonAsync($"http://apibasket.azurewebsites.net/api/IoTs/{iot.Id}", iot).Result;

        }
    }
}
