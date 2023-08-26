FROM python:3.8-slim-buster

RUN apt-get update && apt-get install -y libsm6 libxext6 libxrender-dev wget

WORKDIR /app

COPY requirements.txt .
RUN --mount=type=cache,target=/root/.cache/pip pip install -r requirements.txt
RUN pip install torch torchvision --index-url https://download.pytorch.org/whl/cpu
RUN pip install opencv-python-headless --force-reinstall

COPY . /app/face_service

WORKDIR /app/face_service
RUN mkdir /root/.deepface && mkdir /root/.deepface/weights
RUN wget -P /root/.deepface/weights https://github.com/serengil/deepface_models/releases/download/v1.0/arcface_weights.h5 

CMD ["bentoml", "serve", "service.py:svc", "--port", "50001"]
