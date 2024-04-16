# DeepFace
Hướng dẫn Setup môi trường:

Setup OpenCV:
download bản cài ở đây https://opencv.org/releases/
Clone project opencv trên git về: https://github.com/opencv/opencv.git
Setup bản cài sao cho thư mục cài chứa dll cùng đường dẫn với thư mục git

Setup libtorch:
download bản cài ở: https://pytorch.org/ bản cài này có sẵn cả file lẫn code

Setup tensorflow:
làm tương tự như tensorflow, tải bản cài ở https://storage.googleapis.com/tensorflow/libtensorflow/libtensorflow-gpu-windows-x86_64-2.10.0.zip
và clone code trên git về: https://github.com/tensorflow/tensorflow.git

OnnxRuntime: Do Microsoft phát triển nên chỉ cần nuget về
Nếu chạy trên gpu thì làm theo bước bên dưới
Setup Cuda Toolkit và CuDnn để chạy trên gpu.
Nếu Visual studio Nsight nó có recommend thì cài  nó luôn
Làm theo hướng dẫn trên trang https://onnxruntime.ai/docs/tutorials/csharp/csharp-gpu.html


----------------------------------------------------------------------------------------------

Convert onnx:
Chạy code DeepFacePyFaceDetect/testbuildonnx.py để build các file model trong deepface thành onnx
tương ứng với version onnx runtime, cài đặt Cuda, cudnn, tensorRT theo requirement sau:
https://onnxruntime.ai/docs/execution-providers/CUDA-ExecutionProvider.html

Build thư viện wrapper OpenCV:
build project DetectorDll ra dll và cop vào project PreProcess là có thể run được(lưu ý setup lại đường dẫn opencv cho project DetectorDll)

