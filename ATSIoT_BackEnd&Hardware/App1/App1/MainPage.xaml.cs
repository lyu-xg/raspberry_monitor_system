using System;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.Diagnostics;
using Emmellsoft.IoT.Rpi.SenseHat;
using Emmellsoft.IoT.Rpi.SenseHat.Fonts.SingleColor;
using Microsoft.Azure.Devices.Client;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using System.Text;
using App1.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        private MainViewModel _vm;

        public MainPage()
        {
            this.InitializeComponent();
            _vm = this.DataContext as MainViewModel;

            Loaded += async (sender, args) =>
            {
                // send device connected message
                await _vm.checkAzureConnection();

                // receive remote light control events
                await _vm.ReceiveCloudToDeviceMessageAsync();
            };
        }

        

        private void ClickMe_Click(object sender, RoutedEventArgs e)
        {
            int MeansurementNumber = 20;
            for (int i = 1; i <= MeansurementNumber; ++i)
            {
                //Task taskAll = Task.Run(() => TheWholeThing(i));
                //taskAll.Wait();
                //_waitEvent.Wait(TimeSpan.FromMilliseconds(1000));
            }
            //HelloMessage.Text = MeansurementNumber.ToString()+" measurements finished! Click to restart.";
        }


    }

}
