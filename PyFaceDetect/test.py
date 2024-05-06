from deepface import DeepFace
import cv2
import time

image_1 = cv2.imread("lee_1.jpg")
image_2 = cv2.imread("lee_2.jpg")
res = DeepFace.verify(image_1, image_2, model_name="VGG-Face", distance_metric="cosine", detector_backend= "yolov8", enforce_detection=False)
timeConsume = 0
for i in range(10):
    c1 = time.time()
    res = DeepFace.verify(image_1, image_2, model_name="VGG-Face", distance_metric="cosine", detector_backend= "yolov8", enforce_detection=False)
    c2 = time.time()
    print(round(c2-c1,2))
    
print( timeConsume /10)