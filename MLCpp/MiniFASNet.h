#include <torch/torch.h>
#include<map>
#include<string>
#pragma once
namespace MLCpp
{
	class L2Norm :public torch::nn::Module
	{
	public:
		L2Norm();
		~L2Norm();
		torch::Tensor forward(torch::Tensor Input);
	};

	class Flatten : public torch::nn::Module
	{
	private:
		torch::nn::Flatten flatten;
	public:
		Flatten();
		torch::Tensor forward(torch::Tensor input);
		~Flatten();
	};

	class Conv_Block :public torch::nn::Module
	{
	public:
		torch::nn::Conv2d conv;
		torch::nn::BatchNorm2d bn;
		torch::nn::PReLU prelu;
	public:
		Conv_Block();
		Conv_Block(int in_c, int out_c, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int groups = 1);
		~Conv_Block();
		torch::Tensor forward(torch::Tensor input);

	};

	class Linear_Block : public torch::nn::Module
	{
	public:
		torch::nn::Conv2d conv;
		torch::nn::BatchNorm2d bn;
	public:
		Linear_Block();
		Linear_Block(int in_c, int out_c, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int groups);
		~Linear_Block();
		torch::Tensor forward(torch::Tensor input);
	};

	class Depth_Wise :public torch::nn::Module
	{
	public:
		Conv_Block conv;
		Conv_Block conv_dw;
		Linear_Block project;
		bool residual;
	public:
		Depth_Wise();
		Depth_Wise(std::vector<int> c1, std::vector<int> c2, std::vector<int> c3, bool Residual, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int groups);
		~Depth_Wise();
		torch::Tensor forward(torch::Tensor input);
	};

	class Residual : torch::nn::Module
	{
	public:
		torch::nn::Sequential models;
	public:
		Residual();
		Residual(std::vector<std::vector<int>> c1, std::vector<std::vector<int>> c2, std::vector<std::vector<int>> c3, int numblock, int groups, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding);
		~Residual();
	public:
		torch::Tensor forward(torch::Tensor input);
	};
	class MiniFASNet
	{

	};

	class SEModule : public torch::nn::Module
	{
	private:
		torch::nn::AdaptiveAvgPool2d avg_pool;
		torch::nn::Conv2d fc1;
		torch::nn::BatchNorm2d bn1;
		torch::nn::ReLU relu;
		torch::nn::Conv2d fc2;
		torch::nn::BatchNorm2d bn2;
		torch::nn::Sigmoid sigmoid();
	public:
		SEModule();
		SEModule(std::vector<int64_t> chanels, int reduction);
		~SEModule();
		torch::Tensor forward(torch::Tensor input);
	};

	class Depth_Wise_SE : public torch::nn::Module
	{
	public:
		Conv_Block conv;
		Conv_Block conv_dw;
		Linear_Block project;
		bool residual;
		SEModule se_module;
	public:
		Depth_Wise_SE();
		Depth_Wise_SE(std::vector<int> c1, std::vector<int> c2, std::vector<int> c3, bool _residual, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int groups = 1, int se_reduct = 8);
		~Depth_Wise_SE();
		torch::Tensor forward(torch::Tensor input);
	};


	class ResidualSE : public Residual
	{	
	public:
		ResidualSE() :Residual::Residual() {};
		ResidualSE(std::vector<std::vector<int>> c1, std::vector<std::vector<int>> c2, std::vector<std::vector<int>> c3, int num_block, int groups, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int se_reduct = 4) : Residual::Residual(c1, c2, c3, num_block, groups, kernel, stride, padding) {};
		~ResidualSE();
	};

	class MiniFASNet : public torch::nn::Module
	{
	public:
		int Embeding_Size;
		Conv_Block conv_1;
		Conv_Block conv_2_dw;
		Depth_Wise conv_23;
		Residual conv_3;
		Depth_Wise conv_34;
		Residual conv_4;
		Depth_Wise conv_45;
		Residual conv_5;
		Conv_Block conv_6_sep;
		Linear_Block conv_6_dw;
		torch::nn::Flatten conv_6_flatten;
		torch::nn::Linear linear;
		torch::nn::BatchNorm1d bn;
		torch::nn::Dropout drop;
		torch::nn::Linear prob;
	public:
		MiniFASNet();
		~MiniFASNet();
		MiniFASNet(std::vector<int> keep, int embedding_size, std::vector<int64_t> conv6_kernel, float drop_p, int num_classess, int img_chanel);
		torch::Tensor forward(torch::Tensor input);
	};

	class MiniFASNetSE : public MiniFASNet
	{
	public:
		MiniFASNetSE();
		~MiniFASNetSE();
		MiniFASNetSE(std::vector<int> keep, int embedding_size, std::vector<int64_t> conv6_kernel, float drop_p, int num_classess, int img_chanel) : MiniFASNet(keep, embedding_size, conv6_kernel, drop_p, num_classess, img_chanel){}
	};

	std::map<std::string, std::vector<int>> keep_dict = { {"1.8M", {32, 32, 103, 103, 64, 13, 13, 64, 26, 26,
																	64, 13, 13, 64, 52, 52, 64, 231, 231, 128,
																	154, 154, 128, 52, 52, 128, 26, 26, 128, 52,
																	52, 128, 26, 26, 128, 26, 26, 128, 308, 308,
																	128, 26, 26, 128, 26, 26, 128, 512, 512} },
														  {"1.8M_", {32, 32, 103, 103, 64, 13, 13, 64, 13, 13, 64, 13,
																	13, 64, 13, 13, 64, 231, 231, 128, 231, 231, 128, 52,
																	52, 128, 26, 26, 128, 77, 77, 128, 26, 26, 128, 26, 26,
																	128, 308, 308, 128, 26, 26, 128, 26, 26, 128, 512, 512} } };
	/// <summary>
	/// (80x80) flops: 0.044, params: 0.41
	/// </summary>
	class MiniFASNetV1: public MiniFASNet
	{
	public:
		MiniFASNetV1();
		~MiniFASNetV1();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="embedding_size">128</param>
		/// <param name="conv_kernel">(7,7)</param>
		/// <param name="drop_p">0.2</param>
		/// <param name="num_classes">3</param>
		/// <param name="img_chanel">3</param>
		MiniFASNetV1(int embedding_size, std::vector<int64_t> conv_kernel, float drop_p, int num_classes, int img_chanel) : MiniFASNet(keep_dict["1.8M"], embedding_size, conv_kernel, drop_p, num_classes, img_chanel) {};
	};
	
	/// <summary>
	/// (80x80) flops: 0.044, params: 0.43
	/// </summary>
	class MiniFASNetV2 : public MiniFASNet
	{
	public:
		MiniFASNetV2();
		~MiniFASNetV2();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="embedding_size">128</param>
		/// <param name="conv_kernel">(7,7)</param>
		/// <param name="drop_p">0.2</param>
		/// <param name="num_classes">3</param>
		/// <param name="img_chanel">2</param>
		MiniFASNetV2(int embedding_size, std::vector<int64_t> conv_kernel, float drop_p, int num_classes, int img_chanel) : MiniFASNet(keep_dict["1.8M_"], embedding_size, conv_kernel, drop_p, num_classes, img_chanel) {};
	};

	class MiniFASNetV1SE : public MiniFASNetSE
	{
	public:
		MiniFASNetV1SE();
		~MiniFASNetV1SE();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="embedding_size">128</param>
		/// <param name="conv_kernel">(7,7)</param>
		/// <param name="drop_p">0.75</param>
		/// <param name="num_classes">3</param>
		/// <param name="img_chanel">3</param>
		MiniFASNetV1SE(int embedding_size, std::vector<int64_t> conv_kernel, float drop_p, int num_classes, int img_chanel) : MiniFASNetSE(keep_dict["1.8M"], embedding_size, conv_kernel, drop_p, num_classes, img_chanel) {};
	};
	class MiniFASNetV2SE : public MiniFASNetSE
	{
	public:
		MiniFASNetV2SE();
		~MiniFASNetV2SE();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="embedding_size">128</param>
		/// <param name="conv_kernel">(7,7)</param>
		/// <param name="drop_p">0.75</param>
		/// <param name="num_classes">3</param>
		/// <param name="img_chanel">3</param>
		MiniFASNetV2SE(int embedding_size, std::vector<int64_t> conv_kernel, float drop_p, int num_classes, int img_chanel) : MiniFASNetSE(keep_dict["1.8M_"], embedding_size, conv_kernel, drop_p, num_classes, img_chanel) {};
	};
}

