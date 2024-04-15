from deepface.basemodels import Facenet
from deepface.basemodels import VGGFace
import tf2onnx
import onnx
import os
import tensorflow as tf
from deepface.commons import package_utils, folder_utils
from keras.models import Model, Sequential
from keras.layers import (
        Convolution2D,
        ZeroPadding2D,
        MaxPooling2D,
        Flatten,
        Dropout,
        Activation,
    )


model2 = VGGFace.load_model()
print(model2.input_shape)
print(model2.input)
# input_signature = [tf.TensorSpec(model2.input_shape, tf.float32, name ='x')]

onnx_model, onnx_output = tf2onnx.convert.from_keras(model2)
onnx.save(onnx_model, "Facenet128d.onnx")