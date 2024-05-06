#include "detector.h"
#include <string>
// OpenCV 4.0 update
#define CV_BGR2RGB cv::COLOR_BGR2RGB
#define CV_BGRA2RGB cv::COLOR_BGR2RGB


MTCNNDetector::MTCNNDetector(const ProposalNetwork::Config &pConfig,
                             const RefineNetwork::Config &rConfig,
                             const OutputNetwork::Config &oConfig) 
{

  _pnet = std::unique_ptr<ProposalNetwork>(new ProposalNetwork(pConfig));
  std::cout << "Load pnet successfull" << '\n';
  
  _rnet = std::unique_ptr<RefineNetwork>(new RefineNetwork(rConfig));
  std::cout << "Load rnet successfull" << '\n';

  _onet = std::unique_ptr<OutputNetwork>(new OutputNetwork(oConfig));
  std::cout << "Load onet successfull" << '\n';

  std::cout << "Create MtCnnModel successfull" << '\n';
}

std::vector<Face> MTCNNDetector::detect(const cv::Mat &img,
                                        const float minFaceSize,
                                        const float scaleFactor) {

  cv::Mat rgbImg;
  if (img.channels() == 3) {
    cv::cvtColor(img, rgbImg, CV_BGR2RGB);
  } else if (img.channels() == 4) {
    cv::cvtColor(img, rgbImg, CV_BGRA2RGB);
  }
  if (rgbImg.empty()) {
    return std::vector<Face>();
  }
  rgbImg.convertTo(rgbImg, CV_32FC3);
  rgbImg = rgbImg.t();

  // Run Proposal Network to find the initial set of faces
  std::vector<Face> faces = _pnet->run(rgbImg, minFaceSize, scaleFactor);

  // Early exit if we do not have any faces
  if (faces.empty()) {
    return faces;
  }

  // Run Refine network on the output of the Proposal network
  faces = _rnet->run(rgbImg, faces);

  // Early exit if we do not have any faces
  if (faces.empty()) {
    return faces;
  }

  // Run Output network on the output of the Refine network
  faces = _onet->run(rgbImg, faces);

  for (size_t i = 0; i < faces.size(); ++i) {
    std::swap(faces[i].bbox.x1, faces[i].bbox.y1);
    std::swap(faces[i].bbox.x2, faces[i].bbox.y2);
    for (int p = 0; p < NUM_PTS; ++p) {
      std::swap(faces[i].ptsCoords[2 * p], faces[i].ptsCoords[2 * p + 1]);
    }
  }

  return faces;
}

static inline bool is_base64(unsigned char c)
{
	return (isalnum(c) || (c == '+') || (c == '/'));
}
static const std::string base64_chars =
"ABCDEFGHIJKLMNOPQRSTUVWXYZ"
"abcdefghijklmnopqrstuvwxyz"
"0123456789+/";
std::string base64_decode(char* base64StringPointer)
{
	std::string const& encoded_string = base64StringPointer;
	int in_len = encoded_string.size();
	int i = 0;
	int j = 0;
	int in_ = 0;
	unsigned char char_array_4[4], char_array_3[3];
	std::string ret;

	while (in_len-- && (encoded_string[in_] != '=') && is_base64(encoded_string[in_]))
	{
		char_array_4[i++] = encoded_string[in_]; in_++;

		if (i == 4)
		{
			for (i = 0; i < 4; i++)
			{
				char_array_4[i] = base64_chars.find(char_array_4[i]);
			}

			char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
			char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
			char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

			for (i = 0; (i < 3); i++)
			{
				ret += char_array_3[i];
			}

			i = 0;
		}
	}

	if (i)
	{
		for (j = i; j < 4; j++)
		{
			char_array_4[j] = 0;
		}

		for (j = 0; j < 4; j++)
		{
			char_array_4[j] = base64_chars.find(char_array_4[j]);
		}

		char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
		char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
		char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

		for (j = 0; (j < i - 1); j++)
		{
			ret += char_array_3[j];
		}
	}

	return ret;
}


void* CreateMTCnnModel(char* path)
{

	std::cout << "Init MtCnnModel" << '\n';
	ProposalNetwork::Config pConfig;
	std::string modelPath = path;
	pConfig.caffeModel = modelPath + "det1.caffemodel";
	pConfig.protoText = modelPath + "det1.prototxt";
	pConfig.threshold = 0.6f;
	RefineNetwork::Config rConfig;
	rConfig.caffeModel = modelPath + "det2.caffemodel";
	rConfig.protoText = modelPath + "det2.prototxt";
	rConfig.threshold = 0.7f;
	OutputNetwork::Config oConfig;
	oConfig.caffeModel = modelPath + "det3.caffemodel";
	oConfig.protoText = modelPath + "det3.prototxt";
	oConfig.threshold = 0.7f;
	std::cout << "Get pnet from path " << pConfig.caffeModel << '\n';
	std::cout << "Get rnet from path " << rConfig.caffeModel << '\n';
	std::cout << "Get onet from path " << oConfig.caffeModel << '\n';
	return new MTCNNDetector(pConfig, rConfig, oConfig);
}


int DetectFace(void* model, char* base64Image, int length, int width, int height, void*& ListImage)
{
    MTCNNDetector* detector = (MTCNNDetector*)model;
	std::string base64DecodeImg = base64_decode(base64Image);
	std::vector<uchar> base64data(base64DecodeImg.begin(), base64DecodeImg.end());
    cv::Mat img = cv::imdecode(base64data, cv::ImreadModes::IMREAD_COLOR);
	std::vector<Face> faces = detector->detect(img, 20.f, 0.709f);
	ListImage = new char[faces.size() * 3 * width * height];
	for (int i = 0; i < faces.size(); i++)
	{
		cv::Mat ResizeFace;
		cv::Mat face = img(faces[i].bbox.getRect());
		cv::resize(face, ResizeFace, cv::Size(width, height));

		_memccpy(ListImage, ResizeFace.data, i, width * height * 3);
		i += width * height * 3;
	}
	return faces.size();
}