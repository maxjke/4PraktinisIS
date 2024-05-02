﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace _4Praktinis1App
{
    public class Program
    {
        private static RSAParameters _privateKey;
        private static RSAParameters _publicKey;

        public static void Main()
        {
            try
            {
                AssignNewKey();

                IPAddress ipAddress = IPAddress.Loopback;
                int port = 11000;
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

                using (Socket client = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    client.Connect(ipEndPoint);

                    Console.WriteLine("Iveskite Teksta");
                    string message = Console.ReadLine();
                    var signature = SignData(message);
                    var publicKeyXml = ExportPublicKey();
                    var fullMessage = $"{message}:{Convert.ToBase64String(signature)}:{publicKeyXml}";
                    var messageBytes = Encoding.UTF8.GetBytes(fullMessage);

                    client.Send(messageBytes);
                    Console.WriteLine("Zinute, parasas, ir viesasis raktas issiusti.");


                    // client.Shutdown(SocketShutdown.Send);
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void AssignNewKey()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                _privateKey = rsa.ExportParameters(true);
                _publicKey = rsa.ExportParameters(false);
            }
        }

        private static byte[] SignData(string data)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(_privateKey);
                var dataBytes = Encoding.UTF8.GetBytes(data);
                return rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        private static string ExportPublicKey()
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(_publicKey);
                return rsa.ToXmlString(false);
            }
        }
    }
}
