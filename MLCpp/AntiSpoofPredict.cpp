#include "AntiSpoofPredict.h"
#include <math.h>
#include<map>
namespace MLCpp
{
	Detection::Detection()
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
		cv::Mat out =  detector->forward("detection_out");
		
	}

	void AntiSpoofPredict::LoadModel(std::string ModelPath)
	{
		Model = Model_Dictionary[ModelPath](128);
	}

	AntiSpoofPredict::AntiSpoofPredict()
	{

	}

	AntiSpoofPredict::~AntiSpoofPredict()
	{

	}

	AntiSpoofPredict::AntiSpoofPredict(std::string Model_Name ,std::string Model_Path, std::string device)
	{
		Device = device;
		

	}
}