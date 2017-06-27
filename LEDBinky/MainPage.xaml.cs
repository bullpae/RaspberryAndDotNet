using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace LEDBinky
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        GpioController gpio;
        GpioPin ledPin;
        GpioPin swPin;
        DispatcherTimer tmrBlink;
        bool isBlink = true;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            gpio = GpioController.GetDefault();

            ledPin = gpio.OpenPin(5);
            ledPin.Write(GpioPinValue.High);
            ledPin.SetDriveMode(GpioPinDriveMode.Output);

            swPin = gpio.OpenPin(4);
            swPin.SetDriveMode(GpioPinDriveMode.Input);
            swPin.ValueChanged += SwPin_ValueChanged;
            swPin.DebounceTimeout = TimeSpan.FromMilliseconds(30); // 0.03 sec 이내 변화시
          
            tmrBlink = new DispatcherTimer();
            tmrBlink.Interval = TimeSpan.FromMilliseconds(500); // 0.5 sec
            tmrBlink.Tick += TmrBlink_Tick;
            tmrBlink.Start(); // Start Timer

            //GpioController gpio = GpioController.GetDefault();

            //if (gpio == null)
            //{
            //    return;
            //}

            //for (int i = 0; i < 10; i++)
            //{
            //    using (GpioPin pin = gpio.OpenPin(5))
            //    {
            //        pin.Write(GpioPinValue.High);
            //        pin.SetDriveMode(GpioPinDriveMode.Output);
            //    }
            //}
        }

        private void SwPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge)
            {
                // switch down
                //ToggleLED();

                // Cross Thread 문제가 발생!!
                if (isBlink == true)
                {
                    tmrBlink.Stop(); 
                }
                else
                {
                    tmrBlink.Start();
                }

                isBlink = !isBlink;
            }
            //throw new NotImplementedException();
        }

        private void TmrBlink_Tick(object sender, object e)
        {
            ToggleLED();

            //ledPin.SetDriveMode(GpioPinDriveMode.Output);
            //throw new NotImplementedException();
        }

        private void ToggleLED()
        {
            if (ledPin.Read() == GpioPinValue.High)
            {
                ledPin.Write(GpioPinValue.Low);
            }
            else
            {
                ledPin.Write(GpioPinValue.High);
            }
        }
    }
}
