#include "OpenCVModel.h"
#include "OpenCVModel.h"
#include<algorithm>
#include "Common.h"
namespace OpenCVClr
{
	static bool CompareEye(cv::Rect e1, cv::Rect e2)
	{
		{
			int x1 = (e1.x - e1.width) * (e1.y * e1.height);
			int x2 = (e2.x - e2.width) * (e2.y * e2.height);
			return true;
		}
	}

	OpenCVModel::OpenCVModel(char* path, char* path_eye)
	{
		{
			Detector = new cv::CascadeClassifier();
			Eye_Detector = new cv::CascadeClassifier();

			if (Detector->load(path))
				cout << "Load Detect Face FrontScale Complete" << endl;
			else
				cout << "Load Detect Face FrontScale Fail";
			if (Eye_Detector->load(path_eye))
				cout << "Load Detect Eye Complete" << endl;
			else
				cout << "Load Detect Eye Fail" << endl;
			cout << "Init Detector at locate: " << Detector << endl;
			cout << "Init Eye Detector locate: " << Eye_Detector << endl;

		}
	}

	void OpenCVModel::Detect(char* Base64Array, int Length, bool Align, std::vector<int> EyeCoordinate, std::vector<cv::Mat> &ListFaces)
	{
		vector<cv::Mat> Detect_Faces;
		vector<cv::Rect> Eye_Region;
		cv::Mat img;
		std::string base64DecodeImg = base64_decode(Base64Array);
		vector<uchar> base64data(base64DecodeImg.begin(), base64DecodeImg.end());
		img = cv::imdecode(base64data, IMREAD_COLOR);

		vector<cv::Rect> objects;
		cv::Size MaxSize();
		cv::Size MinSize();
		Detector->detectMultiScale(img, objects, 1.1, 10); //Detect Face

		for (int i = 0; i < objects.size(); i++)
		{
			cv::Mat Eye;
			vector<cv::Rect> detectFaceRect;
			cv::Mat Face = img(cv::Range(objects[i].y, objects[i].y + objects[i].height), cv::Range(objects[i].x, objects[i].x + objects[i].width)); // Crop Image
			ListFaces.push_back(Face);
			std::cout << "Detected Image in coordinates with x: " << objects[i].x << " y: " << objects[i].y << " w: " << objects[i].width << " h: " << objects[i].height;
			if (Align)
			{
				vector<cv::Rect> Eye_Align = Align_Face(Face);
				Eye_Region.insert(Eye_Region.end(), Eye_Align.begin(), Eye_Align.end());
				if (Eye_Align.size() > 0)
				{
					EyeCoordinate.push_back((Eye_Align[0].x + Eye_Align[0].width) / 2);
					EyeCoordinate.push_back((Eye_Align[0].y + Eye_Align[0].height) / 2);
					EyeCoordinate.push_back((Eye_Align[1].x + Eye_Align[1].width) / 2);
					EyeCoordinate.push_back((Eye_Align[1].y + Eye_Align[1].height) / 2);
				}
				//Left_Eye.GetCoordinates((Eye_Align[0].x + Eye_Align[0].width) / 2, (Eye_Align[0].y + Eye_Align[0].height) / 2);
				//Right_Eye.GetCoordinates((Eye_Align[1].x + Eye_Align[1].width) / 2, (Eye_Align[1].y + Eye_Align[1].height) / 2);
			}

		}

	}

	vector<cv::Rect> OpenCVModel::Align_Face(cv::Mat Face)
	{
		cv::Mat Face_Gray;
		cvtColor(Face, Face_Gray, cv::COLOR_BGR2GRAY, 0);

		vector<cv::Rect> Eyes;
		cv::Rect Left_Eye;
		cv::Rect Right_Eye;

		Eye_Detector->detectMultiScale(Face_Gray, Eyes, 1.1, 10, 0);
		std::sort(Eyes.begin(), Eyes.end(), &CompareEye);
		if (Eyes.size() >= 2)
		{
			if (Eyes[0].x < Eyes[1].x)
			{
				Left_Eye = Eyes[0];
				Right_Eye = Eyes[1];
			}
			else
			{
				Left_Eye = Eyes[1];
				Right_Eye = Eyes[0];
			}

		}
		vector<cv::Rect> Detect_Eyes;
		Detect_Eyes.push_back(Left_Eye);
		Detect_Eyes.push_back(Right_Eye);
		return Detect_Eyes;

	}
	static inline bool is_base64(unsigned char c)
	{
		return (isalnum(c) || (c == '+') || (c == '/'));
	}
	static const std::string base64_chars =
		"ABCDEFGHIJKLMNOPQRSTUVWXYZ"
		"abcdefghijklmnopqrstuvwxyz"
		"0123456789+/";
	std::string OpenCVModel::base64_decode(char* base64StringPointer)
	{
		std::string const& encoded_string = base64StringPointer;
		int in_len = encoded_string.size();
		int i = 0;
		int j = 0;
		int in_ = 0;
		unsigned char char_array_4[4], char_array_3[3];
		std::string ret;

		while (in_len-- && (encoded_string[in_] != '=') && is_base64(encoded_string[in_]))
		{
			char_array_4[i++] = encoded_string[in_]; in_++;

			if (i == 4)
			{
				for (i = 0; i < 4; i++)
				{
					char_array_4[i] = base64_chars.find(char_array_4[i]);
				}

				char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
				char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
				char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

				for (i = 0; (i < 3); i++)
				{
					ret += char_array_3[i];
				}

				i = 0;
			}
		}

		if (i)
		{
			for (j = i; j < 4; j++)
			{
				char_array_4[j] = 0;
			}

			for (j = 0; j < 4; j++)
			{
				char_array_4[j] = base64_chars.find(char_array_4[j]);
			}

			char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
			char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
			char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

			for (j = 0; (j < i - 1); j++)
			{
				ret += char_array_3[j];
			}
		}

		return ret;
	}
}
void* CreateModel(char* path, char* path_Eyes)
{
	return new OpenCVClr::OpenCVModel(path, path_Eyes);
}

int DetectImage(void* model, char* base64Image, int length, int width, int height, void*& ListImage)
{
	OpenCVClr::OpenCVModel* model1 = (OpenCVClr::OpenCVModel*)model;
	std::vector<int> EyesCoordinate;
	std::vector<cv::Mat>Faces = { };
	model1->Detect(base64Image, length, false, EyesCoordinate, Faces);
	/*if (Faces.size() == 0)
		return 0;*/
	ListImage = new unsigned char[Faces.size() * 244 * 244 * 3];
	int index = 0;
	for (int i = 0; i < Faces.size(); i++)
	{
		cv::Mat ResizeFace;
		cv::resize(Faces[i], ResizeFace, cv::Size(width, height));

		_memccpy(ListImage, Faces[i].data, i, width * height * 3);
		i += width * height * 3;

	}
	return Faces.size();
}
