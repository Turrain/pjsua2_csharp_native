
#!/bin/bash
# build_pjproject.sh
# This script clones and builds PJProject

# Settings
PJPROJECT_REPO="https://github.com/pjsip/pjproject.git"
PJPROJECT_DIR="pjproject"

# Clone the repo if not already present
if [ ! -d "${PJPROJECT_DIR}" ]; then
    echo "Cloning PJProject from ${PJPROJECT_REPO}..."
    git clone "${PJPROJECT_REPO}" || { echo "Git clone failed!"; exit 1; }
    
    echo "Configuring and building PJProject..."
    cd "${PJPROJECT_DIR}" || exit 1
    # Configure with shared library support and PIC flag
    ./configure --enable-shared CFLAGS="-fPIC" || { echo "Configure failed!"; exit 1; }
    # Build dependencies and then PJProject itself
    make dep || { echo "make dep failed!"; exit 1; }
    make || { echo "make failed!"; exit 1; }
    cd ..
else
    echo "PJProject directory already exists. Skipping clone/build."
fi

