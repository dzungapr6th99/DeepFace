using Adaptive.Aeron;
using Disruptor;
using H264Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTransmission
{
    /// <summary>
    /// H264 decode and process like stream
    /// </summary>
    public class H264Stream : IEventHandler<DataContainer> ,IDisposable
    {
        private readonly Subscription _subcription;
        public readonly string StreamId;
        private readonly H264Decoder _h264Decoder;
        public H264Stream(string streamId)
        {
            _h264Decoder = new H264Decoder();
            
        }

        public void OnEvent(DataContainer data, long seqeuence, bool endOfBatch)
        {
            
        }

        public void Dispose()
        {

        }
        
    }

    public class DataContainer
    {
        public byte[] Frame;
        public long Sequence;
    }
}
