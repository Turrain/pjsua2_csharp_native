# Clone PjProject repository
PJPROJECT_REPO = https://github.com/pjsip/pjproject.git
PJPROJECT_DIR = pjproject

# Project settings
PROJ_NAME = PjSua2.Native
OUT_DIR = $(PROJ_NAME)/$(PROJ_NAME)/pjsua2
NAMESPACE = $(PROJ_NAME).pjsua2
LIBPJSUA2_DIR = $(PROJ_NAME)/lib
LIBPJSUA2 = $(LIBPJSUA2_DIR)/libpjsua2.a
LIBPJSUA2_SHARED := $(LIBPJSUA2_DIR)/libpjsua2.so
LIBPJSUA2_SONAME := libpjsua2.so
# SWIG settings
SWIG_FLAGS = -I$(PJPROJECT_DIR)/pjlib/include \
             -I$(PJPROJECT_DIR)/pjlib-util/include \
             -I$(PJPROJECT_DIR)/pjmedia/include \
             -I$(PJPROJECT_DIR)/pjsip/include \
             -I$(PJPROJECT_DIR)/pjnath/include \
             -c++ -w312

# Source directory and files
SRC_DIR = $(PJPROJECT_DIR)/pjsip/include
SRCS = $(SRC_DIR)/pjsua2/endpoint.hpp $(SRC_DIR)/pjsua2/types.hpp

# SWIG Interface files
SWIG_INTERFACE = $(PJPROJECT_DIR)/pjsip-apps/src/swig/pjsua2.i
SYMBOLS_INTERFACE = $(PJPROJECT_DIR)/pjsip-apps/src/swig/symbols.i

# Build flags placeholder
MY_CFLAGS =
MY_LDFLAGS =

# Include custom build steps if available
-include custom_build.mak

.PHONY: all clean install uninstall build-pjproject build-wrapper build-sample dotnet-build

all: build-pjproject build-wrapper build-sample

# Clone and build PjProject
build-pjproject:
	@if [ ! -d "$(PJPROJECT_DIR)" ]; then \
		echo "Cloning PjProject..."; \
		git clone $(PJPROJECT_REPO); \
		cd $(PJPROJECT_DIR) && ./configure --enable-shared CFLAGS="-fPIC"  && make dep && make; \
	fi

# Include PJSIP build settings
include $(PJPROJECT_DIR)/build.mak
include $(PJPROJECT_DIR)/build/common.mak

# Update build flags after including makefiles
MY_CFLAGS = $(PJ_CXXFLAGS) $(CFLAGS)
MY_LDFLAGS = $(PJ_LDXXFLAGS) $(PJ_LDXXLIBS) $(LDFLAGS)

# Build the static library
build-wrapper: $(LIBPJSUA2_SHARED)

$(LIBPJSUA2_SHARED): $(OUT_DIR)/pjsua2_wrap.o
	@mkdir -p $(LIBPJSUA2_DIR)
	$(PJ_CXX) -shared -fPIC -o $(LIBPJSUA2_SHARED) $(OUT_DIR)/pjsua2_wrap.o $(PJ_LIBXX_FILES) $(MY_LDFLAGS) -Wl,-soname,$(LIBPJSUA2_SONAME)

$(OUT_DIR)/pjsua2_wrap.o: $(OUT_DIR)/pjsua2_wrap.cpp
	$(PJ_CXX) -c $(OUT_DIR)/pjsua2_wrap.cpp -o $(OUT_DIR)/pjsua2_wrap.o $(MY_CFLAGS)

$(OUT_DIR)/pjsua2_wrap.cpp: $(SWIG_INTERFACE) $(SYMBOLS_INTERFACE) Makefile $(SRCS)
	@mkdir -p $(OUT_DIR)
	swig $(SWIG_FLAGS) -namespace $(NAMESPACE) -csharp -o $(OUT_DIR)/pjsua2_wrap.cpp $(SWIG_INTERFACE)

# Clean build artifacts
clean:
	@rm -rf $(OUT_DIR)/*
	@rm -rf $(LIBPJSUA2_DIR)/*
	@rm -rf $(PJPROJECT_DIR)

# Installation steps (if any)
install:
	@echo "Installation not required for .NET build"

# Uninstallation steps (if any)
uninstall:
	@echo "Uninstallation not required for .NET build"

# .NET solution build
dotnet-build:
	@echo "Building .NET solution..."
	cd $(PROJ_NAME) && dotnet build
