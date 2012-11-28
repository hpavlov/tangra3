#!/bin/sh
export DYLD_LIBRARY_PATH=/usr/lib/Tangra3:$DYLD_LIBRARY_PATH
export LD_LIBRARY_PATH=/usr/lib/Tangra3:$LD_LIBRARY_PATH
exec /usr/bin/mono /usr/lib/Tangra3/Tangra.exe "$@"