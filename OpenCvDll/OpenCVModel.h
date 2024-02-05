#pragma once
#include<string>
#include<opencv2/opencv.hpp>
#include"pch.h"
using namespace std;
namespace OpenCVClr
{
	using namespace cv;

	class OpenCVModel
	{
	public:
		cv::CascadeClassifier* Detector;
		cv::CascadeClassifier* Eye_Detector;
		/// <summary>
		/// Load model, Path = path file xml
		/// </summary>
		/// <param name="path"></param>
		/// <param name="type"></param>
		OpenCVModel(char* Path, char* Path_Eyes);

		void Detect(char* Base64Array, int Length, bool Align, std::vector<int> EyeCoordinate);
		cv::Rect* a;
		vector<cv::Rect> Align_Face(cv::Mat Face);


	};

}
