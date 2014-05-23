using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fracktory
{
    class SerialConsole
    {
        public SerialPort Port;
        public String Reply;

        public static String[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public Boolean TryPortOpen(string[] args)
        {
            try
            {
                Port = new SerialPort(args[0]);
                Port.BaudRate = int.Parse(args[1]);
                Port.DataReceived += port_DataReceived;
                Port.Open();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public void PortClose()
        {
            Port.Close();
        }

        public Boolean SendMessage(string Message)
        {
            if (Port.IsOpen)
            {
                Port.Write(Message + "\n");
                return true;
            }
            return false;
        }

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Reply = (sender as SerialPort).ReadExisting();
        }
    }
}

