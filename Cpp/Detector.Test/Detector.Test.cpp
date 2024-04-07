/*
Because unit test project is not compatible with Dll project(Dll project use windows.h)
So I coppy library and function in Project need to unit test to here.
There are serveral method to unit test for DLLs project. but I need to check the coverage of code, so I do like this
*/
#include "pch.h"
#include "CppUnitTest.h"
#include "OpenCVModel.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace DetectorTest
{
	TEST_CLASS(DetectorTest)
	{
	public:



		TEST_METHOD(TestDetectImage)
		{
			OpenCVClr::OpenCVModel* model = (OpenCVClr::OpenCVModel*)OpenCVClr::CreateModel("", "");
			unsigned char** Images = new unsigned char *[4];
			OpenCVClr::DetectImage(model, "", 0, Images);

		}
	};
}
