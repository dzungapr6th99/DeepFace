#include "MiniFASNet.h";
#include<torch/all.h>
#include<opencv2/opencv.hpp>
#include<string>
#include<vector>
#pragma once
namespace MLCpp
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

	
	static std::map<std::string, std::function<MiniFASNet()>> Model_Dictionary = 
	{ {"MiniFASNETV1", []() { return MiniFASNetV1(); } }, {"MiniFASNetV2" ,[]() {return MiniFASNetV2(); } },
		{"MiniFASNetV1SE", []() {return MiniFASNetV1SE(); }},	{"MiniFASNetV2SE", []() {return MiniFASNetV2SE(); }} };


	class AntiSpoofPredict
	{
	private:
		std::string Device;
		std::string ModelPath;

		torch::nn::Module Model;
	public:
		AntiSpoofPredict();
		AntiSpoofPredict(std::string model_name,std::string model_path, std::string device);
		~AntiSpoofPredict();
		void LoadModel(std::string ModelPath);
		void predict(char img[]);

	};
}


