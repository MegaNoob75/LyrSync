Public Class HelpForm
    Private Sub HelpForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtHelpContent.Text = "How to Use the Lyrics Tagging App:

1. Click 'File' > 'Select Folder' to choose a folder with FLAC or MP3 files.
2. Files will be listed with a 'Not processed' status.
3. Set concurrency with 'Settings' > 'Max Concurrent Threads'.
4. Enable 'Skip If Synced' to skip files with synced lyrics.
5. Enable 'Skip If Unsynced Lyrics Exist' to skip files with any existing unsynced lyrics.
6. Enable 'Fallback to Unsynced' to try unsynced lyrics if synced ones aren't found.
7. Click 'Start Tagging' to begin.
8. The app logs each result and shows elapsed/estimated time.
9. View results in the list; failed entries are color-coded.
10. Use the column headers to sort the result list.

"
    End Sub
End Class