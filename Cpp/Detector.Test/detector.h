#ifndef _include_opencv_mtcnn_detector_h_
#define _include_opencv_mtcnn_detector_h_

#include "face.h"
#include "onet.h"
#include "pnet.h"
#include "rnet.h"
#include "pch.h"
#define CPPEXPORT __declspec(dllexport)

class MTCNNDetector {
private:
  std::unique_ptr<ProposalNetwork> _pnet;
  std::unique_ptr<RefineNetwork> _rnet;
  std::unique_ptr<OutputNetwork> _onet;

public:
  MTCNNDetector(const ProposalNetwork::Config &pConfig,
                const RefineNetwork::Config &rConfig,
                const OutputNetwork::Config &oConfig);
  std::vector<Face> detect(const cv::Mat &img, const float minFaceSize,
                           const float scaleFactor);
};
extern "C"
{
    void* CreateMTCnnModel();
    int DetectFace(void* model, char* base64Image, int length, int width, int height, void*& ListImage);
}
#endif
