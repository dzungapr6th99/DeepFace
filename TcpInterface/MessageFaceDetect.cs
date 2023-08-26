
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TcpInterface
{
    public class MessageJson
    {
        /// <summary>
        ///ID người cần Detect - Tag 1
        /// </summary>
        public string ID;
        /// <summary>
        /// Ảnh ở dạng base64 tag 2
        /// </summary>
        public string Image; 
        /// <summary>
        /// Loại Model dùng để detect - Tag 3
        /// </summary>
        public string Model;
        /// <summary>
        /// Loại Metric dùng để detect - Tag 4
        /// </summary>
        public string Metric;
        /// <summary>
        /// Loại Detector dùng để detect - Tag 5
        /// </summary>
        public string Detector;

        public const ushort TAG_ID = 1;
        public const ushort TAG_IMAGE = 2;
        public const ushort TAG_MODEL = 3;
        public const ushort TAG_METRIC = 4;
        public const ushort TAG_DETECTOR = 5;
        public const ushort TAG_TOTAL_LENGTH = 99;
        public const string DELIMETER = "=";
        public byte[] Package;

        public MessageJson(string iD, string image, string model, string metric, string detector)
        {
            if (iD != null)
                ID = iD;
            else ID = string.Empty;
            if (image != null)
                Image = image;
            else Image = string.Empty;
            if (model != null)
                Model = model;
            else Model = string.Empty;
            if (metric != null) Metric = metric;
            else Metric = string.Empty;
            if (detector != null)
                Detector = detector;
            else Detector = string.Empty;
        }

        public byte[] BuildByte()
        {
            //Build theo chuẩn Tag - Length - Value, Header la total length
            //6 TAG dạng ushort 2 byte + 5 giá trị của length các tag dạng int 4 byte + tổng độ dài tất cả các tag
            int totalLength = 12 + 4*5 + ID.Length + Image.Length + Model.Length +Metric.Length + Detector.Length;
            Package = new byte[totalLength];
            int index = 0;
            Package.Merge(BitConverter.GetBytes(TAG_TOTAL_LENGTH), ref index);//bit 0 - bit 1
            Package.Merge(BitConverter.GetBytes(totalLength), ref index);// bit 2 - bit 5
            Package.Merge(BitConverter.GetBytes(TAG_ID), ref index); // byte 6- byte 7
            Package.Merge(BitConverter.GetBytes(ID.Length), ref index);//byte 8 - byte 11
            Package.Merge(Encoding.ASCII.GetBytes(ID), ref index);// byte 12 - byte 12 +lengthID
            Package.Merge(BitConverter.GetBytes(TAG_IMAGE), ref index);
            Package.Merge(BitConverter.GetBytes(Image.Length), ref index);
            Package.Merge(Encoding.ASCII.GetBytes(Image), ref index);
            Package.Merge(BitConverter.GetBytes(TAG_MODEL), ref index);
            Package.Merge(BitConverter.GetBytes(Model.Length), ref index);
            Package.Merge(Encoding.ASCII.GetBytes(Model), ref index);
            Package.Merge(BitConverter.GetBytes(TAG_METRIC), ref index);
            Package.Merge(BitConverter.GetBytes(Metric.Length), ref index);
            Package.Merge(Encoding.ASCII.GetBytes (Metric), ref index);
            Package.Merge(BitConverter.GetBytes(TAG_DETECTOR ), ref index);
            Package.Merge(BitConverter.GetBytes(Detector.Length), ref index);
            Package.Merge(Encoding.ASCII.GetBytes(Detector), ref index);
            return Package;
        }

        public void ParseByte(byte[] data)
        {
            Package = data;
            int length = BitConverter.ToInt32(Package, 2);
            int lengthTag;
            int index = 8;
            //Lấy Length của ID
            lengthTag = BitConverter.ToInt32(Package, index);
            ID = Encoding.ASCII.GetString(Package, index +4, lengthTag);
            index += 4 + lengthTag;
            lengthTag = BitConverter.ToInt32(Package, index);
            Image = Encoding.ASCII.GetString(Package, index + 4, lengthTag);
            index += 4 + lengthTag;
            lengthTag = BitConverter.ToInt32(Package, index);
            Model = Encoding.ASCII.GetString(Package, index + 4, lengthTag);
            index += 4 + lengthTag;
            lengthTag = BitConverter.ToInt32(Package, index);
            Metric = Encoding.ASCII.GetString(Package, index + 4, lengthTag);
            index += 4 + lengthTag;
            lengthTag = BitConverter.ToInt32(Package, index);
            Detector = Encoding.ASCII.GetString(Package, index + 4, lengthTag);
        }

        /*public string BuildString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(TAG_ID).Append(DELIMETER).Append(ID.Length).Append(ID);
            sb.Append(TAG_IMAGE).Append(DELIMETER).Append(Image.Length).Append(ID);
        }*/
    }
}