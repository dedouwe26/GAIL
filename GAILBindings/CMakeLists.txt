cmake_minimum_required(VERSION 3.0.0)
project(GAILBindings VERSION 0.1.0 LANGUAGES C CXX)

include(CTest)
enable_testing()

set(HEADER_FILES lib/glad/glad.h lib/glad/khrplatform.h lib/glfw/glfw3.h lib/glfw/glfw3native.h)

add_library(libs lib/glad/lib/glad.c lib/glfw/lib/glfw3_mt.lib ${HEADER_FILES})
target_include_directories(libs PRIVATE .)
add_executable(exec GAILBindings.cpp Mesh.cpp Mesh.hpp ${HEADER_FILES})
target_include_directories(exec PRIVATE .)

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
