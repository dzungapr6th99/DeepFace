# CMakeList.txt : CMake project for DetectFace, include source and define
# project specific logic here.
#
# Set the path to OpenCV (modify as necessary)
#set(OpenCV_DIR "C:/Users/Admin/AI/.opencv/opencv/build")  # Path to the OpenCV build directory
#set (OpenCV_LIBS "C:/Users/Admin/AI/.opencv/opencv/build/x64/vc16/lib") # Path to OpenCV lib

set(OpenCV_DIR "/root/AI/opencv-4.7.0/build")  # Path to the OpenCV build directory
set (OpenCV_LIBS "/root/AI/opencv-4.7.0/build/lib") # Path to OpenCV lib

SET (BUILD_SHARED_LIBS  TRUE)
find_package(OpenCV REQUIRED)
# Add source files
set(SOURCES
    detector.cpp
    onet.cpp
    pnet.cpp
    rnet.cpp
)

# Add header files
set(HEADERS
    detector.h
    face.h
    helpers.h
    onet.h
    pnet.h
    rnet.h

)
#add_executable (DetectFace "OpenCVModel.cpp" "OpenCVModel.h" "Common.h" "Common.cpp" )
# Include directories
include_directories(${OpenCV_INCLUDE_DIRS})
add_library(MtCnnModel SHARED  "detector.h" "detector.cpp" "face.h" "helpers.h" "onet.h" "onet.cpp" "pnet.h" "pnet.cpp" "rnet.h" "rnet.cpp")
# Add source to this project's executable.

target_link_libraries(MtCnnModel ${OpenCV_LIBS})

#if (CMAKE_VERSION VERSION_GREATER 3.12)
#  set_property(TARGET DetectFace PROPERTY CXX_STANDARD 20)
#endif()

# TODO: Add tests and install targets if needed.
