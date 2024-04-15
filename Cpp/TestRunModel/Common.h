#pragma once
#include<vector>
#include<torch/all.h>
#include<opencv2/opencv.hpp>
#include<opencv2/core/mat.hpp>
namespace AntiSpoofing
{
    static torch::Tensor ToTensor(cv::Mat img = cv::Mat(), bool show_output = false, bool unsqueeze = false, int unsqueeze_dim = 0)
    {
        std::cout << "image shape: " << img.size() << std::endl;
        at::Tensor tensor_image = torch::from_blob(img.data, { img.rows, img.cols, 3 }, at::kByte);

        if (unsqueeze)
        {
            tensor_image.unsqueeze_(unsqueeze_dim);
        }

        if (show_output)
        {
            std::cout << tensor_image.slice(2, 0, 1) << std::endl;
        }
        return tensor_image;
    }
    class Common
    {
    };
    class Point2D
    {
    public:
        int x;
        int y;
        Point2D(int X, int Y);
        ~Point2D();
        Point2D();
        void GetCoordinates(int x, int y);
    };
}


