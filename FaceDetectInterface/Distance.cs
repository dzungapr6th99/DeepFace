using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectInterface
{
    internal class Distance
    {
        public static void Main(string[] args)
        {
            // Example usage:
            float[] sourceRepresentation = { 1.0f, 2.0f, 3.0f }; // Example source representation
            float[] testRepresentation = { 4.0f, 5.0f, 6.0f }; // Example test representation

            double cosineDistance = FindCosineDistance(sourceRepresentation, testRepresentation);
            Console.WriteLine("Cosine Distance: " + cosineDistance);

            double euclideanDistance = FindEuclideanDistance(sourceRepresentation, testRepresentation);
            Console.WriteLine("Euclidean Distance: " + euclideanDistance);
        }

        public static double FindCosineDistance(float[] sourceRepresentation, float[] testRepresentation)
        {
            float a = 0;
            float b = 0;
            float c = 0;

            for (int i = 0; i < sourceRepresentation.Length; i++)
            {
                a += sourceRepresentation[i] * testRepresentation[i];
                b += sourceRepresentation[i] * sourceRepresentation[i];
                c += testRepresentation[i] * testRepresentation[i];
            }

            double distance = 1 - (a / (Math.Sqrt(b) * Math.Sqrt(c)));
            return distance;
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
