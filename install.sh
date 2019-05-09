#!/bin/bash

if [ $# -eq 0 ]; then
    echo "USAGE: (sudo) ./install.sh ATOMPM_DIR"
    exit 1
fi


ATOMPM_DIR="$1"

if [ -d "${ATOMPM_DIR}" ]; then
    echo cp -r railway_wd ${ATOMPM_DIR}/
    cp -r railway_wd ${ATOMPM_DIR}/

    for USER in ${ATOMPM_DIR}/users/*; do
        echo cp -r Railway ${ATOMPM_DIR}/users/$(basename ${USER})/Formalisms/
        cp -r Railway ${ATOMPM_DIR}/users/$(basename ${USER})/Formalisms/
        echo cp -r TrainSchedule ${ATOMPM_DIR}/users/$(basename ${USER})/Formalisms/
        cp -r TrainSchedule ${ATOMPM_DIR}/users/$(basename ${USER})/Formalisms/
    done
else
    echo "Cannot find AToMPM home directory"
    exit 1
fi
