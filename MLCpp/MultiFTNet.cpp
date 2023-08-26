#include "MultiFTNet.h"
namespace MLCpp
{
    FTGenerator::FTGenerator()
    {
        int in_channels = 48;
        int out_channels = 1;
        std::vector<int64_t> kernel3 = { 3,3 };

        ft = torch::nn::Sequential(
            torch::nn::Conv2d(torch::nn::Conv2dOptions(in_channels, 128, kernel3).padding(1)),
            torch::nn::BatchNorm2d(128),
            torch::nn::ReLU(true),

            torch::nn::Conv2d(torch::nn::Conv2dOptions(128, 64, kernel3).padding(1)),
            torch::nn::BatchNorm2d(64),
            torch::nn::ReLU(true),

            torch::nn::Conv2d(torch::nn::Conv2dOptions(64, out_channels, kernel3).padding(1)),
            torch::nn::BatchNorm2d(out_channels),
            torch::nn::ReLU(true)
        );
    }

	FTGenerator::FTGenerator(int in_channels = 48, int out_channels =1)
	{
        std::vector<int64_t> kernel3 = { 3,3 };

        ft = torch::nn::Sequential(
            torch::nn::Conv2d(torch::nn::Conv2dOptions(in_channels, 128, kernel3).padding(1)),
            torch::nn::BatchNorm2d(128),
            torch::nn::ReLU(true),

            torch::nn::Conv2d(torch::nn::Conv2dOptions(128, 64, kernel3).padding(1)),
            torch::nn::BatchNorm2d(64),
            torch::nn::ReLU(true),

            torch::nn::Conv2d(torch::nn::Conv2dOptions(64, out_channels, kernel3).padding(1)),
            torch::nn::BatchNorm2d(out_channels),
            torch::nn::ReLU(true)
        );
	}

    FTGenerator::~FTGenerator() { }

    torch::Tensor FTGenerator::forward(torch::Tensor input)
    {
        return ft->forward(input);
    }

    MultiFTNet::MultiFTNet()
    {
       
    }

    MultiFTNet::~MultiFTNet()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="img_chanels">3</param>
    /// <param name="num_classes">3</param>
    /// <param name="embedding_size">128</param>
    /// <param name="conv6_kernel">(3,3)</param>
    MultiFTNet::MultiFTNet(int img_chanels, int num_classes, int embedding_size, std::vector<int64_t> conv6_kernel)
    {
        img_chanel = img_chanels;
        this->num_classes = num_classes;
        this->Model = MiniFASNetV2SE(embedding_size, conv6_kernel, 0.75, num_classes, img_chanel);
        ftGenerator = FTGenerator(128, 1);

    }

    void MultiFTNet::initialize_weights()
    {
        for (auto& m : this->modules()) {
            if (auto conv2d = dynamic_cast<torch::nn::Conv2dImpl*>(m.get())) {
                torch::nn::init::kaiming_normal_(conv2d->weight, 0.0, torch::kFanOut, torch::kReLU);
                if (conv2d->bias.defined()) {
                    torch::nn::init::constant_(conv2d->bias, 0);
                }
            }
            else if (auto batch_norm = dynamic_cast<torch::nn::BatchNorm1dImpl*>(m.get())) {
                torch::nn::init::constant_(batch_norm->weight, 1);
                torch::nn::init::constant_(batch_norm->bias, 0);
            }
            else if (auto group_norm = dynamic_cast<torch::nn::GroupNormImpl*>(m.get())) {
                torch::nn::init::constant_(group_norm->weight, 1);
                torch::nn::init::constant_(group_norm->bias, 0);
            }
            else if (auto linear = dynamic_cast<torch::nn::LinearImpl*>(m.get())) {
                torch::nn::init::normal_(linear->weight, 0, 0.001);
                if (linear->bias.defined()) {
                    torch::nn::init::constant_(linear->bias, 0);
                }
            }
        }
    }

    torch::Tensor MultiFTNet::forward(torch::Tensor input)
    {
        torch::Tensor x = Model.conv_1.forward(input);
        x = Model.conv_2_dw.forward(x);
        x = Model.conv_23.forward(x);
        x = Model.conv_3.forward(x);
        x = Model.conv_34.forward(x);
        x = Model.conv_4.forward(x);
        torch::Tensor x1 = Model.conv_45.forward(x);
        x1 = Model.conv_5.forward(x1);
        x1 = Model.conv_6_sep.forward(x1);
        x1 = Model.conv_6_dw.forward(x1);
        x1 = Model.conv_6_flatten(x1);
        x1 = Model.linear(x1);
        x1 = Model.bn(x1);
        x1 = Model.drop(x1);
        torch::Tensor cls = Model.prob(x1);
        return cls;
    }
}