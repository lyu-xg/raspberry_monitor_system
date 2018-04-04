using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Windows.Storage;
using System.Threading;
using Emmellsoft.IoT.Rpi.SenseHat;
using Emmellsoft.IoT.Rpi.SenseHat.Fonts.SingleColor;
using Windows.Storage.Streams;

namespace App1.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region junk
        //connection information
        /*
        public static string deviceID = "Raspberry03";
        public static string primaryKey = "Tdbpt69m33s/jIxey3YBE+FdGMtb3GXF66L8Z7c5quE=";
        public static DeviceClient deviceClient = DeviceClient.Create(hostName,
                AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(deviceID, primaryKey), TransportType.Http1);
        */
        //private myBuffer localDataBuffer = new myBuffer("testing.txt");
        //private static int timeResetInterval = 20; // Re-Download DateTime after how many times of uploading data
        //private static int uploadRate = 1; // how many data are uploaded every second
        #endregion

        #region System and Communication Settings
        public static string timeStampFormat = "yyyy-MM-ddTHH:mm:ss.fffffffZ";
        public static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        public static string fileName = "testing.txt";
        public static ManualResetEventSlim _waitEvent = new ManualResetEventSlim(false);
        public static DateTime realStartTime;
        public static DateTime deviceStartTime;
        private class CustomData
        {
            public string timestamp;
            public string value;
            public string dataType;
            public string sensorID;
        }
        #endregion

        #region Azure IoT Hub Settings

        public DeviceClient DeviceClient { get; }

        public string IotHubUri { get; } = "UCI.azure-devices.net";

        public string DeviceKey { get; } = "Tdbpt69m33s/jIxey3YBE+FdGMtb3GXF66L8Z7c5quE=";

        public string DeviceId => "Raspberry03";
        public string sensorID = "SenseHat03";

        #endregion

        #region User Interface Display Fields

        private bool _isAzureConnected;
        private string _cloudToDeviceLog;

        public bool IsAzureConnected
        {
            get { return _isAzureConnected; }
            set { _isAzureConnected = value; RaisePropertyChanged(); }
        }

        public string CloudToDeviceLog
        {
            get { return _cloudToDeviceLog; }
            set { _cloudToDeviceLog = value; RaisePropertyChanged(); }
        }

        #endregion

        public MainViewModel()
        {
            DeviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey));
            
        }


        public async Task ReceiveCloudToDeviceMessageAsync()
        {
            CloudToDeviceLog = "Data upload startd, also listening for cloud-to-device command...";
            var msg = "on";
            var theIndex = 0;
            while (true)
            {
                if (theIndex % 10 == 0) getCentralTime();
                if (msg == "on")
                {
                    Task<List<string>> taskAll = Task.Run(() => TheWholeThing(theIndex));
                    taskAll.Wait();
                    foreach (var i in taskAll.Result) {
                        CloudToDeviceLog = "Upload message failed, message is saved locally: " + taskAll.Result[0] + taskAll.Result[1] + CloudToDeviceLog;
                    }
                    ++theIndex;
                }
                
                Message receivedMessage = await DeviceClient.ReceiveAsync();

                if (receivedMessage == null) continue;

                msg = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                Debug.WriteLine(msg);
                CloudToDeviceLog = "Received command: measurement upload " + msg +"\n"+ CloudToDeviceLog;
                
                await DeviceClient.CompleteAsync(receivedMessage);
            }
        }


        private async Task<List<string>> TheWholeThing(int theIndexNumberWhichIsCool)
        {
            StorageFile file = await storageFolder.CreateFileAsync(fileName,
                    CreationCollisionOption.OpenIfExists);
            ISenseHat senseHat = await SenseHatFactory.GetSenseHat().ConfigureAwait(false);
            ISenseHatDisplay display = senseHat.Display;

            var exceptionSave = new List<string>();

            //Connect to the senseHat & Clear the display
            var tinyFont = new TinyFont();
            string myText = theIndexNumberWhichIsCool.ToString();
            if (myText.Length > 2) myText = myText.Substring(0, 2);
            for (int i = 0; i < 5; ++i)
            {
                display.Clear();
                tinyFont.Write(display, myText, Windows.UI.Colors.Green);
                display.Update();
                //_waitEvent.Wait(TimeSpan.FromMilliseconds(100));
                display.Clear();
                //display.Update();
                //_waitEvent.Wait(TimeSpan.FromMilliseconds(200));
            }

            //Reading Temperature
            CustomData myTemperatureReading = new CustomData
            {
                value = "",
                timestamp = "",
                dataType = "te",
                sensorID = sensorID
            };
            senseHat.Sensors.HumiditySensor.Update();
            for (int i = 0; i <= 3; ++i)
            {
                if (senseHat.Sensors.Temperature.HasValue)
                {
                    myTemperatureReading.timestamp = getTimeNow().ToString(timeStampFormat);
                    var ttt = senseHat.Sensors.Temperature.Value;
                    myTemperatureReading.value = ttt.ToString();
                    break;
                }
                //else _waitEvent.Wait(TimeSpan.FromMilliseconds(100));
            }
            string temperatureReadingString = JsonConvert.SerializeObject(myTemperatureReading);
            
            //Read Acceleration
            CustomData myAccelerationReading = new CustomData
            {
                value = "",
                timestamp = "",
                dataType = "ac",
                sensorID = sensorID
            };
            senseHat.Sensors.ImuSensor.Update();
            for (int i = 0; i <= 3; ++i)
            {
                if (senseHat.Sensors.Acceleration.HasValue)
                {
                    myAccelerationReading.timestamp = getTimeNow().ToString(timeStampFormat);
                    myAccelerationReading.value = senseHat.Sensors.Acceleration.Value.ToString();
                    break;
                }
                //else _waitEvent.Wait(TimeSpan.FromMilliseconds(100));
            }
            string accelerationReadingString = JsonConvert.SerializeObject(myAccelerationReading);

            //send data to Azure
            var message = new Message(Encoding.ASCII.GetBytes(temperatureReadingString));

            try
            {
                await DeviceClient.SendEventAsync(message);
            }
            catch
            {
                Debug.WriteLine("Sending temperature data to cloud failed.");
                await FileIO.AppendTextAsync(file, temperatureReadingString);
                exceptionSave.Add(temperatureReadingString);
            }
            message = new Message(Encoding.ASCII.GetBytes(accelerationReadingString));

            try
            {
                await DeviceClient.SendEventAsync(message);
            }
            catch
            {
                Debug.WriteLine("Sending acceleration data to cloud failed.");
                await FileIO.AppendTextAsync(file, accelerationReadingString);
                exceptionSave.Add(accelerationReadingString);
            }
            
            //Reading
            using (IRandomAccessStream textStream = await file.OpenReadAsync())
            {
                using (DataReader textReader = new DataReader(textStream))
                {
                    uint textLength = (uint)textStream.Size;
                    await textReader.LoadAsync(textLength);
                    Debug.WriteLine(textReader.ReadString(textLength));
                }
            }

            return exceptionSave;
        }
        

        public async Task checkAzureConnection()
        {
            try
            {
                var telemetryDataPoint = new
                {
                    deviceId = DeviceId,
                    message = "Hello"
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await DeviceClient.SendEventAsync(message);
                Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                IsAzureConnected = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static DateTime getTimeNow()
        {
            long ticks = DateTime.UtcNow.Ticks - deviceStartTime.Ticks;
            TimeSpan runTime = new TimeSpan(ticks);
            //Debug.WriteLine("Run Time", ticks.ToString());
            return realStartTime + runTime;
        }

        private async void getCentralTime()
        {
            //Get the Real Time, only try three times
            for (int i = 0; i < 3; ++i)
            {
                try
                {
                    while (true)
                    {
                        var client = new Yort.Ntp.NtpClient();
                        var currentTime = await client.RequestTimeAsync();
                        if (currentTime != null)
                        {
                            Debug.WriteLine(currentTime);
                            deviceStartTime = DateTime.UtcNow;
                            realStartTime = (DateTime)currentTime;
                            break;
                        }
                    }
                    Debug.WriteLine("Try to get NTP time succeed");
                    break;
                }
                catch
                {
                    Debug.WriteLine("Try to get NTP time failed.");
                }
            }
        }
    }
}
