#pragma once
#include "MiniFASNet.h";
#include<torch/all.h>
#include<torch/jit.h>
#include<opencv2/opencv.hpp>
#include<string>
#include<vector>

namespace AntiSpoofing
{
    class Detection
	{
	private:
		std::string* dirname;
		cv::dnn::Net* detector;
		float detector_confidence;
	public:
		Detection();
		~Detection();
		Detection(std::string model, std::string dir);
		std::vector<int> getbbox(cv::Mat Img);
	};

	

	class AntiSpoofPredict
	{
	private:
		std::string Device;
		std::string ModelPath;
		MiniFASNet* Model;
		torch::Device* device;
	public:
		AntiSpoofPredict();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="model_path">Đường dẫn đến file pth</param>
		/// <param name="device">cpu/gpu</param>
		AntiSpoofPredict(std::string model_path, std::string device);
		~AntiSpoofPredict();
		void LoadModel(std::string ModelPath, std::string device);
		torch::Tensor predict(cv::Mat img);

	};
}


