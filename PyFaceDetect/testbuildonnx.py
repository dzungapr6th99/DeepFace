
from deepface import DeepFace
from deepface.models import FacialRecognition
from deepface.modules.modeling import Facenet, VGGFace
import tf2onnx
import onnx
import torch
import onnxscript
import os
import tensorflow as tf
from deepface.commons import package_utils, folder_utils
import onnxruntime as ort
import cv2


from anti_face.model_lib.MiniFASNet import MiniFASNet, MiniFASNetV1

#from deepface.DeepFace import MTCNN, MtCnnClient
#gpuSessionOption = ort.SessionOptions.MakeSessionOptionWithCudaProvider(0)
#model1 = VGGFace.VggFaceClient()
model2 = Facenet.load_facenet512d_model()

input_signature = [tf.TensorSpec(model2.input_shape, tf.float32, name ='x')]
onnx_model, onnx_output = tf2onnx.convert.from_keras(model2)
onnx.save(onnx_model, "FaceNet512.onnx")
#model2 = MiniFASNetV1()
#torch_input = torch.rand()
#minifasV1OnnxModel = torch.onnx.export(model2, )
#minifasV1OnnxModel.save("MiniFASNetV1.onnx")
# print(model2.input)
# model3 = YoloClient()
# model3.build_model()
# input_signature = [tf.TensorSpec(model2.input_shape, tf.float32, name ='x')]
# onnx_model, onnx_output = tf2onnx.convert.from_keras(model2)
# onnx.save(onnx_model, "VGGFace.onnx")
# print(onnx.checker.check_model(onnx_model))

# onnxProgram = torch.onnx.export(model= model3,  args = torch_input,f="YoloV8.onnx", verbose=True)
# onnxProgram.save("YoloV8.onnx")
#image_2 = cv2.imread("lee_2.jpg")

#model3.detect_faces(image_2)
#model4 = MtCnnClient()
#model4.detect_faces(image_2);

#miniFas = MiniFASNet.MiniFASNetV1()
