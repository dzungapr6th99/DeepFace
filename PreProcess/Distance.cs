using Numpy;
namespace PreProcess
{
    public class Distance
    {
        public static NDarray DistanceEuclide(NDarray src, NDarray dst)
        {

            NDarray r1 = src - dst;
            r1 = np.multiply(r1, r1);
            NDarray result1 = np.sum(r1);
            return result1;
        }

    }
}
