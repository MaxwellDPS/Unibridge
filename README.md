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

Next setup unitrunker and trunking recorder

Next set trunking recorder settings to
![audio](https://maxwelldps.com/trrec.PNG)
![audior](https://maxwelldps.com/trrecaudio.PNG)

then in Module1.vb change the settings on the top to match you Trunking recorder output dir amd the dir that trunk player is looking for files. then set your tplayer box ssh settings and run!

```
    'Change --------------------------------------
    Public host As String = "Tplayer.net.local"
    Public username As String = "radio"
    Public pass As String = "pass"
    Public TRPATH = "V:\NAS\Truning Recorder Output Folder\"
    Public TPLAYERPATH = "V:\NAS\trunk player symlinked dir\"
    '---------------------------------------------
    ```
