#include <torch/torch.h>
#include<torch/nn/modules.h>
#include<map>
#include<string>
#include "pch.h"
#pragma once
namespace AntiSpoofing
{

	static std::map<std::string, std::vector<int>> keep_dict = { {"1.8M", {32, 32, 103, 103, 64, 13, 13, 64, 26, 26,
																	64, 13, 13, 64, 52, 52, 64, 231, 231, 128,
																	154, 154, 128, 52, 52, 128, 26, 26, 128, 52,
																	52, 128, 26, 26, 128, 26, 26, 128, 308, 308,
																	128, 26, 26, 128, 26, 26, 128, 512, 512} },
														  {"1.8M_", {32, 32, 103, 103, 64, 13, 13, 64, 13, 13, 64, 13,
																	13, 64, 13, 13, 64, 231, 231, 128, 231, 231, 128, 52,
																	52, 128, 26, 26, 128, 77, 77, 128, 26, 26, 128, 26, 26,
																	128, 308, 308, 128, 26, 26, 128, 26, 26, 128, 512, 512} } };
	class L2Norm :public torch::nn::Module
	{
	public:

		~L2Norm() {}
		torch::Tensor forward(torch::Tensor Input)
		{
			return torch::nn::functional::normalize(Input);
		}
	};

	class Flatten : public torch::nn::Module
	{
	private:
		torch::nn::Flatten flatten{ nullptr };
	public:
		Flatten();
		torch::Tensor forward(torch::Tensor input)
		{
			return torch::nn::Flatten()(input);
		}
		~Flatten() { }
	};

	class Conv_Block :public torch::nn::Module
	{
	public:
		torch::nn::Conv2d conv{ nullptr };
		torch::nn::BatchNorm2d bn{ nullptr };
		torch::nn::PReLU prelu{ nullptr };
	public:
		Conv_Block();
		Conv_Block(int in_c, int out_c, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int groups);
		~Conv_Block();
		torch::Tensor forward(torch::Tensor input);

	};

	class Linear_Block : public torch::nn::Module
	{
	public:
		torch::nn::Conv2d conv{ nullptr };
		torch::nn::BatchNorm2d bn{ nullptr };
	public:
		Linear_Block();
		Linear_Block(int in_c, int out_c, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int groups);
		~Linear_Block();
		torch::Tensor forward(torch::Tensor input);
	};

	class Depth_Wise :public torch::nn::Module
	{
	public:
		Conv_Block* conv;
		Conv_Block* conv_dw;
		Linear_Block* project;
		bool residual;
	public:
		Depth_Wise(std::vector<int> c1, std::vector<int> c2, std::vector<int> c3, bool Residual, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int groups);
		~Depth_Wise();
		torch::Tensor forward(torch::Tensor input);
	};

	class Residual : torch::nn::Module
	{
	public:
		torch::nn::Sequential models{ nullptr };
	public:
		Residual(std::vector<std::vector<int>> c1, std::vector<std::vector<int>> c2, std::vector<std::vector<int>> c3, int numblock, int groups, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding);
		~Residual();
	public:
		torch::Tensor forward(torch::Tensor input);
	};

	class SEModule : public torch::nn::Module
	{
	private:
		torch::nn::AdaptiveAvgPool2d avg_pool{ nullptr };
		torch::nn::Conv2d fc1{ nullptr };
		torch::nn::BatchNorm2d bn1{ nullptr };
		torch::nn::ReLU relu{ nullptr };
		torch::nn::Conv2d fc2{ nullptr };
		torch::nn::BatchNorm2d bn2{ nullptr };
		torch::nn::Sigmoid sigmoid{ nullptr };
	public:
		SEModule(int chanels, int reduction);
		~SEModule();
		torch::Tensor forward(torch::Tensor input);
	};

	class Depth_Wise_SE : public torch::nn::Module
	{
	public:
		Conv_Block* conv;
		Conv_Block* conv_dw;
		Linear_Block* project;
		bool residual;
		SEModule* se_module;
	public:
		Depth_Wise_SE(std::vector<int> c1, std::vector<int> c2, std::vector<int> c3, bool _residual, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int groups, int se_reduct);
		~Depth_Wise_SE();
		torch::Tensor forward(torch::Tensor input);
	};


	class ResidualSE : public Residual
	{
	private:
		std::vector<torch::nn::Module> Modules;
	public:
		ResidualSE(std::vector<std::vector<int>> c1, std::vector<std::vector<int>> c2, std::vector<std::vector<int>> c3, int num_block, int groups, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int se_reduct);
		~ResidualSE();
	};

	class MiniFASNet : public torch::nn::Module
	{
	public:
		int Embeding_Size;
		Conv_Block* conv_1;
		Conv_Block* conv_2_dw;
		Depth_Wise* conv_23;
		Residual* conv_3;
		Depth_Wise* conv_34;
		Residual* conv_4;
		Depth_Wise* conv_45;
		Residual* conv_5;
		Conv_Block* conv_6_sep;
		Linear_Block* conv_6_dw;
		torch::nn::Flatten conv_6_flatten{ nullptr };
		torch::nn::Linear linear{ nullptr };
		torch::nn::BatchNorm1d bn{ nullptr };
		torch::nn::Dropout drop{ nullptr };
		torch::nn::Linear prob{ nullptr };
	public:
		~MiniFASNet();
		MiniFASNet(std::vector<int> keep, int embedding_size, std::vector<int64_t> conv6_kernel, float drop_p, int num_classess, int img_chanel);
		torch::Tensor forward(torch::Tensor input);
	};

	class MiniFASNetSE : public MiniFASNet
	{
	public:
		~MiniFASNetSE();
		MiniFASNetSE(std::vector<int> keep, int embedding_size, std::vector<int64_t> conv6_kernel, float drop_p, int num_classess, int img_chane);

	};

	/// <summary>
	/// (80x80) flops: 0.044, params: 0.41
	/// </summary>
	static MiniFASNet* MiniFASNetV1(int embedding_size = 128, std::vector<int64_t> conv6_kernel = { 7,7 }, float drop_p = 0.2, int num_classes = 3, int img_chanel = 3)
	{
		return new MiniFASNet(keep_dict["1.8M"], embedding_size, conv6_kernel, drop_p, num_classes, img_chanel);
	}


	/// <summary>
	/// (80x80) flops: 0.044, params: 0.43
	/// </summary>
	static MiniFASNet* MiniFASNetV2(int embedding_size = 128, std::vector<int64_t> conv6_kernel = { 7,7 }, float drop_p = 0.2, int num_classes = 3, int img_chanel = 2)
	{
		return new MiniFASNet(keep_dict["1.8M_"], embedding_size, conv6_kernel, drop_p, num_classes, img_chanel);
	}


	static MiniFASNetSE* MiniFASNetV1SE(int embedding_size = 128, std::vector<int64_t> conv6_kernel = { 7,7 }, float drop_p = 0.2, int num_classes = 3, int img_chanel = 2)
	{
		return new MiniFASNetSE(keep_dict["1.8M"], embedding_size, conv6_kernel, drop_p, num_classes, img_chanel);

	}
	static MiniFASNetSE* MiniFASNetV2SE(int embedding_size = 128, std::vector<int64_t> conv6_kernel = { 7,7 }, float drop_p = 0.2, int num_classes = 3, int img_chanel = 2)
	{
		return new MiniFASNetSE(keep_dict["1.8M_"], embedding_size, conv6_kernel, drop_p, num_classes, img_chanel);
	};
}

