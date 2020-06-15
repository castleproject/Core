#!/bin/bash
set -e
MONO_TAG=${MONO_TAG:-6.0.0.334}
docker run --rm -v "$PWD":'/project' -w='/project' mono:$MONO_TAG mono "$@"
