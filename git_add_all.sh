#!/bin/bash
sudo git rm -rf Railway
sudo git rm -rf TrainSchedule
sudo cp -r ../atompm/users/admin/Formalisms/Railway ./
sudo cp -r ../atompm/users/admin/Formalisms/TrainSchedule ./
sudo git add *
sudo git status