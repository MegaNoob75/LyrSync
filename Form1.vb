Imports System.Collections
Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Threading.Tasks
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

        ProgressBar.Value = 0
        lstLog.Items.Clear()
        lvFileResults.Items.Clear()
        lstLog.Items.Add("Starting tagging...")

        completedFiles = 0
        failedFiles = 0
        UpdateFailedLabel()

        ' Set maximum concurrency based on user input
        semaphore = New SemaphoreSlim(CInt(numMaxConcurrent.Value))

        ' Start processing all files asynchronously
        Await ProcessAllFiles(selectedFolder, cancelToken)

        taggingRunning = False
        Invoke(Sub()
                   btnStart.Text = "Start Tagging"
                   btnStart.Enabled = True
               End Sub)

        lstLog.Items.Add("Finished.")
        lstLog.TopIndex = lstLog.Items.Count - 1
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

            If String.IsNullOrWhiteSpace(rawArtist) OrElse String.IsNullOrWhiteSpace(rawTitle) Then
                Dim msg = "Missing artist or title tag"
                LogFailure(Path.GetFileName(filePath), msg)
                UpdateLog($"{Path.GetFileName(filePath)}: {msg}")
                Return
            End If

            Dim artist = CleanTag(rawArtist)
            Dim title = CleanTag(rawTitle)
            Dim localDuration = CInt(tfile.Properties.Duration.TotalSeconds)
            Dim url = $"https://lrclib.net/api/search?track_name={Uri.EscapeDataString(title)}&artist_name={Uri.EscapeDataString(artist)}"
            UpdateLog($"Querying: {artist} - {title}")

            Dim jsonText As String = ""
            Using client As New WebClient()
                jsonText = client.DownloadString(url)
            End Using

            Dim jsonArray = JArray.Parse(jsonText)
            Dim bestMatch As JObject = Nothing
            Dim highestScore As Integer = -1
            Dim finalLyrics As String = ""
            Dim bestLyricsType As String = ""

            ' Try synced lyrics first
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
                    finalLyrics = lyricsSync
                    bestLyricsType = "synced"
                End If
            Next

            ' Try unsynced if fallback is enabled
            If bestMatch Is Nothing AndAlso chkFallbackUnsynced.Checked Then
                highestScore = -1
                For Each result As JObject In jsonArray
                    Dim lyricsPlain = result.Value(Of String)("lyrics")
                    Dim resArtist = result.Value(Of String)("artist_name")?.ToLower()
                    Dim resTitle = result.Value(Of String)("track_name")?.ToLower()
                    Dim resultDuration = result.Value(Of Integer?)("duration").GetValueOrDefault(0)
                    If String.IsNullOrWhiteSpace(lyricsPlain) OrElse resultDuration = 0 Then Continue For

                    UpdateLog($"Checked fallback result: {resArtist} - {resTitle} (lyrics length: {If(lyricsPlain Is Nothing, 0, lyricsPlain.Length)})")

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
                        finalLyrics = lyricsPlain
                        bestLyricsType = "unsynced (lrclib)"
                    End If
                Next
            End If

            ' Try Lyrics.ovh as final fallback
            If bestMatch Is Nothing AndAlso chkFallbackUnsynced.Checked Then
                Dim ovhLyrics = FetchUnsyncedLyricsFromLyricsOvh(artist, title)
                If Not String.IsNullOrWhiteSpace(ovhLyrics) Then
                    finalLyrics = ovhLyrics
                    bestLyricsType = "unsynced (Lyrics.ovh)"
                    UpdateLog($"Used fallback from Lyrics.ovh for: {artist} - {title}")
                End If
            End If

            If String.IsNullOrWhiteSpace(finalLyrics) Then
                Dim msg = "No suitable lyrics found"
                LogFailure(Path.GetFileName(filePath), msg)
                UpdateLog($"{Path.GetFileName(filePath)}: {msg}")
                Return
            End If

            SyncLock lockObj
                tfile.Tag.Lyrics = finalLyrics
                tfile.Save()
            End SyncLock

            LogSuccess(Path.GetFileName(filePath), "Tagged successfully (" & bestLyricsType & ")")
            UpdateLog($"Tagged: {artist} - {title}")

        Catch ex As Exception
            LogFailure(Path.GetFileName(filePath), $"Error: {ex.Message}")
            UpdateLog($"{Path.GetFileName(filePath)}: Error - {ex.Message}")
        Finally
            Interlocked.Increment(completedFiles)
            UpdateProgress()
        End Try
    End Sub

    Function FetchUnsyncedLyricsFromLyricsOvh(artist As String, title As String) As String
        Try
            Dim url = $"https://api.lyrics.ovh/v1/{Uri.EscapeDataString(artist)}/{Uri.EscapeDataString(title)}"
            Using client As New WebClient()
                Dim jsonText = client.DownloadString(url)
                Dim json = JObject.Parse(jsonText)
                Dim lyrics = json.Value(Of String)("lyrics")
                Return If(String.IsNullOrWhiteSpace(lyrics), Nothing, lyrics)
            End Using
        Catch ex As Exception
            UpdateLog($"Lyrics.ovh error: {ex.Message}")
        End Try
        Return Nothing
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
    Sub LogFailure(fileName As String, reason As String)
        Interlocked.Increment(failedFiles)
        UpdateFailedLabel()
        If lvFileResults.InvokeRequired Then
            lvFileResults.Invoke(Sub() LogFailure(fileName, reason))
        Else
            Dim item As New ListViewItem(fileName)
            item.SubItems.Add(reason)
            item.ForeColor = If(reason.ToLower().Contains("error"), Color.Red, Color.DarkGoldenrod)
            lvFileResults.Items.Add(item)
            lvFileResults.EnsureVisible(lvFileResults.Items.Count - 1) ' Auto-scroll here
        End If
    End Sub

    Sub LogSuccess(fileName As String, message As String)
        If lvFileResults.InvokeRequired Then
            lvFileResults.Invoke(Sub() LogSuccess(fileName, message))
        Else
            Dim item As New ListViewItem(fileName)
            item.SubItems.Add(message)
            item.ForeColor = Color.LimeGreen
            lvFileResults.Items.Add(item)
            lvFileResults.EnsureVisible(lvFileResults.Items.Count - 1) ' Auto-scroll here
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
    End Sub
    Private Sub lstLog_DrawItem(sender As Object, e As DrawItemEventArgs)
        If e.Index < 0 Then Return
        Dim lb As ListBox = CType(sender, ListBox)
        Dim text As String = lb.Items(e.Index).ToString()

        e.DrawBackground()

        Dim textColor As Brush = Brushes.White
        If text = "Finished." Then
            textColor = Brushes.LimeGreen
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
