﻿using System.Net.WebSockets;
using System.IO;
using System.Drawing;
using CommonLib;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using System.Numerics.Tensors;
using PreProcess;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Runtime.InteropServices;
using System;
namespace FaceDetectInterface
{
    public interface IFaceDetect
    {
        public bool Verify(string ImgBase64Db, string ImgBase64Input);
        public void LoadModel();
    }

    public class FaceDetect : IFaceDetect
    {
        private IDetectorModel c_DetectorModel;
        private int width;
        private int height;
        private InferenceSession c_InferenceSession;
        private string PathModel;
        private bool IsLoadModel = false;
        public FaceDetect(IDetectorModel p_DetectorModel)
        {
            width = 224;
            height = 224;
            c_DetectorModel = p_DetectorModel;
            PathModel = ConfigData.ModelVerifyPath;
            c_DetectorModel.LoadModel();
            IsLoadModel = false;

        }
        public FaceDetect()
        {
            IsLoadModel = false;
        }
        public void LoadModel()
        {
            width = 224;
            height = 224;
            if (!IsLoadModel)
            {
                if (ConfigData.IsRunOnGpu)
                {
                    LOG.log.Info("Start init Model run on Gpu");
                    SessionOptions gpuSessionOption = SessionOptions.MakeSessionOptionWithCudaProvider(0);
                    c_InferenceSession = new InferenceSession(PathModel, gpuSessionOption);
                    LOG.log.Info("Init Model run on Gpu Success");
                }
                else
                {
                    LOG.log.Info("Start init Model run on Cpu");
                    c_InferenceSession = new InferenceSession(PathModel);
                    LOG.log.Info("Init Model run on Cpu Success");
                }
            }
            IsLoadModel = true;
        }

        public bool Verify(string ImgBase64Db, string ImgBase64Input)
        {
            (int numFaceDb, byte[] DataDb) = c_DetectorModel.Detect(ImgBase64Db, width, height);
            LOG.log.Info("ImgBaseDb detected {0} faces", numFaceDb);
            (int numFaceInput, byte[] DataInput) = c_DetectorModel.Detect(ImgBase64Input, width, height);
            LOG.log.Info("ImgBaseInpit detected {0} faces", numFaceInput);
            List<byte[]> FacesData = new List<byte[]>();
            FacesData.Add(DataDb.AsSpan().Slice(0 , width * height * 3).ToArray());
            FacesData.Add(DataInput.AsSpan().Slice(0, width * height * 3).ToArray());
            for (int i = 0; i < numFaceDb; i++)
            {
                FacesData.Add(DataDb.AsSpan().Slice(i * (width * height * 3), width * height * 3).ToArray());
            }
            for (int i = 0; i < numFaceInput; i++)
            {
                FacesData.Add(DataInput.AsSpan().Slice(i * (width * height * 3), width * height * 3).ToArray());
            }
            Tensor<float> InputInference = ByteArraysToTensor(FacesData, width, height);

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(c_InferenceSession.InputNames[0], InputInference)
            };
            var ouputReference = c_InferenceSession.Run(inputs);
            //Console.WriteLine("Dimension {0} | {1}",ouputReference[0].AsTensor<float>().Dimensions[0], ouputReference[0].AsTensor<float>().Dimensions[1]);
            //float[] floatOutPut = ouputReference[0].AsTensor<float>().ToArray();
            List<float[]> outPutDir = TensorToListOfArrays(ouputReference[0].AsTensor<float>());
            double distance = Distance.FindCosineDistance(outPutDir[0], outPutDir[1]);
            LOG.log.Info("Distance: {0}", distance);
            return distance > 0.069;

        }

        private Tensor<float> ByteArray2Tensor(int numFace, byte[] data, int width, int height)
        {
            if (data.Length != width * height * 3)
                throw new ArgumentException("Invalid image data size");

            float[] tensorData = new float[3 * width * height];
            int tensorIndex = 0;
            for (int i = 0; i < data.Length; i += 3)
            {
                // Assuming RGB order
                tensorData[tensorIndex++] = data[i] / 255.0f; // R
                tensorData[tensorIndex++] = data[i + 1] / 255.0f; // G
                tensorData[tensorIndex++] = data[i + 2] / 255.0f; // B
            }

            return new DenseTensor<float>(tensorData, new[] { numFace, height, width, 3 });
        }

        private Tensor<float> ByteArraysToTensor(List<byte[]> imageDatas, int width, int height)
        {
            int numImages = imageDatas.Count;
            var tensorData = new float[numImages * 3 * width * height];
            int tensorIndex = 0;

            foreach (var imageData in imageDatas)
            {
                if (imageData.Length != width * height * 3)
                    throw new ArgumentException("Invalid image data size");

                for (int i = 0; i < imageData.Length; i += 3)
                {
                    // Assuming RGB order
                    tensorData[tensorIndex++] = imageData[i] / 255.0f; // R
                    tensorData[tensorIndex++] = imageData[i + 1] / 255.0f; // G
                    tensorData[tensorIndex++] = imageData[i + 2] / 255.0f; // B
                }
            }

            return new DenseTensor<float>(tensorData, new[] { numImages, height, width, 3 });
        }
        public List<float[]> TensorToListOfArrays(Tensor<float> tensor)
        {
            int numVectors = tensor.Dimensions[0];
            int vectorSize = tensor.Dimensions[1];

            // Get the array of float values from the tensor
            float[] tensorArray = tensor.ToArray();

            // Convert the tensor to a list of float arrays
            List<float[]> floatList = new List<float[]>();
            int index = 0;
            for (int i = 0; i < numVectors; i++)
            {
                float[] array = new float[vectorSize];
                for (int j = 0; j < vectorSize; j++)
                {
                    array[j] = tensorArray[index++];
                }
                floatList.Add(array);
            }

            return floatList;
        }
        public float[] TensorToFloatArray(Tensor<float> tensor)
        {
            // Get the array of float values from the tensor
            float[] tensorArray = tensor.ToArray();

            // Copy the values to a new float array
            float[] floatArray = new float[tensorArray.Length];
            Array.Copy(tensorArray, floatArray, tensorArray.Length);

            return floatArray;
        }
    }
}