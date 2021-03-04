using System;
using System.IO.Ports;


namespace EC_test_app2
{
    enum SerialConnectionStatus
    {
        Disconnected,
        Connected,
        Error,
        Timeout
    }
    class SerialConnection
    {     
        public string connectedPort = null;
        public string connectedDevice = null;

        private static SerialPort _serialPort = new SerialPort();
        private string[] portnames;
        private string message = null;
        private SerialConnectionStatus status = SerialConnectionStatus.Disconnected;

        public string[] SearchForConnectedDevices()
        {
            portnames = SerialPort.GetPortNames();
            return portnames;
        }

        public SerialConnectionStatus ConnectToSerial(string portName)
        {
            if (status == SerialConnectionStatus.Disconnected)
            {
                _serialPort.PortName = portName;
                _serialPort.BaudRate = 115200;
                _serialPort.ReadTimeout = 1500;
                _serialPort.Open();
                try
                {
                    _serialPort.DiscardInBuffer();
                    message = _serialPort.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                if (message != null)
                {
                    if (message.Contains("EC-Test SN:"))
                    {
                        _serialPort.Write("x");
                        status = SerialConnectionStatus.Connected;
                        connectedPort = portName;
                        connectedDevice = message;
                        return SerialConnectionStatus.Connected;
                    }                  
                }
                _serialPort.Close();
                return SerialConnectionStatus.Error;
            }
            else
            {
                return SerialConnectionStatus.Error;
            }
        }

        public string SerialMessageReceiver()
        {            
                string buf = _serialPort.ReadLine();
                string channel = buf.Substring(0, buf.IndexOf("="));
                string value = buf.Substring(3, buf.IndexOf("\r") - 3);
                                
                return value + "\n";            
        }
    
        public string SerialMessageTransmitter()
        {
            //TODO make a serialWriter
            return "It went fine";
        }       

        public SerialConnectionStatus DisconnectFromSerial()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Write("q");
                status = SerialConnectionStatus.Disconnected;
                connectedPort = null;
                connectedDevice = null;
                _serialPort.Close();
                return status;
            }
            return SerialConnectionStatus.Error;
        }

        public string getConnectedDevice()
        {
            return connectedDevice;
        }

        public SerialConnectionStatus getConnectionsStatus() 
        {
            return status;
        }

        ~SerialConnection()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }
        }
    }
}
