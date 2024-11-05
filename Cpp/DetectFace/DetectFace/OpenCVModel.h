#pragma once
#include<string>
#include<opencv2/opencv.hpp>
using namespace std;

#ifndef CPPEXPORT
# if (defined _WIN32 || defined WINCE || defined __CYGWIN__) && defined(CVAPI_EXPORTS)
#   define CPPEXPORT __declspec(dllexport)
# elif defined __GNUC__ && __GNUC__ >= 4 && (defined(CVAPI_EXPORTS) || defined(__APPLE__))
#   define CPPEXPORT __attribute__ ((visibility ("default")))
# endif
#endif
#ifndef CPPEXPORT
# define CPPEXPORT
#endif

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
		void Detect(char* Base64Array, int Length, bool Align, std::vector<int> EyeCoordinate, std::vector<cv::Mat> &ListFaces);
		cv::Rect* a;
		vector<cv::Rect> Align_Face(cv::Mat Face);
		std::string base64_decode(char* base64StringPointer);

	};
	
}
extern "C"
{
	CPPEXPORT void* CreateModel(char* path, char* path_eyes);
	CPPEXPORT int DetectImage(void* model, char* base64Image, int length, int width, int height, void*& Data);

};

