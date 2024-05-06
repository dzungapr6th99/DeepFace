from ast import arg
from deepface.basemodels import Facenet
from deepface.basemodels import VGGFace
import tf2onnx
import onnx
import torch
import onnxscript
import os
import tensorflow as tf
from deepface.commons import package_utils, folder_utils
import onnxruntime as ort
import cv2
from keras.models import Model, Sequential
from keras.layers import (
        Convolution2D,
        ZeroPadding2D,
        MaxPooling2D,
        Flatten,
        Dropout,
        Activation,
    )
from deepface.detectors.Yolo import Any, YoloClient
from deepface.detectors.MtCnn import MTCNN, MtCnnClient
#gpuSessionOption = ort.SessionOptions.MakeSessionOptionWithCudaProvider(0)

#model2 = VGGFace.load_model()
#print(model2.input_shape)
#print(model2.input)
model3 = YoloClient()
model3.build_model()
# input_signature = [tf.TensorSpec(model2.input_shape, tf.float32, name ='x')]
#onnx_model, onnx_output = tf2onnx.convert.from_keras(model2)
torch_input = torch.randn()
onnxProgram = torch.onnx.export(model= model3,  args = torch_input,f="YoloV8.onnx", verbose=True)
onnxProgram.save("YoloV8.onnx")
image_2 = cv2.imread("lee_2.jpg")

model3.detect_faces(image_2)
model4 = MtCnnClient()
model4.detect_faces(image_2);