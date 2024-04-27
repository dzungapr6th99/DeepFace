# CMakeList.txt : CMake project for DetectFace, include source and define
# project specific logic here.
#
# Set the path to OpenCV (modify as necessary)
set(OpenCV_DIR "C:/Users/Admin/AI/.opencv/opencv/build")  # Path to the OpenCV build directory
set (OpenCV_LIBS "C:/Users/Admin/AI/.opencv/opencv/build/x64/vc16/lib") # Path to OpenCV lib
SET (BUILD_SHARED_LIBS  TRUE)
find_package(OpenCV REQUIRED)

add_executable (DetectFace "OpenCVModel.cpp" "OpenCVModel.h" "Common.h" "Common.cpp" )
# Include directories
include_directories(${OpenCV_INCLUDE_DIRS})
add_library(DetectFace SHARED ${SOURCES})
# Add source to this project's executable.

target_link_libraries(DetectFace ${OpenCV_LIBS})

#if (CMAKE_VERSION VERSION_GREATER 3.12)
#  set_property(TARGET DetectFace PROPERTY CXX_STANDARD 20)
#endif()

# TODO: Add tests and install targets if needed.
