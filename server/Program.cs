using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            
            TcpListener serverSocket = new TcpListener(8888);
            int requestCount = 0;
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine(" >> Server Started");
            clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine(" >> Accept connection from client");
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[66536];
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                    Program obj = new Program();
                    
                    string getData = obj.GetCurveValueWithIndex(dataFromClient);
                    Console.WriteLine(" >> Data from client - " + dataFromClient);
                   
                    string serverResponse = getData;

                    Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine(" >> " + serverResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> exit");
            Console.ReadLine();
        }
        
        public string GetCurveValueWithIndex(string selectedCurve)
        {
            string directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string newPath = Path.Combine(directory, "DATA.csv");            
            // using (var reader = new StreamReader(@"D:\project\PetrolinkSocketProject\PetrolinkSocketProject\DataFile\DATA.csv"))
            using (var reader = new StreamReader(newPath))
            {
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    listA.Add(values[0]);
                }
                //selectedCurve = "A,C";
                string[] selectedCurveArray = selectedCurve.Split(',').ToArray();
                string[] curveNames = listA[0].Split(',');
                listB.Add("-");
                for (int i = 1; i < listA.Count; i++)
                {
                    string[] valuecurve = listA[i].ToString().Split(',');
                    listB.Add(valuecurve[0].ToString());
                }
                listB.Add("-");

                foreach (string item in selectedCurveArray)
                {
                    int k = 0;
                    foreach (object curve in curveNames)
                    {
                        k++;
                        if (item.Equals(curve.ToString()))
                        {
                            for (int i = 1; i < listA.Count; i++)
                            {
                                string[] valuecurve = listA[i].ToString().Split(',');
                                listB.Add(valuecurve[k - 1].ToString());
                            }
                            listB.Add("-");
                        }
                    }
                }
                string combindedString = string.Join(",", listB.ToArray());
                return combindedString;
            }

        }
    }
}
