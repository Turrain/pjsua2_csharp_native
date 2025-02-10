#!/bin/bash
# build_wrapper.sh
# This script uses PJProject to generate and compile the pjsua2 wrapper library

# Settings (adjust as needed)
PROJ_NAME="PjSua2.Native"
OUT_DIR="${PROJ_NAME}/${PROJ_NAME}/pjsua2"
NAMESPACE="${PROJ_NAME}.pjsua2"
LIBPJSUA2_DIR="${PROJ_NAME}/lib"
LIBPJSUA2_SHARED="${LIBPJSUA2_DIR}/libpjsua2.so"
LIBPJSUA2_SONAME="libpjsua2.so"
PJPROJECT_DIR="pjproject"

# SWIG settings
SWIG_FLAGS="-I${PJPROJECT_DIR}/pjlib/include \
-I${PJPROJECT_DIR}/pjlib-util/include \
-I${PJPROJECT_DIR}/pjmedia/include \
-I${PJPROJECT_DIR}/pjsip/include \
-I${PJPROJECT_DIR}/pjnath/include \
-c++ -w312"

# Source files (for dependency checking)
SRC_DIR="${PJPROJECT_DIR}/pjsip/include"
SRCS="${SRC_DIR}/pjsua2/endpoint.hpp ${SRC_DIR}/pjsua2/types.hpp"

# SWIG interface files
SWIG_INTERFACE="${PJPROJECT_DIR}/pjsip-apps/src/swig/pjsua2.i"
SYMBOLS_INTERFACE="${PJPROJECT_DIR}/pjsip-apps/src/swig/symbols.i"

# Compiler settings
# If you have custom flags or a custom compiler (perhaps from PJProject’s build),
# set them here. Otherwise, use defaults.
PJ_CXX="g++"
MY_CFLAGS="-I${PJPROJECT_DIR}/pjlib/include -I${PJPROJECT_DIR}/pjlib-util/include \
-I${PJPROJECT_DIR}/pjmedia/include -I${PJPROJECT_DIR}/pjsip/include -I${PJPROJECT_DIR}/pjnath/include -fPIC"
MY_LDFLAGS=""

# Create needed directories
mkdir -p "${OUT_DIR}"
mkdir -p "${LIBPJSUA2_DIR}"

# Step 1: Run SWIG to generate the wrapper source
echo "Running SWIG to generate ${OUT_DIR}/pjsua2_wrap.cpp..."
swig ${SWIG_FLAGS} -namespace "${NAMESPACE}" -csharp -o "${OUT_DIR}/pjsua2_wrap.cpp" "${SWIG_INTERFACE}" || { echo "SWIG failed!"; exit 1; }

# Step 2: Compile the generated C++ source file into an object file
echo "Compiling ${OUT_DIR}/pjsua2_wrap.cpp..."
${PJ_CXX} -c "${OUT_DIR}/pjsua2_wrap.cpp" -o "${OUT_DIR}/pjsua2_wrap.o" ${MY_CFLAGS} || { echo "Compilation failed!"; exit 1; }

# Step 3: Link the object file into a shared library
echo "Linking to create shared library ${LIBPJSUA2_SHARED}..."
${PJ_CXX} -shared -fPIC -o "${LIBPJSUA2_SHARED}" "${OUT_DIR}/pjsua2_wrap.o" ${MY_LDFLAGS} -Wl,-soname,"${LIBPJSUA2_SONAME}" || { echo "Linking failed!"; exit 1; }

echo "Wrapper library built successfully."
