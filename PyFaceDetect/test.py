import DeepFace
import cv2

image_1 = cv2.imread("lee_1.jpg")
image_2 = cv2.imread("lee_2.jpg")

for i in range(3):
    res = DeepFace.verify(image_1, image_2, model_name="ArcFace", distance_metric="cosine", enforce_detection=False)
    print(res)