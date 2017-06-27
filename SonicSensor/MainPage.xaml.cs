using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
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

namespace SonicSensor
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        GpioController gpio;
        GpioPin triggerPin, echoPin;
        DispatcherTimer tmrTrigger;

        Stopwatch stopWatch = new Stopwatch();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            gpio = GpioController.GetDefault();
            triggerPin = gpio.OpenPin(23);
            echoPin = gpio.OpenPin(24);

            triggerPin.Write(GpioPinValue.Low);
            triggerPin.SetDriveMode(GpioPinDriveMode.Output);
            
            echoPin.SetDriveMode(GpioPinDriveMode.Input);
            echoPin.ValueChanged += EchoPin_ValueChanged;

            tmrTrigger = new DispatcherTimer();
            tmrTrigger.Interval = TimeSpan.FromMilliseconds(500); // 0.5 sec
            tmrTrigger.Tick += TmrTrigger_Tick; ;
            tmrTrigger.Start(); 

        }

        private void TmrTrigger_Tick(object sender, object e)
        {
            ManualResetEventSlim mre = new ManualResetEventSlim();

            // 트리거 발생
            triggerPin.Write(GpioPinValue.High);
            //Task.Delay(1); // 10마이크로 세크가 필요해서 다른 것을 사용
            mre.Wait(TimeSpan.FromMilliseconds(0.01));
            triggerPin.Write(GpioPinValue.Low);

            //throw new NotImplementedException();
        }

        private void EchoPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.RisingEdge)
            {
                // 펄스의 시작
                stopWatch.Reset();
                stopWatch.Start();
            }
            else
            {
                stopWatch.Stop();
                double width = stopWatch.Elapsed.TotalSeconds;
                double distance = width * 17000;
                Debug.WriteLine($"측정거리:{distance}cm");
            }
            //throw new NotImplementedException();
        }
    }
}
