Imports System.Threading
Imports System.IO
Imports Newtonsoft.Json
Imports Renci.SshNet

Module Module1
    'Change --------------------------------------
    Public host As String = "Tplayer.net.local"
    Public username As String = "radio"
    Public pass As String = "pass"
    Public TRPATH = "V:\NAS\Truning Recorder Output Folder\"
    Public TPLAYERPATH = "V:\NAS\trunk player symlinked dir\"
    '---------------------------------------------

    Public watchfolder As FileSystemWatcher

    Public Class Files
        Public name As String
        Public path As String
        Public outpath As String
    End Class

    Sub Main()

        Dim clArgs() As String = Environment.GetCommandLineArgs()
        Dim count = 0

        If clArgs.Count = 11 Then
            host = clArgs(2)
            username = clArgs(4)
            pass = clArgs(6)
            TRPATH = clArgs(8)
            TPLAYERPATH = clArgs(10)
            WatchFolders(TRPATH)
            Console.WriteLine("STARTED")
            While True

            End While
        Else
            Console.WriteLine("UniPlayer HELP" & vbCrLf)
            Console.WriteLine("EXAMPLE: unitrunker.exe --host 192.168.1.23 --user radio --pass Password1 --trpath V:\NAS\NewAudio\ --tplayerpath V:\NAS\FinalAudioFolder" & vbCrLf)
            Console.WriteLine("     --host Trunkplayer SSH HOST OR IP")
            Console.WriteLine("     --user TrunkPlayer SSH User")
            Console.WriteLine("     --pass TrunkPlyer SSH Password")
            Console.WriteLine("     --trpath V:\NAS\Truning Recorder Output Folder\")
            Console.WriteLine("     --tplayerpath V:\NAS\trunk player symlinked dir\" & vbCrLf)
        End If

    End Sub

    Sub WatchFolders(folder As String)
        watchfolder = New FileSystemWatcher With {
            .Path = folder,
            .NotifyFilter = NotifyFilters.DirectoryName
        }
        watchfolder.NotifyFilter = watchfolder.NotifyFilter Or
                                   NotifyFilters.FileName
        watchfolder.NotifyFilter = watchfolder.NotifyFilter Or
                                   NotifyFilters.Attributes
        AddHandler watchfolder.Created, AddressOf logchange
        watchfolder.EnableRaisingEvents = True
        Console.WriteLine("Watching folder")
    End Sub

    Private Sub logchange(ByVal source As Object, ByVal e As FileSystemEventArgs)
        If e.ChangeType = IO.WatcherChangeTypes.Created Then
            If Not e.Name.EndsWith(".srt") Then
                Dim newfile As New Files With {
                    .name = e.Name,
                    .path = e.FullPath,
                    .outpath = TPLAYERPATH
                }
                Console.WriteLine("NEW FILE: " + e.Name)
                Transmission(newfile)
            End If
        End If
    End Sub

    Sub Transmission(file As Files)
        Dim TransmissionThread = New Thread(AddressOf DoWork)
        TransmissionThread.Start(file)
    End Sub
    Public Sub DoWork(ByVal data As Object)
        Dim parameters = CType(data, Files)
        Dim myFile As New FileInfo(parameters.path)
        Dim sizeInBytes As Long = myFile.Length
        ' Console.WriteLine("NEW THREAD Created")
        While True
            Try
                myFile.Refresh()
                sizeInBytes = myFile.Length
                If sizeInBytes = 0 Then
                    Thread.Sleep(30000)
                Else
                    finish(parameters)
                    Exit While

                End If
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try
        End While


    End Sub
    Public Sub finish(parameters As Files)
        Try
            File.Move(parameters.path, parameters.outpath + "\" + parameters.name)
            File.Move(parameters.path + ".srt", parameters.outpath + "\" + parameters.name + ".srt")
            Console.WriteLine("Copied Files")
        Catch
        End Try
        Class1.Main2(parameters.outpath + "\" + parameters.name, parameters.outpath)
        Console.WriteLine("Passed to LIB")

    End Sub

    Public Class Class1

        Public Class Unit
            Public StartTime As TimeSpan
            Public endTime As TimeSpan
            Public initalTime As Int32
            Public Source As Int32
        End Class
        Public Class Source
            Public src As Int32
            Public time As Int32
            Public pos As Int32
        End Class

        Public Class freqs
            Public freq As Double
            Public time As Double
            Public pos As Double
            Public len As Double
            Public error_count As Double
            Public spike_count As Double
        End Class

        Public Class Transmission
            Public freq As String
            Public start_time As Int32
            Public stop_time As Int32
            Public emergency As Int32
            Public talkgroup As Int32
            Public srcList As New List(Of Source)
            Public play_length As Int32
            Public source As Int32
            Public analog As Int32
            Public freqList As New List(Of freqs)
        End Class

        Public Shared Sub Main2(fpath As String, tppath As String)
            Dim srtname As String = fpath + ".srt"
            Dim audiofile As String = fpath
            Dim audiofilepath As String = Path.GetDirectoryName(audiofile)
            Dim result As String = Handle(srtname)
            Dim correctName As String = CorrectFile(srtname)
            Dim jsonName As String = audiofilepath + "\" + correctName + ".json"
            Dim mp3Name As String = correctName + ".mp3"

            Console.WriteLine("NEW THREAD INITED")
            Try
                File.WriteAllText(jsonName, result)
                My.Computer.FileSystem.DeleteFile(srtname)
                My.Computer.FileSystem.RenameFile(audiofile, mp3Name)
                SSH(correctName)
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try

        End Sub

        Public Shared Function Handle(filename As String)
            Dim value As String = Path.GetFileName(filename)
            Dim line As String
            value = value.Substring(0, value.Length - 8)
            Try
                line = File.ReadAllText(filename)
                Return SrtToJSON(line, value)
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try
        End Function

        Public Shared Function CorrectFile(filename As String)
            Dim value As String = Path.GetFileName(filename)
            value = value.Substring(0, value.Length - 8)
            Dim sections() As String
            Dim out As String
            If value.Contains(",") Then
                Dim parts() As String = value.Split(",")
                value = parts(parts.Count - 1).Trim

            End If
            sections = value.Split("_")

            out = sections(0) + "_" + Format(Convert.ToDouble(sections(1)), "e")

            Return out
        End Function

        Public Shared Function SrtToJSON(input As String, filename As String)
            Dim trans As New Transmission
            Dim freqss As New freqs
            Dim Units As New List(Of Unit)
            Dim strText() As String
            Dim numUnits As Int32 = 0
            If filename.Contains(",") Then
                Dim parts() As String = filename.Split(",")
                filename = parts(parts.Count - 1).Trim

            End If
            Dim val As Double = Convert.ToDouble(filename.Split("-")(1).Split("_")(1)) * 1000000

            strText = Split(input, vbCrLf)

            For Each line In strText
                If line.StartsWith("S") Then
                ElseIf line.Contains(":") Then
                ElseIf line.Trim <> "" Then
                    numUnits = numUnits + 1
                End If
            Next

            For x = 1 To numUnits Step 1
                Dim base = ((x - 1) * 5)
                Dim unit As New Unit
                Dim time As String
                Dim today As String
                Dim oDate As DateTime
                Dim str() As String

                If strText(base + 2).Contains("PM") Then
                    time = strText(base + 2).Substring(0, strText(base + 2).Length - 2)
                    time = time + " PM"
                Else
                    time = strText(base + 2).Substring(0, strText(base + 2).Length - 2)
                    time = time + " AM"
                End If

                today = DateTime.Today.Year.ToString + "-" + DateTime.Today.Month.ToString + "-" + DateTime.Today.Day.ToString + " " + time
                oDate = DateTime.Parse(today)
                unit.initalTime = Convert.ToInt32(TimeToUnix(oDate))


                str = strText(base + 1).Split(">")
                str(0) = str(0).Substring(0, str(0).Length - 3)
                str(1) = str(1).TrimStart(" ")
                str(0) = str(0).Split(",")(0)
                str(1) = str(1).Split(",")(0)

                unit.StartTime = New TimeSpan(str(0).Split(":")(0), str(0).Split(":")(1), str(0).Split(":")(2))
                unit.endTime = New TimeSpan(str(1).Split(":")(0), str(1).Split(":")(1), str(1).Split(":")(2))
                If strText(base + 3).Split(":")(1).Split("-")(0).Trim.Length > 0 Then
                    unit.Source = Convert.ToInt32(strText(base + 3).Split(":")(1).Split("-")(0))
                Else
                    unit.Source = 0
                End If

                Units.Add(unit)
            Next

            For Each unit In Units
                Dim src As New Source With {
                .src = unit.Source,
                .time = unit.initalTime,
                .pos = unit.StartTime.TotalSeconds
            }
                trans.srcList.Add(src)
            Next


            trans.freq = Format(val, "e")
            trans.analog = 1
            trans.emergency = 0
            trans.talkgroup = Convert.ToInt32(filename.Split("-")(0))
            trans.play_length = Units.Last.endTime.TotalSeconds
            trans.source = numUnits
            trans.start_time = Units.First.initalTime
            trans.stop_time = Units.Last.initalTime + Units.Last.endTime.TotalSeconds
            freqss.freq = val
            freqss.time = Units.First.initalTime
            freqss.pos = 0.0
            freqss.len = 0.0
            freqss.error_count = 0.0
            freqss.spike_count = 0.0
            trans.freqList.Add(freqss)

            Return JsonConvert.SerializeObject(trans, Formatting.Indented)

        End Function

        Public Shared Sub SSH(file As String)
            Dim ConnectionInfo As New ConnectionInfo(host, username, New PasswordAuthenticationMethod(username, pass))

            Using ssh As New SshClient(ConnectionInfo)
                ssh.Connect()
                Dim command = ssh.CreateCommand("~/Load.sh " + file)
                Dim result = command.Execute()
                Console.WriteLine(result)
                ssh.Disconnect()
            End Using
            Console.WriteLine("SSH " + file + " COMPLETE")
        End Sub

        Public Shared Function TimeToUnix(ByVal dteDate As Date) As String
            If dteDate.IsDaylightSavingTime = True Then
                dteDate = DateAdd(DateInterval.Hour, -1, dteDate)
            End If
            TimeToUnix = DateDiff(DateInterval.Second, #1/1/1970#, dteDate)
        End Function
    End Class


End Module
