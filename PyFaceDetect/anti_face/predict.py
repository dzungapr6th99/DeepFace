import os
import cv2
import numpy as np
import warnings
import time
from anti_face.anti_spoof_predict import AntiSpoofPredict
from anti_face.generate_patches import CropImage
warnings.filterwarnings('ignore')

model_dir = "resources/anti_spoof_models"

image_cropper = CropImage()

model_dict = {'1': {'model_name': '2.7_80x80_MiniFASNetV2.pth', 'scale': 2.7, 'out_w': 80, 'out_h': 80, 'crop': True}, 
              '2': {'model_name': '4_0_0_80x80_MiniFASNetV1SE.pth', 'scale': 4.0, 'out_w': 80, 'out_h': 80, 'crop': True}}

model_1 = AntiSpoofPredict(model_path=os.path.join(model_dir, model_dict['1']['model_name']))
model_2 = AntiSpoofPredict(model_path=os.path.join(model_dir, model_dict['2']['model_name']))
param_1 = {'scale': model_dict['1']['scale'], 'out_w': model_dict['1']['out_w'], 'out_h': model_dict['1']['out_h'], 'crop': model_dict['1']['crop']}
param_2 = {'scale': model_dict['2']['scale'], 'out_w': model_dict['2']['out_w'], 'out_h': model_dict['2']['out_h'], 'crop': model_dict['2']['crop']}

list_model = [model_1, model_2]
list_param = [param_1, param_2]

def check_spoofing(image):
    image_bbox = [0, 0, image.shape[1], image.shape[0]]
    prediction = np.zeros((1, 3))
    for idx, model in enumerate(list_model):
        param = list_param[idx].copy()
        param['org_img'] = image
        param['bbox'] = image_bbox
        img = image_cropper.crop(**param)
        prediction += model.predict(img)
    label = np.argmax(prediction)
    return label