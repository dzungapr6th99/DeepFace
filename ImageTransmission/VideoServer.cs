using Adaptive.Aeron;

namespace ImageTransmission
{
    /// <summary>
    /// Class to get video data from subcriber
    /// </summary>
    public class VideoServer
    {
        private readonly Aeron _aeron;
        private const int WILDCARDID = -1;
        private readonly int _port;
        public VideoServer(int Port)
        {
            _port = Port;
            _aeron = Aeron.Connect();
        }

        public void StartServer()
        {
            string channel = @$"aeron:udp?endpoint=localhost:{_port}";
            Subscription subscription = _aeron.AddSubscription(channel, WILDCARDID);

        }
    }
}
