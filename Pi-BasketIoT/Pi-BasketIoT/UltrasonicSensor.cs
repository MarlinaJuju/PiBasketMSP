using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Pi_BasketIoT
{
    public class UltrasonicSensor
    {
        GpioController gpio = GpioController.GetDefault();

        GpioPin TriggerPin;
        GpioPin EchoPin;
        Stopwatch pulseLength;

        public UltrasonicSensor(int TriggerPin, int EchoPin)
        {
            this.TriggerPin = gpio.OpenPin(TriggerPin);
            this.EchoPin = gpio.OpenPin(EchoPin);

            this.TriggerPin.SetDriveMode(GpioPinDriveMode.Output);
            this.EchoPin.SetDriveMode(GpioPinDriveMode.Input);

            this.TriggerPin.Write(GpioPinValue.Low);

            pulseLength = new Stopwatch();
        }

        public double GetDistance()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            mre.WaitOne(500);

            pulseLength.Reset();

            //Send pulse
            this.TriggerPin.Write(GpioPinValue.High);
            mre.WaitOne(TimeSpan.FromMilliseconds(0.01));
            this.TriggerPin.Write(GpioPinValue.Low);

            //Recieve pusle
            while (this.EchoPin.Read() == GpioPinValue.Low)
            {
            }
            pulseLength.Start();


            while (this.EchoPin.Read() == GpioPinValue.High)
            {
            }
            pulseLength.Stop();

            //Calculating distance
            TimeSpan timeBetween = pulseLength.Elapsed;
            Debug.WriteLine(timeBetween.ToString());
            double distance = timeBetween.TotalSeconds * 17000;

            return distance;
        }

    }
}
