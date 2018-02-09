using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyAdb.Entity
{
    public class Device
    {
        private const string PRODUCT = "product:";
        private const string MODEL = "model:";

        public string Id;
        public string Type;
        public string Product;
        public string Model;

        public Device(string raw)
        {
            string[] splited = Regex.Split(raw, @"\s+").Where(s => s != string.Empty).ToArray();

            // Old devices: ~ 4.0
            // Alesy's ZTE 880e: P772A10D_RU            device
            if (splited.Length == 2)
            {
                Id = splited[0];
                Type = splited[1];
                Model = Id;
                return;
            }

            //$ adb devices -l
            //List of devices attached
            //192.168.0.54:5555      device product:nakasi model:Nexus_7 device:grouper
            //0123456789ABCDEF       device product:P188F03 model:ZTE_V965 device:P188F03
            if (splited.Length != 5)
                return;

            if (!splited[2].StartsWith(PRODUCT))
                return;

            if (!splited[3].StartsWith(MODEL))
                return;

            Id = splited[0];
            Type = splited[1];
            Product = splited[2].Substring(PRODUCT.Length);
            Model = splited[3].Substring(MODEL.Length);
        }

        public Boolean IsEmpty()
        {
            return string.IsNullOrEmpty(Id) && string.IsNullOrEmpty(Type) && string.IsNullOrEmpty(Model) && string.IsNullOrEmpty(Product);
        }

        public override string ToString()
        {
            return string.Format("Id: {0}. Type: {1}. Product: {2}. Model: {3}", Id, Type, Product, Model);
        }
    }
}