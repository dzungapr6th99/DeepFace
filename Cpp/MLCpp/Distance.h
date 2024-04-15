#pragma once
#include<torch/csrc/api/include/torch/torch.h>
#include<torch/csrc/api/include/torch/all.h>
#include<torch/csrc/api/include/torch/autograd.h>
#include<torch/csrc/autograd/variable.h>
#include <torch/csrc/autograd/function.h>
#include<ATen/ATen.h>
namespace MLCpp
{
	ref class Distance
	{
	public:
		float findEuclideanDistance(float source_Presentation[], float test_Presentation[]);
		float findCosineDistance(float source_Presentation[], float test_Presentation[]);
		
	};

}

