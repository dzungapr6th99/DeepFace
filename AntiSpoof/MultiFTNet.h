#pragma once
#include <torch/torch.h>
#include<map>
#include<string>
#include"MiniFASNet.h"
#include "pch.h"
namespace AntiSpoofing
{
	class FTGenerator : public torch::nn::Module
	{
	public:
		torch::nn::Sequential ft;

	public:
		~FTGenerator();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_in_chanels">48</param>
		/// <param name="out_chanels">1</param>
		FTGenerator(int _in_chanels, int out_chanels);

	public:
		torch::Tensor forward(torch::Tensor input);
	};

	class MultiFTNet : public torch::nn::Module
	{
	public:
		int img_chanel;
		int num_classes;
		MiniFASNet* Model;
		FTGenerator* ftGenerator;
	public:
		/// <summary>
		/// 
		/// </summary>
		/// <param name="img_chanels">3</param>
		/// <param name="num_classes">3</param>
		/// <param name="embedding_size">128</param>
		/// <param name="conv6_kernel">(5,5)</param>
		MultiFTNet(int img_chanels, int num_classes, int embedding_size, std::vector<int64_t> conv6_kernel);
		~MultiFTNet();		
	public:
		void initialize_weights();
		torch::Tensor forward(torch::Tensor input);
	};
}
