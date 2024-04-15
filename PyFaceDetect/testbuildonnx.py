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


<<<<<<< HEAD
model2 = VGGFace.load_model()
=======
model2 = Facenet.load_facenet128d_model()
>>>>>>> d52732b9f3bc244a910e59c5744ae2efbae3bac8
print(model2.input_shape)
print(model2.input)
# input_signature = [tf.TensorSpec(model2.input_shape, tf.float32, name ='x')]

onnx_model, onnx_output = tf2onnx.convert.from_keras(model2)
onnx.save(onnx_model, "Facenet128d.onnx")