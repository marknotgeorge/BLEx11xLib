using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bluegiga;
using System.ComponentModel;


namespace BLED112Lib
{
    
    /// <summary>
    /// A minimal wrapper for the BGLib port by Jeff Rowberg. Sets up a serial port and  handles reading 
    /// and writing to and from the serial port.
    /// </summary>
    public partial class BLEDevice
    {
        private SerialPort serialPort;
        private BGLib bgLib;

        public BLEDevice()
        {
            BGLib = new BGLib();             
        }

        public BGLib BGLib
        {
            get
            {
                return bgLib;
            }
            set
            {
                bgLib = value;
            }
        }

        public void SetPort(string port)
        {
            serialPort = new SerialPort();
            serialPort.Handshake = Handshake.RequestToSend;
            serialPort.BaudRate = 115200;
            serialPort.PortName = port;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            serialPort.DataReceived += serialPort_DataReceived;
        } 

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            System.IO.Ports.SerialPort sp = (System.IO.Ports.SerialPort)sender;
            Byte[] inData = new Byte[sp.BytesToRead];

            // read all available bytes from serial port in one chunk
            sp.Read(inData, 0, sp.BytesToRead);

            parse(inData);
        }

        private bool isBusy = false;

        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; }
        }

        private bool packetMode;

        public bool PacketMode
        {
            get { return packetMode; }
            set { packetMode = value; }
        }

        public bool IsOpen
        {
            get
            {
                return serialPort.IsOpen;
            }
        }      
        

        /// <summary>
        /// Parses inData according to the BGAPI packet structure
        /// </summary>
        /// <param name="inData">Array of bytes holding the packet.</param>
        private void  parse(Byte[] inData)
        {
            
            for (int i = 0; i < inData.Length; i++)
            {
                bgLib.Parse(inData[i]);
            }
        }

        public void Open()
        {
            if (serialPort != null && !serialPort.IsOpen)
            {
                serialPort.Open();
            }
        }

        public void Close()
        {
            serialPort.Close();
        }

        public void SendCommand(Byte[] command)
        {
            if (serialPort.IsOpen)
            {
                if (PacketMode)
                    serialPort.Write(new Byte[] { (Byte)command.Length }, 0, 1);
                serialPort.Write(command, 0, command.Length);
            }
        }
      
        public Byte[] CreateAdvertisingLocalNameData(string myName)
        {
            Byte[] returnData = new Byte[myName.Length + 2];

            // Length
            returnData[0] = (Byte)(myName.Length + 2);

            // Field type
            returnData[1] = (Byte)GAPAdType.LocalNameComplete;
            for (int i = 0; i < myName.Length; i++)
            {
                returnData[i + 2] = (Byte) myName[i];
            }

            return returnData;
        }
    }
}
