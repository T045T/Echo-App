using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace NetworkTestApp_Moritz
{
    public static class Crypt
    {
        private static RSACryptoServiceProvider serverProvider;
        private static RSACryptoServiceProvider clientProvider;

        public static void init()
        {
            if (!loadKeys())
            {
                generateKeys();
            }
        }

        private static bool loadKeys()
        {
            return false;
        }

        private static void generateKeys()
        {
            serverProvider = new RSACryptoServiceProvider();
            clientProvider = new RSACryptoServiceProvider(1024);
            saveKeys();
        }

        private static void saveKeys()
        {

        }

        public static String getPublicKeyInXML()
        {
            return clientProvider.ToXmlString(false);
        }

        public static void setServerPublicKey(String xmlKey)
        {
            serverProvider.FromXmlString(xmlKey);
        }

        public static byte[] encrypt(byte[] data)
        {
            return serverProvider.Encrypt(data, false);
        }

        public static byte[] decrypt(byte[] data)
        {
            return clientProvider.Decrypt(data, false);
        }

        public static byte[] sign(byte[] data)
        {
            
            return clientProvider.SignData(data, new SHA1Managed());
        }

        public static bool verify(byte[] data, byte[] signature)
        {
            return serverProvider.VerifyData(data, new SHA1Managed(), signature);
        }
    }
}
