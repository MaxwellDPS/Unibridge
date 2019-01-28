#!/bin/bash
echo "Encoding: $1"
filename="/mnt/radio/$1.mp3"
basename="${filename%.*}"
filename_only=$(basename $basename)
json="$basename.json"
AACencoded="$basename.m4a"                                                                #NEW
system=0 # Change this for each system

~/ffmpeg/ffmpeg -i $filename -c:a libfdk_aac -profile:a aac_he -b:a 32k -ar 48k $AACencoded

cd /home/radio/trunk-player; . env/bin/activate; ./manage.py add_transmission $basename  --system=$system --m4a
rm -f $filename
