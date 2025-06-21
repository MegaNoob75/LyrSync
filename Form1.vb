Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Newtonsoft.Json.Linq

Imports TagLib

Public Class Form1
    ' Holds the selected folder path
    Private selectedFolder As String = ""
    ' Handles task cancellation
    Private cancelTokenSource As CancellationTokenSource
    Private cancelToken As CancellationToken
    ' Indicates whether the tagging process is running
    Private taggingRunning As Boolean = False
    ' Total number of files to process
    Private totalFiles As Integer = 0
    ' Count of completed file processing
    Private completedFiles As Integer = 0
    ' Count of failed file processing
    Private failedFiles As Integer = 0
    ' Lock object for thread safety
    Private lockObj As New Object()
    ' Controls concurrency
    Private semaphore As SemaphoreSlim
    ' Tracks sorting column for ListView
    Private sortColumn As Integer = -1
    Private sortOrder As SortOrder = SortOrder.Ascending
    'estimate time and elapsed time
    Private processStartTime As DateTime
    Private elapsedTimer As System.Windows.Forms.Timer
    Private failedFilePaths As New List(Of String)

    Private Sub EnableDoubleBuffering(lv As ListView)
        Dim prop = GetType(Control).GetProperty("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
        If prop IsNot Nothing Then
            prop.SetValue(lv, True, Nothing)
        End If
    End Sub

    ' Opens a folder dialog to select a folder containing FLAC files
    Private Sub btnSelectFolder_Click(sender As Object, e As EventArgs) Handles btnSelectFolder.Click
        Using fbd As New FolderBrowserDialog()
            If fbd.ShowDialog() = DialogResult.OK Then
                selectedFolder = fbd.SelectedPath
                txtFolder.Text = selectedFolder

                Dim audioFiles = Directory.GetFiles(selectedFolder, "*.*", SearchOption.AllDirectories)
                Dim validFiles = audioFiles.Where(Function(f) f.EndsWith(".flac", StringComparison.OrdinalIgnoreCase) OrElse
                                                f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)).ToArray()

                lvFileResults.Items.Clear()
                For Each file In validFiles
                    Dim item As New ListViewItem(Path.GetFileName(file))
                    item.SubItems.Add("Not processed")
                    item.ForeColor = Color.White
                    lvFileResults.Items.Add(item)
                Next

                lblProgress.Text = $"0 processed / {validFiles.Length} files to scan"
                failedFiles = 0
                UpdateFailedLabel()
            End If
        End Using
    End Sub


    ' Starts or cancels the tagging process
    Private Async Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        If taggingRunning Then
            cancelTokenSource?.Cancel()
            UpdateLog("Cancellation requested...")
            btnStart.Enabled = False
            Return
        End If

        If String.IsNullOrEmpty(selectedFolder) OrElse Not Directory.Exists(selectedFolder) Then
            MessageBox.Show("Please select a valid folder.")
            Return
        End If

        cancelTokenSource = New CancellationTokenSource()
        cancelToken = cancelTokenSource.Token
        taggingRunning = True
        btnStart.Text = "Cancel"
        btnStart.Enabled = True

        processStartTime = DateTime.Now

        ' Setup and start the timer to update elapsed/estimated time every second
        elapsedTimer = New System.Windows.Forms.Timer()
        elapsedTimer.Interval = 1000 ' 1 second
        AddHandler elapsedTimer.Tick, AddressOf ElapsedTimer_Tick
        elapsedTimer.Start()

        ProgressBar.Value = 0
        lstLog.Items.Clear()
        lvFileResults.Items.Clear()
        lstLog.Items.Add("Starting tagging...")

        completedFiles = 0
        failedFiles = 0
        UpdateFailedLabel()

        ' Set maximum concurrency based on user input
        semaphore = New SemaphoreSlim(CInt(numMaxConcurrent.Value))
        'clear faild files from list
        failedFilePaths.Clear()

        ' Start processing all files asynchronously
        Await ProcessAllFiles(selectedFolder, cancelToken)

        ' Retry failed files once if enabled
        If chkRetryFailed.Checked AndAlso failedFilePaths.Count > 0 Then
            UpdateLog("Retrying failed files...")
            Dim retryTasks As New List(Of Task)
            Dim retrySemaphore = New SemaphoreSlim(CInt(numMaxConcurrent.Value))

            Dim retryPaths = failedFilePaths.ToList() ' Clone to avoid modification issues
            failedFilePaths.Clear() ' Reset before retry

            For Each path In retryPaths
                If cancelToken.IsCancellationRequested Then Exit For
                Dim localPath = path
                retryTasks.Add(Task.Run(Async Function()
                                            Await retrySemaphore.WaitAsync()
                                            Try
                                                ProcessFile(localPath, cancelToken)
                                            Finally
                                                retrySemaphore.Release()
                                            End Try
                                        End Function))
            Next

            Await Task.WhenAll(retryTasks)
        End If

        taggingRunning = False
        btnStart.Text = "Start Tagging"
        btnStart.Enabled = True

        elapsedTimer.Stop()
        elapsedTimer.Dispose()

        lstLog.Items.Add("Finished.")
        lstLog.TopIndex = lstLog.Items.Count - 1
    End Sub


    Private Sub ElapsedTimer_Tick(sender As Object, e As EventArgs)
        Dim elapsed = DateTime.Now - processStartTime

        Dim averageTimePerFile As TimeSpan
        Dim estimatedRemaining As TimeSpan

        If completedFiles > 0 Then
            averageTimePerFile = TimeSpan.FromTicks(elapsed.Ticks \ completedFiles)
            Dim filesLeft = totalFiles - completedFiles
            estimatedRemaining = TimeSpan.FromTicks(averageTimePerFile.Ticks * filesLeft)
        Else
            averageTimePerFile = TimeSpan.Zero
            estimatedRemaining = TimeSpan.Zero
        End If

        Dim elapsedStr = $"{elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}"
        Dim estimatedStr = If(estimatedRemaining.TotalSeconds > 0,
                          $"{estimatedRemaining.Hours:D2}:{estimatedRemaining.Minutes:D2}:{estimatedRemaining.Seconds:D2}",
                          "Calculating...")

        If lblTimeInfo.InvokeRequired Then
            lblTimeInfo.Invoke(Sub()
                                   lblTimeInfo.Text = $"Elapsed Time: {elapsedStr} | Estimated Remaining: {estimatedStr}"
                               End Sub)
        Else
            lblTimeInfo.Text = $"Elapsed Time: {elapsedStr} | Estimated Remaining: {estimatedStr}"
        End If
    End Sub

    ' Runs tagging on all .flac files concurrently using a semaphore
    Async Function ProcessAllFiles(folderPath As String, token As CancellationToken) As Task
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim flacFiles = Directory.GetFiles(folderPath, "*.flac", SearchOption.AllDirectories)
        totalFiles = flacFiles.Length

        Invoke(Sub()
                   lblProgress.Text = $"0 processed / {totalFiles} files to scan"
                   ProgressBar.Maximum = totalFiles
                   ProgressBar.Value = 0
               End Sub)

        UpdateProgress()

        If totalFiles = 0 Then
            UpdateLog("No FLAC files found.")
            Return
        End If

        Dim tasks As New List(Of Task)
        For Each filePath In flacFiles
            If token.IsCancellationRequested Then Exit For
            Dim localPath = filePath
            tasks.Add(Task.Run(Async Function()
                                   Await semaphore.WaitAsync()
                                   Try
                                       ProcessFile(localPath, token)
                                   Finally
                                       semaphore.Release()
                                   End Try
                               End Function))
        Next
        Await Task.WhenAll(tasks)
    End Function


    ' Processes a single FLAC file, queries the LRClib API, and writes lyrics to the tag
    Sub ProcessFile(filePath As String, token As CancellationToken)
        If token.IsCancellationRequested Then Return
        Try
            Dim tfile = TagLib.File.Create(filePath)
            Dim rawArtist = tfile.Tag.FirstPerformer
            Dim rawTitle = tfile.Tag.Title

            ' Check if lyrics already exist and look like synced lyrics (contain timestamps)
            If chkSkipIfSynced.Checked AndAlso Not String.IsNullOrWhiteSpace(tfile.Tag.Lyrics) Then
                Dim existingLyrics = tfile.Tag.Lyrics
                ' Simple check: look for [mm:ss.xx] pattern to detect synced lyrics
                If Regex.IsMatch(existingLyrics, "\[\d{1,2}:\d{2}(\.\d{1,2})?\]") Then
                    LogSuccess(Path.GetFileName(filePath), "Skipped (Already has synced lyrics)")
                    UpdateLog($"{Path.GetFileName(filePath)}: Skipped - already contains synced lyrics.")
                    Return
                End If
            End If

            If String.IsNullOrWhiteSpace(rawArtist) OrElse String.IsNullOrWhiteSpace(rawTitle) Then
                Dim msg = "Missing artist or title tag"
                LogFailure(filePath, msg)
                UpdateLog($"{Path.GetFileName(filePath)}: {msg}")
                Return
            End If

            Dim artist = CleanTag(rawArtist)
            Dim title = CleanTag(rawTitle)
            Dim localDuration = CInt(tfile.Properties.Duration.TotalSeconds)
            Dim url = $"https://lrclib.net/api/search?track_name={Uri.EscapeDataString(title)}&artist_name={Uri.EscapeDataString(artist)}"
            'Thread.Sleep(500) ' ← This slows down LRClib
            UpdateLog($"Querying: {artist} - {title}")

            Dim jsonText As String = ""
            Using client As New WebClient()
                jsonText = client.DownloadString(url)
            End Using

            Dim jsonArray = JArray.Parse(jsonText)
            If jsonArray.Count = 0 Then
                Dim msg = "No results from API"
                LogFailure(filePath, msg)
                UpdateLog($"{Path.GetFileName(filePath)}: {msg}")
                Return
            End If

            ' Try synced lyrics first
            Dim bestMatch As JObject = Nothing
            Dim highestScore As Integer = -1

            For Each result As JObject In jsonArray
                Dim lyricsSync = result.Value(Of String)("syncedLyrics")
                Dim resArtist = result.Value(Of String)("artist_name")?.ToLower()
                Dim resTitle = result.Value(Of String)("track_name")?.ToLower()
                Dim resultDuration = result.Value(Of Integer?)("duration").GetValueOrDefault(0)

                If String.IsNullOrWhiteSpace(lyricsSync) OrElse resultDuration = 0 Then Continue For

                Dim score As Integer = 0
                If resArtist = artist.ToLower() Then
                    score += 2
                ElseIf resArtist?.Contains(artist.ToLower()) Then
                    score += 1
                End If
                If resTitle = title.ToLower() Then
                    score += 2
                ElseIf resTitle?.Contains(title.ToLower()) Then
                    score += 1
                End If

                Dim delta = Math.Abs(localDuration - resultDuration)
                If delta < 2 Then
                    score += 3
                ElseIf delta < 5 Then
                    score += 2
                ElseIf delta < 10 Then
                    score += 1
                End If

                If score > highestScore Then
                    bestMatch = result
                    highestScore = score
                End If
            Next

            ' If no synced lyrics found, and fallback to unsynced is enabled, try unsynced LRClib lyrics
            If bestMatch Is Nothing AndAlso chkFallbackUnsynced.Checked Then
                highestScore = -1
                For Each result As JObject In jsonArray
                    Dim lyricsPlain = result.Value(Of String)("lyrics")
                    Dim resArtist = result.Value(Of String)("artist_name")?.ToLower()
                    Dim resTitle = result.Value(Of String)("track_name")?.ToLower()
                    Dim resultDuration = result.Value(Of Integer?)("duration").GetValueOrDefault(0)

                    If String.IsNullOrWhiteSpace(lyricsPlain) OrElse resultDuration = 0 Then Continue For

                    Dim score As Integer = 0
                    If resArtist = artist.ToLower() Then
                        score += 2
                    ElseIf resArtist?.Contains(artist.ToLower()) Then
                        score += 1
                    End If
                    If resTitle = title.ToLower() Then
                        score += 2
                    ElseIf resTitle?.Contains(title.ToLower()) Then
                        score += 1
                    End If

                    Dim delta = Math.Abs(localDuration - resultDuration)
                    If delta < 2 Then
                        score += 3
                    ElseIf delta < 5 Then
                        score += 2
                    ElseIf delta < 10 Then
                        score += 1
                    End If

                    If score > highestScore Then
                        bestMatch = result
                        highestScore = score
                    End If
                Next
            End If

            Dim finalLyrics As String = ""
            Dim usedSynced As Boolean = False

            If bestMatch IsNot Nothing Then
                Dim synced = bestMatch.Value(Of String)("syncedLyrics")
                If Not String.IsNullOrWhiteSpace(synced) Then
                    finalLyrics = synced
                    usedSynced = True
                ElseIf chkFallbackUnsynced.Checked Then
                    Dim plain = bestMatch.Value(Of String)("lyrics")
                    If Not String.IsNullOrWhiteSpace(plain) Then
                        finalLyrics = plain
                        usedSynced = False
                        UpdateLog($"Falling back to unsynced LRClib lyrics for: {artist} - {title}")
                    End If
                End If
            End If

            ' If still no lyrics and fallback unsynced is checked, try lyrics.ovh fallback
            If String.IsNullOrWhiteSpace(finalLyrics) AndAlso chkFallbackUnsynced.Checked Then
                UpdateLog($"Trying fallback source lyrics.ovh for: {artist} - {title}")
                Dim lyricsOvh = GetLyricsFromLyricsOvh(artist, title)
                If Not String.IsNullOrWhiteSpace(lyricsOvh) Then
                    finalLyrics = lyricsOvh
                    usedSynced = False
                    UpdateLog($"Using lyrics.ovh fallback for: {artist} - {title}")
                End If
            End If

            If String.IsNullOrWhiteSpace(finalLyrics) Then
                Dim msg = "No suitable lyrics found"
                LogFailure(filePath, msg)
                UpdateLog($"{Path.GetFileName(filePath)}: {msg}")
                Return
            End If

            ' Save lyrics tag
            SyncLock lockObj
                tfile.Tag.Lyrics = finalLyrics
                tfile.Save()
            End SyncLock

            Dim lyricTypeText As String = If(usedSynced, " (Synced)", " (Unsynced)")
            LogSuccess(Path.GetFileName(filePath), "Tagged successfully" & lyricTypeText)
            UpdateLog($"Tagged: {artist} - {title}")

        Catch ex As Exception
            LogFailure(Path.GetFileName(filePath), $"Error: {ex.Message}")
            UpdateLog($"{Path.GetFileName(filePath)}: Error - {ex.Message}")
        Finally
            Interlocked.Increment(completedFiles)
            UpdateProgress()
        End Try
    End Sub


    ' Helper to get lyrics from lyrics.ovh
    Function GetLyricsFromLyricsOvh(artist As String, title As String) As String
        Try
            ' Add a delay before the request (e.g., 500 milliseconds)
            'Thread.Sleep(500)

            Dim url = $"https://api.lyrics.ovh/v1/{Uri.EscapeDataString(artist)}/{Uri.EscapeDataString(title)}"
            Using client As New WebClient()
                Dim json = client.DownloadString(url)
                Dim obj = JObject.Parse(json)
                Dim lyrics = obj.Value(Of String)("lyrics")
                Return lyrics
            End Using
        Catch ex As Exception
            Return ""
        End Try
    End Function




    ' Normalizes and cleans the artist/title metadata to improve API matching
    Function CleanTag(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then Return ""

        ' Remove bracketed/parenthetical text
        input = System.Text.RegularExpressions.Regex.Replace(input, "\[.*?\]|\(.*?\)", "")

        ' Remove featured artist info
        input = System.Text.RegularExpressions.Regex.Replace(input, "(feat\.?|ft\.?|featuring)\s+[^\-]+", "", RegexOptions.IgnoreCase)

        ' Replace special/bad characters
        Dim badChars As New Dictionary(Of String, String) From {
        {Chr(34), ""},  ' " (double quote)
        {"'", ""},
        {"“", ""}, {"”", ""}, {"‘", ""}, {"’", ""},
        {"/", " "}, {"\", " "}, {"|", " "}, {"?", ""}, {"*", ""},
        {":", " "}, {"<", ""}, {">", ""}
    }

        For Each kvp In badChars
            input = input.Replace(kvp.Key, kvp.Value)
        Next

        ' Remove control characters
        input = New String(input.Where(Function(c) Not Char.IsControl(c)).ToArray())

        ' Normalize whitespace
        input = System.Text.RegularExpressions.Regex.Replace(input, "\s+", " ").Trim()

        Return input
    End Function


    ' Updates progress bar and label in the UI
    Sub UpdateProgress()
        If InvokeRequired Then
            Invoke(New Action(AddressOf UpdateProgress))
        Else
            ProgressBar.Value = Math.Min(completedFiles, totalFiles)
            lblProgress.Text = $"{completedFiles} processed / {totalFiles} files to scan"
        End If
    End Sub

    ' Appends a message to the log list box
    Sub UpdateLog(msg As String)
        If lstLog.InvokeRequired Then
            lstLog.Invoke(Sub() UpdateLog(msg))
        Else
            lstLog.Items.Add(msg)
            lstLog.TopIndex = lstLog.Items.Count - 1
        End If
    End Sub

    ' Logs a failed tagging attempt and displays it in the UI
    Sub LogFailure(filePath As String, reason As String)
        Dim fileName = Path.GetFileName(filePath)

        Interlocked.Increment(failedFiles)
        UpdateFailedLabel()

        SyncLock lockObj
            failedFilePaths.Add(filePath)
        End SyncLock

        If lvFileResults.InvokeRequired Then
            lvFileResults.Invoke(Sub() LogFailure(filePath, reason))
        Else
            Dim item As New ListViewItem(fileName)
            item.SubItems.Add(reason)
            item.ForeColor = If(reason.ToLower().Contains("error"), Color.Red, Color.DarkGoldenrod)
            lvFileResults.Items.Add(item)
            lvFileResults.EnsureVisible(lvFileResults.Items.Count - 1)
        End If
    End Sub



    Sub LogSuccess(fileName As String, message As String)
        If lvFileResults.InvokeRequired Then
            lvFileResults.Invoke(Sub() LogSuccess(fileName, message))
        Else
            Dim item As New ListViewItem(fileName)
            item.SubItems.Add(message)
            item.ForeColor = If(message.Contains("Skipped"), Color.Aqua, Color.LimeGreen)
            lvFileResults.Items.Add(item)
            lvFileResults.EnsureVisible(lvFileResults.Items.Count - 1)
        End If
    End Sub





    ' Updates the failed file count label
    Sub UpdateFailedLabel()
        If lblFailed.InvokeRequired Then
            lblFailed.Invoke(Sub() UpdateFailedLabel())
        Else
            lblFailed.Text = $"Failed: {failedFiles}"
        End If
    End Sub

    ' Enables sorting when a ListView column header is clicked
    Private Sub lvFailedFiles_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles lvFileResults.ColumnClick
        If e.Column = sortColumn Then
            sortOrder = If(sortOrder = SortOrder.Ascending, SortOrder.Descending, SortOrder.Ascending)
        Else
            sortColumn = e.Column
            sortOrder = SortOrder.Ascending
        End If
        lvFileResults.ListViewItemSorter = New ListViewItemComparer(sortColumn, sortOrder)
        lvFileResults.Sort()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lstLog.DrawMode = DrawMode.OwnerDrawFixed
        AddHandler lstLog.DrawItem, AddressOf lstLog_DrawItem
        EnableDoubleBuffering(lvFileResults)
    End Sub
    Private Sub lstLog_DrawItem(sender As Object, e As DrawItemEventArgs)
        If e.Index < 0 Then Return

        Dim lb As ListBox = CType(sender, ListBox)
        Dim text As String = lb.Items(e.Index).ToString()

        e.DrawBackground()

        ' Choose color based on message content
        Dim textColor As Brush = Brushes.White
        If text.Contains("Error") OrElse text.Contains("error") Then
            textColor = Brushes.Red
        ElseIf text.Contains("Failed") OrElse text.Contains("No suitable") Then
            textColor = Brushes.DarkOrange
        ElseIf text.Contains("Tagged") Then
            textColor = Brushes.LimeGreen
        ElseIf text.Contains("Skipped") Then
            textColor = Brushes.Aqua
        ElseIf text = "Finished." Then
            textColor = Brushes.LightGreen
        End If

        e.Graphics.DrawString(text, e.Font, textColor, e.Bounds.Location)
        e.DrawFocusRectangle()
    End Sub

End Class


' Custom ListView sorter class
Public Class ListViewItemComparer
    Implements IComparer
    Private col As Integer
    Private order As SortOrder

    Public Sub New(column As Integer, sortOrder As SortOrder)
        col = column
        order = sortOrder
    End Sub

    Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
        Dim itemX As ListViewItem = CType(x, ListViewItem)
        Dim itemY As ListViewItem = CType(y, ListViewItem)
        Dim result As Integer = String.Compare(itemX.SubItems(col).Text, itemY.SubItems(col).Text)
        Return If(order = SortOrder.Descending, -result, result)
    End Function
End Class
