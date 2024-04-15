#pragma once
#include<string>
#include<opencv2/opencv.hpp>
#include"pch.h"
using namespace std;
<<<<<<< HEAD
#define CPPEXPORT __declspec(dllexport) 
=======
>>>>>>> d52732b9f3bc244a910e59c5744ae2efbae3bac8
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
<<<<<<< HEAD
		void Detect(char* Base64Array, int Length, bool Align, std::vector<int> EyeCoordinate, std::vector<cv::Mat> &ListFaces);
		cv::Rect* a;
		vector<cv::Rect> Align_Face(cv::Mat Face);
		std::string base64_decode(char* base64StringPointer);

	};
	
}


	void* CreateModel(char* path, char* path_eyes);
	int DetectImage(void* model, char* base64Image, int length, int width, int height, void*& Data);


=======

		void Detect(char* Base64Array, int Length, bool Align, std::vector<int> EyeCoordinate, std::vector<cv::Mat> ListFaces);
		cv::Rect* a;
		vector<cv::Rect> Align_Face(cv::Mat Face);


	};
<<<<<<<< HEAD:Cpp/OpenCvDll/OpenCVModel.h
	EXTERN_C
	{
		void* CreateModel(char* path, char* path_eyes);
		void DetectImage(void* model, char* base64Image, int length, unsigned char** ListFaces);
========
	void* CreateModel(char* path, char* path_eyes);
	void DetectImage(void* model, char* base64Image, int length, unsigned char** ListFaces);
>>>>>>>> d52732b9f3bc244a910e59c5744ae2efbae3bac8:Cpp/Detector.Test/OpenCVModel.h

	};
}

>>>>>>> d52732b9f3bc244a910e59c5744ae2efbae3bac8
