# UniBridge


Unibridge is a program that allows transmissions to be added to trunk player from a unitrunker system.

  - Add EDACS, and Unitrunker supported systems to Trunk player
  

### Required

  - Shared Storage between Trunkplayer System and Unitrunker system
  - Unitrunker v1 or v2
  - Trunking Recorder
  - Visual Studio



### Installation

Unibridge requires nuget packages
- ssh.net 
- Newtonsoft.Json

Next you will need shared storage 
```
Dir tree

[NAS]
 NAS
  |
Radio - Main Radio Trans Dir
  |
 New - Trunking recorder output 
 ```
 ```
 I have Trunking recorer recored to v:\NAS\Radio\NEW
 
 I have Trunkplayer symlinked to V:\NAS/Radio <- From my windows system
```
I have a symlink on my trunkplayer system

``` lrwxrwxrwx 1 root  root     22 Jan  3 14:22 audio_files -> /media/Mount/NAS/Radio ```

I have this mounted as a network disk on windows

Next setup unitrunker and trunking recorder

Next set trunking recorder settings to
![audio](https://maxwelldps.com/trrec.PNG)
![audior](https://maxwelldps.com/trrecaudio.PNG)

### RUNNING
```
EXAMPLE: unitrunker.exe --host 192.168.1.23 --user radio --pass Password1 --trpath V:\NAS\NewAudio\ --tplayerpath V:\NAS\FinalAudioFolder

     --host Trunkplayer SSH HOST OR IP
     --user TrunkPlayer SSH User
     --pass TrunkPlyer SSH Password
     --trpath "V:\NAS\Truning Recorder Output Folder\"
     --tplayerpath "V:\NAS\trunk player symlinked dir\"
     ```
