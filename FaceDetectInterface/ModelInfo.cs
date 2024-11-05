using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectInterface
{
    internal class ModelInfo
    {
        public string ModelName { get; set; }
        public InputSize Size { get; set; }
        public OutPutDim OutPutDim { get; set; }
        public Threshold Threshold { get; set; }

    }

    internal class InputSize
    {
        public int Witdth { get; set; }
        public int Height { get; set; }
    }

    internal class OutPutDim
    {
        public int Dims { get; set; }
    }

    internal class Threshold
    {
        public double Cosine { get; set; }
        public double Euclidean { get; set; }
        public double Euclidean_l2 { get; set; }
    }
}
