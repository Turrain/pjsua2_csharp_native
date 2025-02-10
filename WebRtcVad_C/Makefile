# Compiler settings
CC = gcc
CFLAGS = -Wall -Wextra -O2 -fPIC -fvisibility=default
INCLUDES = -I. \
          -I./common_audio/vad/include \
          -I./common_audio/signal_processing/include \
          -I./rtc_base

# Source files
VAD_SRCS = common_audio/vad/vad_core.c \
           common_audio/vad/vad_filterbank.c \
           common_audio/vad/vad_gmm.c \
           common_audio/vad/vad_sp.c \
           common_audio/vad/webrtc_vad.c

SP_SRCS = common_audio/signal_processing/complex_bit_reverse.c \
          common_audio/signal_processing/complex_fft.c \
          common_audio/signal_processing/cross_correlation.c \
          common_audio/signal_processing/division_operations.c \
          common_audio/signal_processing/downsample_fast.c \
          common_audio/signal_processing/energy.c \
          common_audio/signal_processing/get_scaling_square.c \
          common_audio/signal_processing/min_max_operations.c \
          common_audio/signal_processing/resample_48khz.c \
          common_audio/signal_processing/resample_by_2_internal.c \
          common_audio/signal_processing/resample_fractional.c \
          common_audio/signal_processing/spl_init.c \
          common_audio/signal_processing/spl_inl.c \
          common_audio/signal_processing/spl_sqrt.c \
          common_audio/signal_processing/vector_scaling_operations.c \
          common_audio/signal_processing/dot_product_with_scale.cc

THIRD_PARTY_SRCS = common_audio/third_party/spl_sqrt_floor/spl_sqrt_floor.c

RTC_BASE_SRCS = rtc_base/checks.cc

WRAPPER_SRCS = vad_wrapper.c

SRCS = $(VAD_SRCS) $(SP_SRCS) $(THIRD_PARTY_SRCS) $(RTC_BASE_SRCS) $(WRAPPER_SRCS)
OBJS = $(SRCS:.c=.o)
OBJS := $(OBJS:.cc=.o)

# Library names
STATIC_LIB = libwebrtcvad.a
SHARED_LIB = libwebrtcvad.so

# Default target
all: $(STATIC_LIB) $(SHARED_LIB)

# Pattern rules for object files
%.o: %.c
	$(CC) $(CFLAGS) $(INCLUDES) -c $< -o $@

%.o: %.cc
	$(CC) $(CFLAGS) $(INCLUDES) -c $< -o $@

# Build static library
$(STATIC_LIB): $(OBJS)
	ar rcs $@ $(OBJS)

# Build shared library
$(SHARED_LIB): $(OBJS)
	$(CC) -shared -o $@ $(OBJS)

# Clean target
clean:
	rm -f $(OBJS) $(STATIC_LIB) $(SHARED_LIB)

# Install target (optional)
install: all
	install -d $(DESTDIR)/usr/local/lib
	install -m 644 $(STATIC_LIB) $(DESTDIR)/usr/local/lib
	install -m 755 $(SHARED_LIB) $(DESTDIR)/usr/local/lib
	install -d $(DESTDIR)/usr/local/include
	install -m 644 webrtcvad.h $(DESTDIR)/usr/local/include

.PHONY: all clean install
