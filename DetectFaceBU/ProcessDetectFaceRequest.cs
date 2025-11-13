using CommonLib;
using DetectFaceObject;
using FaceDetectInterface;
using MilvusDA.Interface;
using PreProcess;
using System.Diagnostics;
using VectorDbObj;
using MilvusDA.Extension;
using PreProcess.Interface;
using DetectFaceBU.Interface;
using FaceDetectInterface.Interface;
using System.Drawing;
namespace DetectFaceBU
{
   
    public class ProcessDetectFaceRequest : IProcessDetectFaceRequest
    {
        private readonly IDetectorModel _detectorModel;
        private readonly IFaceDetect _faceDetectModel;
        private readonly IMilvusHelper _milvusHelper;
        public ProcessDetectFaceRequest(IDetectorModel p_DetectorModel, IFaceDetect p_FaceDetect, IMilvusHelper milvusHelper)
        {
            _detectorModel = p_DetectorModel;
            _faceDetectModel = p_FaceDetect;
            _milvusHelper = milvusHelper;
        }
        public async Task<VerifyFaceResponse> Api1VerifyFaceBU(VerifyFaceRequest request)
        {

            try
            {

                LOG.log.Info("Start Process request {0}", request.RequestID);
                LOG.log.Debug("Image check: {0}", request.ImageCheck);
                LOG.log.Debug("Image verify: {0}", request.ImageVerify);
                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                bool IsVerified = _faceDetectModel.Verify(request.ImageVerify, request.ImageCheck);
                stopwatch.Stop();
                VerifyFaceResponse response = new VerifyFaceResponse()
                {
                    Verified = IsVerified,
                    ReturnCode = 1,
                    ReturnMessage = "Success"
                };
                LOG.log.Info("Process request {0} success in {1} ms and get result {2}", request.RequestID, stopwatch.ElapsedMilliseconds, response.Verified);
                return response;
            }
            catch (Exception ex)
            {
                LOG.log.Error(ex);
                return new VerifyFaceResponse()
                {
                    Verified = false,
                    ReturnCode = -999,
                    ReturnMessage = "Service get unknown error"

                };
            }
            catch
            {
                return new VerifyFaceResponse
                {
                    Verified = false,
                    ReturnCode = -999,
                    ReturnMessage = "Service get Cls error"
                };
            }

        }

        public async Task<EmbedingFaceResponse> Api2EmbedingFaceBU(EmbedingFaceRequest request)
        {
            try
            {
                List<List<float>>? embedingVectors = await Task.Run(()=> _faceDetectModel.Embeding(request.ImageBase, out var faceCoordinates));
                if (embedingVectors != null)
                {
                    if (embedingVectors.Count == 1)
                    {
                        FaceDbObject faceEmebeding = new FaceDbObject()
                        {
                            FaceId = request.FaceId,
                            EmbededVector = embedingVectors[0]
                        };
                        bool insertEmbed = await _milvusHelper.InsertPost<FaceDbObject>(faceEmebeding);
                        if (insertEmbed)
                        {
                            return new EmbedingFaceResponse()
                            {
                                Code = 1,
                                Message = "Success"
                            };
                        }
                    }
                    else if (embedingVectors.Count > 1)
                    {
                        return new EmbedingFaceResponse()
                        {
                            Code = -1,
                            Message = "There are more than 1 face in this image, please use image with only 1 face"
                        };
                    } 
                    else
                    {
                        return new EmbedingFaceResponse()
                        {
                            Code = -1,
                            Message = "There is no face in this image"
                        };
                    }    
                        
                }
               
                return new EmbedingFaceResponse()
                {
                    Code = -1,
                    Message = "Cannot embed face"
                };
            }
            catch (Exception ex)
            {
                LOG.log.Error(ex);
                return new EmbedingFaceResponse()
                {
                    Code = -999,
                    Message = ex.Message,
                };
            }
        }

        public async Task<DetectFaceResponse> Api3DetectFaceBU(DetectFaceRequest request)
        {
            try
            {
                List<Rectangle> faceCoordinates;
                List<List<float>>? embedingVectors = _faceDetectModel.Embeding(request.ImageDetect, out faceCoordinates);


                if (embedingVectors != null && embedingVectors.Count > 0)
                {
                    List<object> listEmbedingVector = new List<object>();
                    for (int i = 0; i < embedingVectors.Count; i++) 
                    {
                        listEmbedingVector.Add(embedingVectors[i]);
                    }
                    var searchEmbed = await _milvusHelper.Search<VectorDbObj.FaceDbObject>(listEmbedingVector);
                    if (searchEmbed != null)
                    {
                        return new DetectFaceResponse()
                        {
                            ReturnCode = 1,
                            ReturnMessage = "Success",
                            facesCoordinates = faceCoordinates,
                            faceNames = searchEmbed.Select(x=> x.FaceId).ToList()
                        };
                    }
                }
                return new DetectFaceResponse()
                {
                    ReturnCode = -1,
                    ReturnMessage = "Cannot embed face"
                };

            }
            catch (Exception ex)
            {
                LOG.log.Error(ex);
                return new DetectFaceResponse()
                {
                    ReturnCode = -999,
                    ReturnMessage = ex.Message,
                    Detected = false
                };

            }
            
        }

        public async Task<DetectFaceResponse> Api2DetectFaceValid(DetectFaceRequest request)
        {
            try
            {

                LOG.log.Info("Start Process request {0}", request.RequestID);

                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                bool IsVerified = _faceDetectModel.Detect(request.ImageDetect);
                stopwatch.Stop();
                DetectFaceResponse response = new DetectFaceResponse()
                {
                    Detected = IsVerified,
                    ReturnCode = 1,
                    ReturnMessage = "Success"
                };
                LOG.log.Info("Process request {0} success in {1} ms", request.RequestID, stopwatch.ElapsedMilliseconds);
                return response;
            }
            catch (Exception ex)
            {
                LOG.log.Error(ex);
                return new DetectFaceResponse()
                {
                    Detected = false,
                    ReturnCode = -999,
                    ReturnMessage = "Service get unknown error"

                };
            }
            catch
            {
                return new DetectFaceResponse
                {
                    Detected = false,
                    ReturnCode = -999,
                    ReturnMessage = "Service get Cls error"
                };
            }
        }
    }
}
