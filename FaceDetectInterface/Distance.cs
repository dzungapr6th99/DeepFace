using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectInterface
{
    public class Distance
    {
       
        public static double FindCosineDistance(float[] sourceRepresentation, float[] testRepresentation)
        {
            Decimal a = 0;
            Decimal b = 0;
            Decimal c = 0;
            /*for (int i = 0; i< sourceRepresentation.Length; i++)
            {
                if (sourceRepresentation[i] + testRepresentation[i] > 0)
                {
                    Console.WriteLine("{0}: source: {1} test: {2}",i, sourceRepresentation[i], testRepresentation[i]);
                }    
            }   */ 
            for (int i = 0; i < sourceRepresentation.Length; i++)
            {
                a += (Decimal)sourceRepresentation[i] * (Decimal)testRepresentation[i];
                b += (Decimal)sourceRepresentation[i] * (Decimal)sourceRepresentation[i];
                c += (Decimal)testRepresentation[i] * (Decimal)testRepresentation[i];
            }

            Decimal distance = 1 - a / (Decimal)(Math.Sqrt((double)b) * Math.Sqrt((double)c));
            return (double)distance;
        }

        public static double FindEuclideanDistance(float[] sourceRepresentation, float[] testRepresentation)
        {
            double euclideanDistance = 0;

            for (int i = 0; i < sourceRepresentation.Length; i++)
            {
                double diff = sourceRepresentation[i] - testRepresentation[i];
                euclideanDistance += diff * diff;
            }

            euclideanDistance = Math.Sqrt(euclideanDistance);
            return euclideanDistance;
        }
    }
}
