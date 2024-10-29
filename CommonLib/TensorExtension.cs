using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public static class TensorExtension
    {
        public static ReadOnlyMemory<float>ToReadOnlyMemory(this Tensor<float> data)
        {
            return new ReadOnlyMemory<float>( data.ToArray<float>());
        }
    }
}
