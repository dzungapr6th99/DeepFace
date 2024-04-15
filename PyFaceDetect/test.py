<<<<<<< HEAD
from deepface import DeepFace
=======
from deepface import Deepface
>>>>>>> d52732b9f3bc244a910e59c5744ae2efbae3bac8
import cv2

image_1 = cv2.imread("lee_1.jpg")
image_2 = cv2.imread("lee_2.jpg")

for i in range(3):
<<<<<<< HEAD
    res = DeepFace.verify(image_1, image_2, model_name="VGG-Face", distance_metric="cosine", enforce_detection=False)
=======
    res = Deepface.verify(image_1, image_2, model_name="ArcFace", distance_metric="cosine", enforce_detection=False)
>>>>>>> d52732b9f3bc244a910e59c5744ae2efbae3bac8
    print(res)