import time
import cv2
import base64
import requests


def image_to_base64(image):
    return base64.b64encode(cv2.imencode('.jpg', image)[1]).decode()

image_1 = cv2.imread('lee_1.jpg')
image_2 = cv2.imread('lee_2.jpg')
payload = {
    "image_1": image_to_base64(image_1),
    "image_2": image_to_base64(image_2),
    "model": "ArcFace",
    "metric": "cosine"
}
url = "http://localhost:3000/face_verify"

t = time.time()
res = requests.post(url, json=payload)
print(time.time() - t)
print(res.json())
    