#include "AntiSpoofPredict.h"
#include <math.h>
#include <string>
#include<iostream>
#include<fstream>
#include "Utils.h"
#include<map>
#include"Common.h"
#include"pch.h"
using namespace std;

namespace AntiSpoofing
{
	std::vector<char> get_the_bytes(std::string filename) {
		std::ifstream input(filename, std::ios::binary);
		std::vector<char> bytes(
			(std::istreambuf_iterator<char>(input)),
			(std::istreambuf_iterator<char>()));

		input.close();
		return bytes;
	}

	Detection::Detection()
	{

	}

	Detection::~Detection()
	{

	}

	Detection::Detection(std::string model, std::string dir)
	{
		detector_confidence = 0.6;
		const cv::String Deploy;
		const cv::String Model;
		*detector = cv::dnn::readNetFromCaffe(Deploy, Model);

	}

	std::vector<int> Detection::getbbox(cv::Mat Img)
	{
		int height = Img.size().height;
		int width = Img.size().width;
		double aspect_ratio = height / width;
		if (height * width > 192 * 192)
		{
			cv::resize(Img, Img, cv::Size(192 * sqrt(aspect_ratio), 192 / sqrt(aspect_ratio)), cv::INTER_LINEAR);
		}
		cv::Mat blob = cv::dnn::blobFromImage(Img, 1, cv::Size(), cv::Scalar(104, 117, 123));
		detector->setInput(blob, "data");
		cv::Mat out = detector->forward("detection_out");

	}

	AntiSpoofPredict::AntiSpoofPredict()
	{


	}

	AntiSpoofPredict::~AntiSpoofPredict()
	{

	}

	AntiSpoofPredict::AntiSpoofPredict(string Model_Name, std::string device_option)
	{
		this->Device = device_option;
		LoadModel(Model_Name, device_option);
		Model->eval();
	}

	void AntiSpoofPredict::LoadModel(std::string model_path, string device_option)
	{
		std::string model_base_name;
#ifdef _UNIX
		model_base_name = model_path.substr(model_path.find_last_of("/"));

#else
		model_base_name = model_path.substr(model_path.find_last_of("/\\"));
#endif

		vector<string> ModelInfo = Utils::parse_model_name(model_base_name);
		string Model_Name = ModelInfo[1];
		vector<int> width_height = Utils::get_width_height(ModelInfo[0]);
		vector<int64_t> getkernel = Utils::get_kernel(width_height[0], width_height[1]);
		if (Model_Name == "MiniFASNetV1")
		{
			Model = MiniFASNetV1(128, getkernel);
		}
		else if (Model_Name == "MiniFASNetV2")
		{
			Model = MiniFASNetV2(128, getkernel);
		}
		else if (Model_Name == "MiniFASNetV1SE")
		{
			Model = MiniFASNetV1SE(128, getkernel);
		}
		else if (Model_Name == "MiniFASNetV2SE")
		{
			Model = MiniFASNetV2SE(128, getkernel);
		}

		if (Device == "gpu")
		{
			if (torch::cuda::is_available())
			{
				Model->to(torch::kCUDA);
				this->device = new torch::Device(torch::kCUDA);

			}
			this->device = new torch::Device(torch::kCPU);
		}

		//Load Model weight
		std::vector<char> getbytes = get_the_bytes(Model_Name);
		c10::Dict<c10::IValue, c10::IValue> weights = torch::pickle_load(getbytes).toGenericDict();

		const torch::OrderedDict<std::string, torch::Tensor>& model_params = this->Model->named_parameters();
		std::vector<std::string> params_name;
		for (auto const& w : model_params)
		{
			params_name.push_back(w.key());
		}
		torch::NoGradGuard no_grad;

		for (auto const& w : weights)
		{
			std::string name1 = w.key().toStringRef();
			std::string name = name1.substr(7, name1.size() - 1);
			torch::Tensor param = w.value().toTensor();

			if (std::find(params_name.begin(), params_name.end(), name) != params_name.end())
			{
				//model_params.find
				model_params.find(name)->copy_(param);
			}
			else
			{

			}

		}

	}

	torch::Tensor AntiSpoofPredict::predict(cv::Mat img)
	{
		torch::Tensor input_img = ToTensor(img);
		torch::NoGradGuard();
		torch::Tensor result = Model->forward(input_img);
		result = torch::nn::functional::softmax(result, torch::nn::functional::SoftmaxFuncOptions(1)).cpu().numpy_T();
		return result;
	}


}