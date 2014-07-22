using BLED112Lib;
using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;


namespace BLEProgramming
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BleDevice = new BLEDevice();

            // Set up the serial ports...
            string[] serialPorts = SerialPort.GetPortNames();
            PortCombo.ItemsSource = serialPorts;

            // ...BGLib event handlers...            
            BleDevice.BGLib.BLEEventConnectionStatus += BGLib_BLEEventConnectionStatus;
            BleDevice.BGLib.BLEEventConnectionDisconnected += BGLib_BLEEventConnectionDisconnected;
            

            // ...and the timer - once a second.
            tickTimer = new DispatcherTimer();
            tickTimer.Interval = TimeSpan.FromSeconds(1);
            tickTimer.Tick += tickTimer_Tick;
        }

        // Using a DependencyProperty as the backing store for HeartRate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeartRateProperty =
            DependencyProperty.Register("HeartRate", typeof(byte), typeof(MainWindow), new PropertyMetadata((byte)0));

        // Using a DependencyProperty as the backing store for IsAdvertising.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAdvertisingProperty =
            DependencyProperty.Register("IsAdvertising", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for IsConnected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsConnectedProperty =
            DependencyProperty.Register("IsConnected", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for IsEncrypted.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEncryptedProperty =
            DependencyProperty.Register("IsEncrypted", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public BLEDevice BleDevice;

        // The heart rate measurement handle.
        private const UInt16 c_heart_rate_measurement = 15;

        // The connection ID. Used when disconnecting.
        private byte connection;

        // Buffer for the heart rate characteristic. Only 2 bytes, as per gatt.xml
        private byte[] measureBuffer;

        private DispatcherTimer tickTimer;

        public byte HeartRate
        {
            get { return (byte)GetValue(HeartRateProperty); }
            set { SetValue(HeartRateProperty, value); }
        }

        public bool IsAdvertising
        {
            get { return (bool)GetValue(IsAdvertisingProperty); }
            set { SetValue(IsAdvertisingProperty, value); }
        }

        public bool IsConnected
        {
            get { return (bool)GetValue(IsConnectedProperty); }
            set { SetValue(IsConnectedProperty, value); }
        }

        public bool IsEncrypted
        {
            get { return (bool)GetValue(IsEncryptedProperty); }
            set { SetValue(IsEncryptedProperty, value); }
        }

        

        /// <summary>
        /// Called when the remote device disconnects.
        /// </summary>        
        private void BGLib_BLEEventConnectionDisconnected(object sender, Bluegiga.BLE.Events.Connection.DisconnectedEventArgs e)
        {
            // Run in Dispatcher.Invoke Lambda as the event is in a different thread to the UI...
            this.Dispatcher.Invoke(() =>
                {
                    DisconnectedReason reason = (DisconnectedReason)e.reason;
                    SendOutput("Device disconnected. Reason: " + reason.ToString());
                    StopTimer();
                    IsEncrypted = false;
                    IsConnected = false;
                    StartAdvertisingMode();
                });
        }

        
        /// <summary>
        /// Event handler for when the connection status changes.
        /// </summary>
        private void BGLib_BLEEventConnectionStatus(object sender, Bluegiga.BLE.Events.Connection.StatusEventArgs e)
        {
            ConnectionStatusFlags flags = (ConnectionStatusFlags)e.flags;

            // Run in Dispatcher.Invoke Lambda as the event is in a different thread to the UI...
            this.Dispatcher.Invoke(() =>
                {
                    if (flags.HasFlag(ConnectionStatusFlags.Connected))
                    {
                        //SendOutput("Device connected!");
                        connection = e.connection;
                        IsConnected = true;
                        IsAdvertising = false;
                    }
                    else if (flags.HasFlag(ConnectionStatusFlags.Encrypted))
                    {
                        //SendOutput("Connection encrypted!");
                        IsEncrypted = true;
                    }
                });
        }

        

        private void StartDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            SendOutput("Starting device...");

            // Enable watermark event on UART endpoint (REQUIRED for USB CDC-based DFU trigger)
            // I'm not sure if this is needed, but it's in the origial BGScript, so...
            BleDevice.SendCommand(BleDevice.BGLib.BLECommandSystemEndpointSetWatermarks(3, 1, 0));

            // Start advertising...
            StartAdvertisingMode();

            // Set bondable (so devices can pair to it). Windows 8.1 & Windows Phone 8.1 need this...
            BleDevice.SendCommand(BleDevice.BGLib.BLECommandSMSetBondableMode((byte)SMBondableMode.True));
            SendOutput("Device Started!");
        }

        /// <summary>
        /// Close the device
        /// </summary>        
        private void CloseDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            SendOutput("Disconnecting...");
            BleDevice.SendCommand(BleDevice.BGLib.BLECommandConnectionDisconnect(connection));
            StopTimer();
            BleDevice.Close();

        }        

        private void OutputDataButton_Click(object sender, RoutedEventArgs e)
        {
            StartTimer();
        }        

        private void PortCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PortCombo.SelectedIndex != -1)
            {
                string port = PortCombo.SelectedItem.ToString();
                BleDevice.SetPort(port);
                BleDevice.Open();
                SendOutput("Opening device on port " + port + "...");
                if (BleDevice.IsOpen)
                {
                    SendOutput("Device open!");                    
                    StartDeviceButton.IsEnabled = true;
                    OutputDataButton.IsEnabled = true;
                    CloseDeviceButton.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Write a line to the output area.
        /// </summary>    
        private void SendOutput(string output)
        {
            OutputArea.WriteLine(output);
            
        }

        /// <summary>
        /// Set the device to advertising mode. Used when device is not connected.
        /// </summary>
        private void StartAdvertisingMode()
        {
            BleDevice.SendCommand(BleDevice.BGLib.BLECommandGAPSetAdvParameters(32, 48, 7));
            BleDevice.SendCommand(BleDevice.BGLib.BLECommandGAPSetMode(
                (byte)GAPDiscoverableMode.GeneralDiscoverable,
                (byte)GAPConnectableMode.UndirectedConnectable));
            IsAdvertising = true;
        }

        private void StartTimer()
        {            
            HeartRate = 0;
            measureBuffer = new byte[2];
            measureBuffer[0] = 0;
            measureBuffer[1] = HeartRate;
            tickTimer.Start();
        }

        private void StopTimer()
        {
            tickTimer.Stop();
        }

        /// <summary>
        /// Each tick, add one to the heart rate, and write to the characteristic...
        /// </summary>        
        void tickTimer_Tick(object sender, EventArgs e)
        {
           this.Dispatcher.Invoke(() =>
                {
                    HeartRate++;
                    measureBuffer[1] = HeartRate;
                });

           BleDevice.SendCommand(BleDevice.BGLib.BLECommandAttributesWrite(c_heart_rate_measurement, 0, measureBuffer));           
        }
    }
}