#ifndef _include_opencv_mtcnn_detector_h_
#define _include_opencv_mtcnn_detector_h_

#include "face.h"
#include "onet.h"
#include "pnet.h"
#include "rnet.h"

#ifndef CPPEXPORT
# if (defined _WIN32 || defined WINCE || defined __CYGWIN__) && defined(CVAPI_EXPORTS)
#   define CPPEXPORT __declspec(dllexport)
# elif defined __GNUC__ && __GNUC__ >= 4 && (defined(CVAPI_EXPORTS) || defined(__APPLE__))
#   define CPPEXPORT __attribute__ ((visibility ("default")))
# endif
#endif
#ifndef CPPEXPORT
# define CPPEXPORT
#endif
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
    CPPEXPORT void* CreateMTCnnModel(char* path);
    CPPEXPORT int DetectFace(void* model, char* base64Image, int length, int width, int height, void*& ListImage);
}
#endif
