// TestRunModel.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include "MiniFASNet.h"
#include "AntiSpoofPredict.h"
using namespace AntiSpoofing;
int main()
{
    std::cout << "Hello World!\n";
    AntiSpoofPredict antiSpoofPredict("D:\\DzungApr6th_git\\DeepFace\\PyFaceDetect\\resources\\anti_spoof_models\\2.7_80x80_MiniFASNetV2.pth", "gpu");
    std::cout << "Load Model Success";
}


