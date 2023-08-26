
using CommonLib;
using Tensorflow;
using Emgu.CV;
using System.Numerics;
using Tensorflow.Keras.Engine;

namespace PreProcess
{
    public class PreProcessModel
    {
        CascadeClassifier Detector { get; set; }   
        Model _Model { get; set; }
        public PreProcessModel(string Path) 
        {
            Detector = new CascadeClassifier(Path);
            if (Detector.)
        }

        public void Pred()
        {
            _Model.predict();
        }

        
    }
}