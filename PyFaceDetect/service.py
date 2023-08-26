import bentoml
import numpy as np
from bentoml.io import JSON
import base64
import cv2
import DeepFace
import sqlite3
import pandas as pd
import uuid
from deepface.commons import distance as dst
import os

PREFIX_IMG = os.path.dirname(__file__)


def base64_to_image(base64_string):
    imgdata = base64.b64decode(base64_string)
    return cv2.imdecode(np.frombuffer(imgdata, np.uint8), -1)


def image_to_base64(image):
    return base64.b64encode(cv2.imencode('.jpg', image)[1]).decode()


class MyModelRunnable(bentoml.Runnable):
    SUPPORTED_RESOURCES = ("nvidia.com/gpu",)
    SUPPORTS_CPU_MULTI_THREADING = True
    SUPPORTED_FACE_RECOGNITION_MODEL = ['ArcFace']
    SUPPORTED_METRICS = ['cosine', 'euclidean', 'euclidean_l2']
    MIN_THRESHOLD = 0.6

    def __init__(self):
        self.db = sqlite3.connect('face.db', check_same_thread=False)
        self.cursor = self.db.cursor()
        self.reload_df()

    def reload_df(self):
        self.df = pd.read_sql_query("SELECT * FROM face", self.db)

    @bentoml.Runnable.method(batchable=False)
    def face_verify(self, payload: JSON):
        image_1 = base64_to_image(payload["image_1"])
        image_2 = base64_to_image(payload["image_2"])
        model_name = payload["model"]
        metric = payload["metric"]
        detector = payload["detector"]
        
        if model_name not in self.SUPPORTED_FACE_RECOGNITION_MODEL:
            return {"code": 422,
                    "success": 0,
                    "message": "Model not supported",
                    "data": {}
                    }
        if metric not in self.SUPPORTED_METRICS:
            return {"code": 422,
                    "success": 0,
                    "message": "Metric not supported",
                    "data": {}
                    }
        
        
        # faces = DeepFace.represent(image_2, model_name='ArcFace', enforce_detection=False, detector_backend=detector)
        # faces = sorted(faces, key=lambda x: x['facial_area']["w"] * x['facial_area']["h"], reverse=True)
        
        # face = faces[0]
        # x, y, w, h = face["facial_area"]["x"], face["facial_area"]["y"], face["facial_area"]["w"], face["facial_area"]["h"]
        # crop = image_2[y:y+h, x:x+w]
        # real = DeepFace.check_spoofing(crop) == 1
            
        try:
            rs = DeepFace.verify(image_1, 
                                  image_2, 
                                  model_name=model_name,
                                  distance_metric=metric, 
                                  enforce_detection=False)
            res = {
                "code": 200,
                "suscess": 1,
                "message": "Success",
                "data": rs
            }
        except Exception as e:
            res = {"code": 500,
                   "suscess": 0,
                    "message": "Cannot Recognize",
                    "data": {}
                }
        return res

    @bentoml.Runnable.method(batchable=False)
    def face_detect(self, payload: JSON):
        if not payload["image"]:
            return {"Exception": "Invalid input"}
        try:
            image = base64_to_image(payload["image"])
        except Exception as e:
            return {"Exception": str(e)}
        detector = payload["detector"]
        if not detector:
            detector = "opencv"
        response = []
        try:
            res = DeepFace.extract_faces(image, detector_backend=detector)
            for idx, face in enumerate(res):
                facial_area = face["facial_area"]
                confidence = face["confidence"]
                real = face["real"]
                response.append(
                    {"facial_area": facial_area, "confidence": confidence, "real": real, "index": idx})
        except Exception as e:
            response = {"Exception": str(e)}
        return response

    @bentoml.Runnable.method(batchable=False)
    def face_register(self, payload):
        if not payload["name"] or not payload["image"]:
            return {"code": 422,
                    "suscess": 0,
                    "message": "Invalid input", 
                    "data":{}}
        try:
            image = base64_to_image(payload["image"])
        except Exception as e:
            return {"code": 400,
                    "suscess": 0,
                    "message": "Bad request", 
                    "data":{}
                    }
        name = payload["name"]
        detector = payload["detector"]
        if not detector:
            detector = "opencv"
        faces = DeepFace.represent(image, 
                                   model_name='ArcFace', 
                                   enforce_detection=False,
                                   detector_backend=detector)
        faces = sorted(
            faces, key=lambda x: x['facial_area']["w"] * x['facial_area']["h"], reverse=True)
        if faces:
            face = faces[0]
            # x, y, w, h = face["facial_area"]["x"], face["facial_area"]["y"], face["facial_area"]["w"], face["facial_area"]["h"]
            # crop = image[y:y+h, x:x+w]
            # real = DeepFace.check_spoofing(crop) == 1
            
            unique_id = uuid.uuid4()
            cv2.imwrite(f"resources/faces/{unique_id}.jpg", image)
            embedding = face["embedding"]
            embedding = np.array(embedding)
            embedding = embedding.tobytes()
            command = f"INSERT INTO face (uuid, name, path, embedding) VALUES ('{unique_id}', '{name}', 'resources/faces/{unique_id}.jpg', ?)"
            try:
                self.cursor.execute(command, (embedding,))
                self.db.commit()
                self.reload_df()
                return {"code": 200,
                        "suscess": 1,
                        "message": "Success", 
                        "data": {"uuid": unique_id, 
                                 "name": name, 
                                 "path": f"resources/faces/{unique_id}.jpg"
                                }
                        }
            except Exception as e:
                return {"code": 500,
                        "suscess": 0,
                        "message": str(e),
                        "data": {}
                        }
        else:
            return {"code": 500,
                    "suscess": 0,
                    "message": "No Face Detect",
                    "data": {}}

    @bentoml.Runnable.method(batchable=False)
    def face_search(self, payload):
        if not payload["image"]:
            return {"code": 422,
                    "sucess": 0,
                    "message": "Invalid input",
                    "data": {}}
        try:
            image = base64_to_image(payload["image"])
        except Exception as e:
            return {"code": 422,
                    "sucess": 0,
                    "message": str(e),
                    "data": {}}

        detector = payload["detector"]
        if not detector:
            detector = "opencv"
        threshold = payload["threshold"]
        if not threshold:
            threshold = self.MIN_THRESHOLD
        faces = DeepFace.represent(image, model_name='ArcFace', enforce_detection=False, detector_backend=detector)
        faces = sorted(faces, key=lambda x: x['facial_area']["w"] * x['facial_area']["h"], reverse=True)
        if faces:
            face = faces[0]
            x, y, w, h = face["facial_area"]["x"], face["facial_area"]["y"], face["facial_area"]["w"], face["facial_area"]["h"]
            crop = image[y:y+h, x:x+w]
            real = DeepFace.check_spoofing(crop) == 1
            embedding = face["embedding"]
            query_embedding = np.array(embedding)
            if self.df.shape[0] > 0:
                df = self.df.drop(columns=['embedding'])
                df['distance'] = self.df['embedding'].apply(
                    lambda x: dst.findCosineDistance(query_embedding, np.frombuffer(x)))
                df['distance'] = df['distance'].astype(float)
                min_uuid = df.loc[df['distance'].idxmin()]['uuid']
                min_distance = df.loc[df['distance'].idxmin()]['distance']
            else:
                min_uuid = None
                min_distance = 1

            base_64 = ""

            if min_distance < threshold:
                name = self.df.loc[self.df['uuid']
                                   == min_uuid]['name'].values[0]
                path_img = self.df.loc[self.df['uuid']
                                       == min_uuid]['path'].values[0]
                path_img = os.path.join(PREFIX_IMG, path_img)
                base_64 = image_to_base64(cv2.imread(path_img))
            else:
                name = "Unknown"
                min_uuid = "0000000000000"
                
            data = {"name": name, "distance": min_distance, "uuid": min_uuid, "real": real, "image": base_64}
            return {
                "code": 200,
                "sucess": 1,
                "message": "Success",
                "data": data
            }
        else:
            return {
                "code": 500,
                "sucess": 0,
                "message": "No face detect",
                "data": {}
            }


arcface_runner = bentoml.Runner(MyModelRunnable, name="arcface_runner")

svc = bentoml.Service(__name__, runners=[arcface_runner])


@svc.api(input=JSON(), output=JSON())
async def face_verify(payload):
    batch_ret = arcface_runner.face_verify.run(payload)
    return batch_ret


@svc.api(input=JSON(), output=JSON())
async def face_detect(payload):
    batch_ret = arcface_runner.face_detect.run(payload)
    return batch_ret


@svc.api(input=JSON(), output=JSON())
async def face_register(payload):
    batch_ret = arcface_runner.face_register.run(payload)
    return batch_ret


@svc.api(input=JSON(), output=JSON())
async def face_search(payload):
    batch_ret = arcface_runner.face_search.run(payload)
    return batch_ret
