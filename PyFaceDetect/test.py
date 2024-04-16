from deepface import Deepface
import cv2
import datetime

image_1 = cv2.imread("lee_1.jpg")
image_2 = cv2.imread("lee_2.jpg")
res = Deepface.verify(image_1, image_2, model_name="VGG-Face", distance_metric="cosine", enforce_detection=False)
timeConsume = 0
for i in range(10):
    c1 = datetime.datetime.now
    res = Deepface.verify(image_1, image_2, model_name="VGG-Face", distance_metric="cosine", enforce_detection=False)
    c2 = datetime.datetime.now
    c = (c2-c1)
    timeConsume = timeConsume+c.milisecond
    
print( timeConsume /10)