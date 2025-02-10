# Use Ubuntu 24.04 as the base image
FROM ubuntu:24.04

# Disable interactive prompts during build
ENV DEBIAN_FRONTEND=noninteractive

# Update packages and install required tools
RUN apt-get update && apt-get install -y \
    wget \
    ca-certificates \
    apt-transport-https \
    gnupg \
    build-essential \
    make \
    software-properties-common \
    cmake \
    git \ 
    swig
RUN apt-get install libopencore-amrnb-dev -y \
    libopencore-amrwb-dev \
    libasio-dev \
    libyuv-dev \
    libssl-dev \
    libasound2-dev \
    libsdl2-dev \
    libv4l-dev \
    libpulse-dev \
    libsrtp2-dev \
    libcurl4-openssl-dev \
    libunbound-dev \
    uuid-dev 
RUN add-apt-repository ppa:dotnet/backports
RUN apt-get update && \
  apt-get install -y dotnet-sdk-9.0
RUN  apt-get install -y dotnet-runtime-9.0
RUN rm -rf /var/lib/apt/lists/*
WORKDIR /workspace

