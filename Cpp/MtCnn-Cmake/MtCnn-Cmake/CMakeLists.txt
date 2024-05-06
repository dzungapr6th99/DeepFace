# CMakeList.txt : CMake project for MtCnn-Cmake, include source and define
# project specific logic here.
#

# Add source to this project's executable.
add_executable (MtCnn-Cmake "MtCnn-Cmake.cpp" "MtCnn-Cmake.h")

if (CMAKE_VERSION VERSION_GREATER 3.12)
  set_property(TARGET MtCnn-Cmake PROPERTY CXX_STANDARD 20)
endif()

# TODO: Add tests and install targets if needed.
