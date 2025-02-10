#!/bin/bash

sudo apt-get update && sudo apt-get install -y \
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
sudo apt-get install libopencore-amrnb-dev -y \
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
sudo add-apt-repository ppa:dotnet/backports
sudo apt-get update && \
  apt-get install -y dotnet-sdk-9.0
sudo  apt-get install -y dotnet-runtime-9.0