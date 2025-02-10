#!/bin/bash
# build_all.sh
# Master script to build PJProject, then the wrapper library, and finally the .NET solution.

./build_pjproject.sh || { echo "PJProject build failed!"; exit 1; }
./build_wrapper.sh || { echo "Wrapper build failed!"; exit 1; }

echo "All build steps completed successfully."
