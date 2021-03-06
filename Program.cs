using System;
using System.Diagnostics;
using System.IO;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.UI.Notifications;
namespace MicroBitAdv {
    class Program {
        private static ulong oldinstanceid = 0;
        private static ulong oldthreshold = 0;
        private const string TOAST_TITLE = "玄関チャイム通知システム";
        private const string TOAST_MESSAGE = "呼び鈴が鳴っています";
        private const uint MSG_NAME_SPACE       = 0x80808080;
        private const uint THRESHOLD_NAME_SPACE = 0x80808081;
        private static string MyToastTitle;
        private static string MyToastMessage;
        private static ulong MyBtAddress;

        static void Main(string[] args) {
            MyToastTitle = TOAST_TITLE;
            MyToastMessage = TOAST_MESSAGE;
            MyBtAddress = 0;
            if (args.Length > 0) {
                try {
                    MyBtAddress = ulong.Parse(args[0], System.Globalization.NumberStyles.HexNumber);
                    Debug.WriteLine($"BtAddress:{MyBtAddress:X}");
                    if (args.Length >= 2) {
                        MyToastTitle = args[1];
                        Debug.WriteLine($"Title:{MyToastTitle}");
                    }
                    if (args.Length >=3) {
                        MyToastMessage = args[2];
                        Debug.WriteLine($"Message:{MyToastMessage}\n");
                    }
                }
                catch (Exception) {
                    usage();
                }
            } else {
                // No Args
                usage();
            }
            BluetoothLEAdvertisementWatcher MyWatcher;
            MyWatcher = new BluetoothLEAdvertisementWatcher();
            MyWatcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(100);
            MyWatcher.ScanningMode = BluetoothLEScanningMode.Passive;
            MyWatcher.AdvertisementFilter = new BluetoothLEAdvertisementFilter() {
                Advertisement = new BluetoothLEAdvertisement { ServiceUuids = { new Guid("0000FEAA-0000-1000-8000-00805F9B34FB") } } };
            MyWatcher.Received += WatcherReceived;
            // start watch
            MyWatcher.Start();
            Console.WriteLine("----Exit with Any key.");
            Console.ReadKey();
            MyWatcher.Stop();
        }
        private static void usage() {
            Console.WriteLine("Usage: {0} Bluetooth_Address [Toast_Title [Toast_Message]]\n\tBluetooth_Address is Hexdecimal(0-9A-F)\n\t", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
            Environment.Exit(1);
        }
        private static void WatcherReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args) {
            if (args.BluetoothAddress != MyBtAddress) { return; }
            BluetoothLEAdvertisementDataSection x = args.Advertisement.GetSectionsByType(22)[0];
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(x.Data);
            var r = new Byte[x.Data.Length];
            var MyNameSpace = new Byte[4];
            var MyInstaceID = new Byte[8];
            dataReader.ReadBytes(r);
            Array.Copy(r,10, MyNameSpace, 0,4);
            Array.Copy(r,14, MyInstaceID, 0,6);
            //for (var i = 0; i < 10; i++) { MyNameSpace[i] = r[i + 4]; }
            //for (var i = 0; i < 6; i++) { MyInstaceID[i] = r[14 + i]; }
            Array.Reverse(MyInstaceID);     //big-endian to little-endian
//            int new_namespace = BitConverter.ToInt32(MyNameSpace, 6);
            int new_namespace = BitConverter.ToInt32(MyNameSpace, 0);
            ulong instanceid = BitConverter.ToUInt64(MyInstaceID, 0) / 0x10000;
            var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var textNodes = template.GetElementsByTagName("text");
            var notifier = ToastNotificationManager.CreateToastNotifier(MyToastTitle);
            if (((uint)new_namespace) == MSG_NAME_SPACE) {
                // This is message Advertise
                if ( instanceid == oldinstanceid) {
                    // Repeat Same Advertise
                    //Console.WriteLine("Same Adv");
                } else {
                    // New Advertise
                    // It's Chime
                    Console.WriteLine($"Ring {instanceid:X10} {DateTime.Now.ToString("dddd HH:mm:ss")}");
                    textNodes.Item(0).InnerText = MyToastMessage;
                    var notification = new ToastNotification(template);
                    notifier.Show(notification);
                    // Replace Value
                    oldinstanceid = instanceid;
                }
            } else {
                if (instanceid == oldthreshold) {
                    // repeat message
                } else {
                    // This is Threshold Advertise
                    Console.WriteLine($"Threshold Change {instanceid:X2}");
                    oldthreshold = instanceid;
                }
            }
            
        }
    }
}
