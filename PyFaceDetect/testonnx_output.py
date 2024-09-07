from ast import arg
import cv2
import onnx
import onnxscript
import os
import onnxruntime as ort
import numpy as np
from deepface.basemodels import VGGFace
#from keras_vggface.vggface import VGGFace
#keras_model = VGGFace(model='vgg16')
# image_1 = cv2.imread("dung1.jpg")
# image_2 = cv2.imread("dung2.jpg")

# #{'x': 45, 'y': 63, 'w': 71, 'h': 71, 

# input_face = image_1[63:63+71, 45: 45+71]
# imput_face_resize = cv2.resize(input_face, dsize = (224,224), interpolation=cv2.INTER_CUBIC)
# input_faceVGG = np.expand_dims(imput_face_resize, axis=0)
# #with onnx
# onnx_model_path = "VGGFace.onnx"
# session = ort.InferenceSession(onnx_model_path)
# input_name = session.get_inputs()[0].name
# onnxoutput = session.run(None, {input_name, input_faceVGG.astype(np.float32)})[0]
# print(onnxoutput)

#with keras
keras_model = VGGFace.load_model();
# kerasoutput = model2.predict(input_faceVGG)
# for i in range(0,kerasoutput[0].__len__()):
#     if kerasoutput[0][i] > 0:
#         print (i,":", kerasoutput[0][i])

# Load the ONNX model
onnx_model_path = "VGGFACE.onnx"
session = ort.InferenceSession(onnx_model_path)

# Prepare a sample input
input_data = np.random.rand(1, 224, 224, 3).astype(np.float32)

# Run inference with Keras
keras_output = keras_model.predict(input_data)

# Run inference with ONNX
input_name = session.get_inputs()[0].name
onnx_output = session.run(None, {input_name: input_data})[0]

# Compare outputs
print("Difference between Keras and ONNX outputs:", np.sum(np.abs(keras_output - onnx_output)))
