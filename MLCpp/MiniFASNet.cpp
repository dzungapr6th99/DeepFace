#include "MiniFASNet.h"
namespace MLCpp
{
	L2Norm::L2Norm()
	{

	}
	L2Norm::~L2Norm()
	{

	}
	torch::Tensor L2Norm::forward(torch::Tensor input)
	{
		return torch::nn::functional::normalize(input);
	}

	Flatten::Flatten()
	{
		flatten = torch::nn::Flatten();
	}

	Flatten::~Flatten()
	{

	}

	torch::Tensor Flatten::forward(torch::Tensor input)
	{
		return flatten(input);
	}
	Conv_Block::Conv_Block()
	{

	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="in_c"></param>
	/// <param name="out_c"></param>
	/// <param name="kernel">(1,1)</param>
	/// <param name="stride">(1,1)</param>
	/// <param name="padding">(0,0)</param>
	/// <param name="groups">1</param>
	Conv_Block::Conv_Block(int in_c, int out_c, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int groups = 1)
	{
		conv = torch::nn::Conv2d(torch::nn::Conv2dOptions(in_c, out_c, kernel).stride(stride).padding(padding).groups(groups));
		bn = torch::nn::BatchNorm2d(out_c);
		prelu = torch::nn::PReLU(out_c);
	}

	Conv_Block::~Conv_Block()
	{

	}

	torch::Tensor Conv_Block::forward(torch::Tensor input)
	{
		input = conv(input);
		input = bn(input);
		input = prelu(input);
		return input;
	}

	Linear_Block::Linear_Block()
	{

	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="in_c"></param>
	/// <param name="out_c"></param>
	/// <param name="kernel"></param>
	/// <param name="stride"></param>
	/// <param name="padding"></param>
	/// <param name="groups">default is 1</param>
	Linear_Block::Linear_Block(int in_c, int out_c, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int groups)
	{
		conv = torch::nn::Conv2d(torch::nn::Conv2dOptions(in_c, out_c, kernel).stride(stride).padding(padding).groups(groups));
		bn = torch::nn::BatchNorm2d(out_c);
	}
	Linear_Block::~Linear_Block()
	{

	}
	torch::Tensor Linear_Block::forward(torch::Tensor input)
	{
		input = conv(input);
		input = bn(input);
		return input;
	}

	Depth_Wise::Depth_Wise()
	{

	}
	/// <summary>
	/// hàm Depth_wise
	/// </summary>
	/// <param name="c1"></param>
	/// <param name="c2"></param>
	/// <param name="c3"></param>
	/// <param name="Residual">không nói gì thì là false</param>
	/// <param name="kernel"></param>
	/// <param name="stride"></param>
	/// <param name="padding"></param>
	/// <param name="groups"></param>
	Depth_Wise::Depth_Wise(std::vector<int> c1, std::vector<int> c2, std::vector<int> c3, bool Residual, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int groups)
	{
		std::vector<int64_t> kernel_1({ 1,1 });
		std::vector<int64_t> stride_1({ 1,1 });
		std::vector<int64_t> padding_1({ 0,0 });
		conv = Conv_Block(c1[0], c1[1], kernel_1, stride_1, padding_1, 1);
		conv_dw = Conv_Block(c2[0], c2[1], kernel_1, stride, padding, 1);
		project = Linear_Block(c3[0], c3[1], kernel_1, stride_1, padding_1, 1);
		residual = Residual;
	}
	Depth_Wise::~Depth_Wise()
	{

	}
	torch::Tensor Depth_Wise::forward(torch::Tensor input)
	{

		if (residual)
		{
			torch::Tensor shortcut = input;
			input = conv.forward(input);
			input = conv_dw.forward(input);
			input = project.forward(input);
			return shortcut + input;
		}
		else
		{
			input = conv.forward(input);
			input = conv_dw.forward(input);
			input = project.forward(input);
			return input;
		}
	}

	Residual::Residual()
	{

	}

	Residual::Residual(std::vector<std::vector<int>> c1, std::vector<std::vector<int>> c2, std::vector<std::vector<int>> c3, int numblock, int groups, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding)
	{
		std::vector<torch::nn::Module> Modules;
		for (int i = 0; i < numblock; i++)
		{
			std::vector<int> c1_tuple = c1[i];
			std::vector<int> c2_tuple = c2[i];
			std::vector<int> c3_tuple = c3[i];
			Depth_Wise d_w = Depth_Wise(c1_tuple, c2_tuple, c3_tuple, true, kernel, stride, padding, groups);
			Modules.push_back(d_w);

		}
		models = torch::nn::Sequential(Modules);
	}
	Residual::~Residual()
	{

	}

	torch::Tensor Residual::forward(torch::Tensor input)
	{
		for (int i = 0; i < models.get()->size(); i++)
		{
			input = models.get()[i].forward(input);
		}
		return input;
	}

	SEModule::SEModule()
	{

	}

	SEModule::~SEModule()
	{

	}

	SEModule::SEModule(std::vector<int64_t> chanels, int reduction)
	{
		avg_pool = torch::nn::AdaptiveAvgPool2d(1);
		std::vector<int64_t> chanels_1;
		for (int i = 0; i < chanels.size(); i++)
			chanels_1.push_back(chanels[i] / reduction);
		fc1 = torch::nn::Conv2d(chanels_1, chanels, 1);
	}

	torch::Tensor SEModule::forward(torch::Tensor input)
	{
		torch::Tensor Model_Input = input;
		input = avg_pool(input);
		input = fc1(input);
		input = bn1(input);
		input = relu(input);
		input = fc2(input);
		input = bn2(input);
		input = torch::sigmoid(input);
		return Model_Input * input;
	}

	Depth_Wise_SE::Depth_Wise_SE() {}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="c1"></param>
	/// <param name="c2"></param>
	/// <param name="c3"></param>
	/// <param name="_residual">không nói gì thì là false</param>
	/// <param name="kernel"></param>
	/// <param name="stride"></param>
	/// <param name="padding"></param>
	/// <param name="groups"></param>
	/// <param name="se_reduct"></param>
	Depth_Wise_SE::Depth_Wise_SE(std::vector<int> c1, std::vector<int> c2, std::vector<int> c3, bool _residual, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int groups = 1, int se_reduct = 8)
	{
		std::vector<int64_t> kernel_1({ 1,1 });
		std::vector<int64_t> stride_1({ 1,1 });
		std::vector<int64_t> padding_1({ 0,0 });
		conv = Conv_Block(c1[0], c1[1], kernel_1, stride_1, padding_1, 1);
		conv_dw = Conv_Block(c2[0], c2[1], kernel_1, stride, padding, c2[0]);
		project = Linear_Block(c3[0], c3[1], kernel_1, stride_1, padding_1, 1);
		residual = _residual;
	}
	Depth_Wise_SE::~Depth_Wise_SE() {}
	torch::Tensor Depth_Wise_SE::forward(torch::Tensor input)
	{
		torch::Tensor output = input;
		input = conv.forward(input);
		input = conv_dw.forward(input);
		input = project.forward(input);
		if (residual)
		{
			output = output + input;
		}
		else
			output = input;
		return output;
	}

	ResidualSE::ResidualSE()
	{

	}

	ResidualSE::ResidualSE(std::vector<std::vector<int>> c1, std::vector<std::vector<int>> c2, std::vector<std::vector<int>> c3, int num_block, int groups, std::vector<int64_t> kernel, std::vector<int64_t> stride, std::vector<int64_t> padding, int se_reduct = 4)
	{

		for (int i = 0; i < num_block; i++)
		{
			if (i == num_block - 1)
			{
				Modules.push_back(Depth_Wise_SE(c1[i], c2[i], c3[i], true, kernel, stride, padding, 1, 8));
			}
			else
			{
				Modules.push_back(Depth_Wise(c1[i], c2[i], c3[i], true, kernel, stride, padding, groups));
			}

		}

	}
	ResidualSE:: ~ResidualSE() {}

	MiniFASNet::MiniFASNet() {}
	MiniFASNet::MiniFASNet(std::vector<int> keep, int embedding_size, std::vector<int64_t> conv6_kernel, float drop_p, int num_classess, int img_chanel)
	{
		std::vector<int64_t> kernel1({ 1,1 });
		std::vector<int64_t> kernel3({ 3,3 });
		std::vector<int64_t> stride1({ 1,1 });
		std::vector<int64_t> stride2({ 2,2 });
		std::vector<int64_t> padding1({ 1,1 });
		std::vector<int64_t> padding0({ 0,0 });
		Embeding_Size = embedding_size;
		conv_1 = Conv_Block(img_chanel, keep[0], kernel3, stride2, padding1, 1);
		conv_2_dw = Conv_Block(keep[0], keep[1], kernel3, stride1, padding1, keep[1]);

		std::vector<std::vector<int>> c1 = { {keep[1],keep[2]} };
		std::vector<std::vector<int>> c2 = { {keep[2],keep[3]} };
		std::vector<std::vector<int>> c3 = { {keep[3],keep[4]} };

		conv_23 = Depth_Wise(c1[0], c2[0], c3[0], false, kernel3, stride2, padding1, keep[3]);

		c1 = { {keep[4], keep[5]}, {keep[7], keep[8]}, {keep[10], keep[11]}, {keep[13], keep[14]} };
		c2 = { {keep[5], keep[6]}, {keep[8], keep[9]}, {keep[11], keep[12]}, {keep[14], keep[15]} };
		c3 = { {keep[6], keep[7]}, {keep[9], keep[10]}, {keep[12], keep[13]}, {keep[15], keep[16]} };

		conv_3 = Residual(c1, c2, c3, 4, keep[4], kernel3, stride1, padding1);

		c1 = { {keep[16], keep[17]} };
		c2 = { {keep[17], keep[18]} };
		c3 = { {keep[18], keep[19]} };

		conv_34 = Depth_Wise(c1[0], c2[0], c3[0], false, kernel3, stride2, padding1, keep[19]);

		c1 = { {keep[19], keep[20]}, {keep[22], keep[23]}, {keep[25], keep[26]}, {keep[28], keep[29]},
			{keep[31], keep[32]}, {keep[34], keep[35]} };
		c2 = { {keep[20], keep[21]}, {keep[23], keep[24]}, {keep[26], keep[27]}, {keep[29], keep[30]},
			{keep[32], keep[33]}, {keep[35], keep[36] } };
		c3 = { {keep[21], keep[22]}, {keep[24], keep[25]}, {keep[27], keep[28]}, {keep[30], keep[31]},
			{keep[33], keep[34]}, {keep[36], keep[37]} };

		conv_4 = Residual(c1, c2, c3, 6, keep[19], kernel3, stride2, padding1);

		c1 = { {keep[37], keep[38]} };
		c2 = { {keep[38], keep[39]} };
		c3 = { {keep[39], keep[40]} };

		conv_45 = Depth_Wise(c1[0], c2[0], c3[0], false, kernel3, stride2, padding1, keep[40]);

		c1 = { {keep[40], keep[41]}, {keep[43], keep[44]} };
		c2 = { {keep[41], keep[42]}, {keep[44], keep[45]} };
		c3 = { {keep[42], keep[43]}, {keep[45], keep[46]} };

		conv_5 = Residual(c1, c2, c3, 2, keep[40], kernel3, stride1, padding0);
		conv_6_sep = Conv_Block(keep[46], keep[47], kernel1, stride1, padding0);
		conv_6_dw = Linear_Block(keep[47], keep[48], conv6_kernel, stride1, padding0, 1);
		conv_6_flatten = torch::nn::Flatten();
		linear = torch::nn::Linear(512, Embeding_Size, false);
		bn = torch::nn::BatchNorm1d(embedding_size);
		drop = torch::nn::Dropout(drop_p);
		prob = torch::nn::Linear(Embeding_Size, num_classess, false);
	}

	MiniFASNet::~MiniFASNet() {}

	torch::Tensor MiniFASNet::forward(torch::Tensor input)
	{

		torch::Tensor out = conv_1.forward(input);
		out = conv_2_dw.forward(out);
		out = conv_23.forward(out);
		out = conv_3.forward(out);
		out = conv_34.forward(out);
		out = conv_4.forward(out);
		out = conv_4.forward(out);
		out = conv_45.forward(out);
		out = conv_5.forward(out);
		out = conv_6_sep.forward(out);
		out = conv_6_dw.forward(out);
		out = conv_6_flatten(out);
		if (Embeding_Size != 512)
		{
			out = linear(out);
		}
		out = bn(out);
		out = drop(out);
		out = prob(out);
		return out;
	}

	MiniFASNetSE::MiniFASNetSE()
	{

	}

	MiniFASNetSE::~MiniFASNetSE()
	{

	}

	MiniFASNetSE::MiniFASNetSE(std::vector<int> keep, int embedding_size, std::vector<int64_t> conv6_kernel, float drop_p, int num_classess, int img_chanel) : MiniFASNet(keep, embedding_size, conv6_kernel, drop_p, num_classess, img_chanel)
	{
		std::vector<int64_t> kernel1({ 1,1 });
		std::vector<int64_t> kernel3({ 3,3 });
		std::vector<int64_t> stride1({ 1,1 });
		std::vector<int64_t> stride2({ 2,2 });
		std::vector<int64_t> padding1({ 1,1 });
		std::vector<int64_t> padding0({ 0,0 });
		std::vector<std::vector<int>> c1 = { {keep[4], keep[5]}, {keep[7], keep[8]}, {keep[10], keep[11]}, {keep[13], keep[14]} };
		std::vector<std::vector<int>> c2 = { {keep[5], keep[6]}, {keep[8], keep[9]}, {keep[11], keep[12]}, {keep[14], keep[15]} };
		std::vector<std::vector<int>> c3 = { {keep[6], keep[7]}, {keep[9], keep[10]}, {keep[12], keep[13]}, {keep[15], keep[16]} };

		conv_3 = ResidualSE(c1, c2, c3, 4, keep[4], kernel3, stride1, padding1, 4);

		c1 = { {keep[19], keep[20]}, {keep[22], keep[23]}, {keep[25], keep[26]}, {keep[28], keep[29]},
			{keep[31], keep[32]}, {keep[34], keep[35]} };
		c2 = { {keep[20], keep[21]}, {keep[23], keep[24]}, {keep[26], keep[27]}, {keep[29], keep[30]},
			{keep[32], keep[33]}, {keep[35], keep[36]} };
		c3 = { {keep[21], keep[22]}, {keep[24], keep[25]}, {keep[27], keep[28]}, {keep[30], keep[31]},
			{keep[33], keep[34]}, {keep[36], keep[37]} };

		conv_4 = ResidualSE(c1, c2, c3, 6, keep[19], kernel3, stride1, padding1, 4);

		c1 = { {keep[40], keep[41]}, {keep[43], keep[44]} };
		c2 = { {keep[41], keep[42]}, {keep[44], keep[45]} };
		c3 = { {keep[42], keep[43]}, {keep[45], keep[46]} };
		conv_5 = ResidualSE(c1, c2, c3, 2, keep[40], kernel3, stride1, padding1);
	}

	MiniFASNetV1::MiniFASNetV1()
	{

	}

	MiniFASNetV1::~MiniFASNetV1()
	{

	}

	MiniFASNetV2::MiniFASNetV2()
	{

	}

	MiniFASNetV2::~MiniFASNetV2()
	{

	}

}