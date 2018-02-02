# UniBridge


Unibridge is a program that allows transmissions to be added to trunk player from a unitrunker system.

  - Add EDACS, and Unitrunken supported systems to Trunk player
  

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

I have a symlink on my trunkplayer system
``` lrwxrwxrwx 1 root  root     22 Jan  3 14:22 audio_files -> /media/Mount/NAS/Radio ```

I have this mounted as a network disk on windows
