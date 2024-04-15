using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using CommonLib;
namespace TcpInterface
{
    public class PeerConnected
    {

        IPAddress c_RemoteIP;
        int c_RemotePort;
        Socket c_SocketConnected;
        BinaryReader c_BinaryReader;
        private readonly NetworkStream c_Stream;
        bool c_IsActived = false;
        int _ReceiveTimeout = 3600;//thời gian tính bằng giây
        private bool GetPackageFlag = false;
        int _SocketReadZeroTimeOut = 30;//thời gian đọc toàn dữ liệu zero, chứng tỏ socket đã đóng
        #region truy cap thuoc tinh
        /// <summary>
        /// Header mỗi Message DetectFace H - Header
        /// </summary>
        private const string HeaderMessage = "H";
        /// <summary>
        /// Header mỗi Package sau khi chia nhỏ message - Seq
        /// </summary>
        private const string HeaderPackage = "S";
        /// <summary>
        /// Tail của mỗi Package C - Contine - Còn nữa
        /// </summary>
        private const string TailerPackage_Cont = "C";
        /// <summary>
        /// Tail báo hiệu đã nhận xong message - Complete nhưng trùng C với Continew - thôi kệ để là c
        /// </summary>
        private const string TailerPackage_Complete = "c";
        public bool IsActived
        {
            get
            {
                return c_IsActived;
            }
        }

        public IPAddress RemoteIP
        {
            get
            {                
                return c_RemoteIP;
            }
        }

        public int RemotePort
        {
            get
            {
                return c_RemotePort;
            }


        }


        #endregion
        public PeerConnected(Socket p_SocketConnected)
        {
            try
            {
                c_SocketConnected = p_SocketConnected;
                //
                c_SocketConnected.NoDelay = true;
                c_SocketConnected.ReceiveTimeout = _ReceiveTimeout * 1000; //Thời gian timeout nhận message không set thờigian 

                c_SocketConnected.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TypeOfService, 35);
                c_SocketConnected.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.BlockSource, 1);
                c_SocketConnected.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.DropSourceMembership, 5);    
                c_SocketConnected.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                IPEndPoint ipend = (IPEndPoint)c_SocketConnected.RemoteEndPoint;
                c_RemoteIP = ipend.Address;
                c_RemotePort = ipend.Port;
                c_IsActived = true;
                c_Stream = new NetworkStream(c_SocketConnected);
                c_BinaryReader = new BinaryReader(c_Stream);
                TimeStartBytesReadzero = DateTime.Now.Ticks;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        #region đọc/ghi dữ liệu 
        //dữ liệu gửi đến là 1 stream, nên khi đọc 1 DefaultBufferSize: có thể chứa trọn vẹn 1 message, nhiều message, hoặc 1 phần của message
        private StringBuilder c_incomingMessage = new StringBuilder(); //lưu dữ liệu nhận đc.
        private static int DefaultBufferSize = 256; //theo đọc tài liệu của HoSE thì: According to the TCP/IP standard, the minimal MTU size is 576 bytes. Since IP header is 20 bytes long and TCP header is 20 bytes long (with no option), the safest maximum packet size of AUTO-tP is 536 bytes long including its header
        private byte[] _ReadingBuffer = new byte[DefaultBufferSize];
        private bool StartBytesReadzero = false;
        private long TimeStartBytesReadzero = DateTime.Now.Ticks;
        public byte[] GetPackage()
        {
            // Nguyên tắc là HeaderPackage là H<8 byte Độ dài toàn bộ Package> SOH
            // Mỗi Package 256 byte, bắt đầu = S<8 byteSequence>=<2 byte Độ dài Package>- Byte cuối c hoặc C.
            byte[] Package;
            List<byte[]> ListPackage = new List<byte[]>();
            int Sequence = 0;
            try
            {
                do
                {
                    if (!GetPackageFlag)
                    {
                        //Nếu chưa có tín hiệu nhận Package
                        _ReadingBuffer = c_BinaryReader.ReadBytes(DefaultBufferSize);
                        if ((char)_ReadingBuffer[0] == 'H')
                        {
                            GetPackageFlag = true;
                            Package = new byte[BitConverter.ToInt32(_ReadingBuffer, 1)];
                        
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {

                    }
                }
                while (true);
            
            }
            catch(Exception ex) 
            {
                return null;
                LOG.log.Error(ex);
            }
            return null;

           
        }        
        public bool SendMessage(string p_str)
        {
            try
            {
                if (!(p_str == null || p_str == "") && c_SocketConnected != null)
                {
                    byte[] bufContent = Encoding.ASCII.GetBytes(p_str);
                    //send message
                    c_SocketConnected.Send(bufContent, 0, bufContent.Length, SocketFlags.None, out SocketError socketError);

                    if (socketError == SocketError.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LOG.log.Error(ex.ToString());
                StopConnected();
            }
            return false;
        }

     
        #endregion

        public void StopConnected()
        {
            try
            {
               

                if (c_SocketConnected != null)
                {
                    
                    try
                    {
                        c_SocketConnected.Shutdown(SocketShutdown.Both);
                        c_IsActived = false;
                    }
                    catch (Exception)
                    {
                    }
                    c_SocketConnected.Close();

                }
            }
            catch (System.Exception ex)
            {
                LOG.log.Error("TEST xem ghi log gi:" + ex.ToString());

            }
        } 
    }
    
}
