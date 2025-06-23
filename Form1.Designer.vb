<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        btnStart = New Button()
        lstLog = New ListBox()
        ProgressBar = New ProgressBar()
        lblProgress = New Label()
        lblFailed = New Label()
        lvFileResults = New ListView()
        ColumnHeader1 = New ColumnHeader()
        ColumnHeader2 = New ColumnHeader()
        Label2 = New Label()
        Label3 = New Label()
        lblTimeInfo = New Label()
        chkRetryFailed = New CheckBox()
        MenuStrip1 = New MenuStrip()
        FileToolStripMenuItem = New ToolStripMenuItem()
        menuSelectFolder = New ToolStripMenuItem()
        SettingsToolStripMenuItem1 = New ToolStripMenuItem()
        menuSkipIfSynced = New ToolStripMenuItem()
        MaxConcurrentThreadsToolStripMenuItem = New ToolStripMenuItem()
        txtMaxConcurrent = New ToolStripTextBox()
        chkFallbackUnsynced = New ToolStripMenuItem()
        HelpToolStripMenuItem = New ToolStripMenuItem()
        MenuStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnStart
        ' 
        btnStart.ForeColor = SystemColors.ActiveCaptionText
        btnStart.Location = New Point(10, 701)
        btnStart.Name = "btnStart"
        btnStart.Size = New Size(131, 42)
        btnStart.TabIndex = 2
        btnStart.Text = "Embed Lyrics"
        btnStart.UseVisualStyleBackColor = True
        ' 
        ' lstLog
        ' 
        lstLog.BackColor = SystemColors.ControlDarkDark
        lstLog.ForeColor = SystemColors.Window
        lstLog.FormattingEnabled = True
        lstLog.ItemHeight = 15
        lstLog.Location = New Point(12, 27)
        lstLog.Name = "lstLog"
        lstLog.Size = New Size(657, 229)
        lstLog.TabIndex = 3
        ' 
        ' ProgressBar
        ' 
        ProgressBar.Location = New Point(14, 274)
        ProgressBar.Name = "ProgressBar"
        ProgressBar.Size = New Size(655, 23)
        ProgressBar.Style = ProgressBarStyle.Continuous
        ProgressBar.TabIndex = 4
        ' 
        ' lblProgress
        ' 
        lblProgress.AutoSize = True
        lblProgress.Location = New Point(133, 756)
        lblProgress.Name = "lblProgress"
        lblProgress.Size = New Size(30, 15)
        lblProgress.TabIndex = 5
        lblProgress.Text = "0 / 0"
        ' 
        ' lblFailed
        ' 
        lblFailed.AutoSize = True
        lblFailed.Location = New Point(133, 786)
        lblFailed.Name = "lblFailed"
        lblFailed.Size = New Size(13, 15)
        lblFailed.TabIndex = 8
        lblFailed.Text = "0"
        ' 
        ' lvFileResults
        ' 
        lvFileResults.BackColor = SystemColors.ControlDarkDark
        lvFileResults.Columns.AddRange(New ColumnHeader() {ColumnHeader1, ColumnHeader2})
        lvFileResults.ForeColor = SystemColors.Window
        lvFileResults.FullRowSelect = True
        lvFileResults.GridLines = True
        lvFileResults.Location = New Point(12, 316)
        lvFileResults.Name = "lvFileResults"
        lvFileResults.Size = New Size(655, 379)
        lvFileResults.TabIndex = 9
        lvFileResults.UseCompatibleStateImageBehavior = False
        lvFileResults.View = View.Details
        ' 
        ' ColumnHeader1
        ' 
        ColumnHeader1.Text = "File Name"
        ColumnHeader1.Width = 250
        ' 
        ' ColumnHeader2
        ' 
        ColumnHeader2.Text = "Status"
        ColumnHeader2.Width = 400
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(10, 759)
        Label2.Name = "Label2"
        Label2.Size = New Size(86, 15)
        Label2.TabIndex = 13
        Label2.Text = "Files Processed"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(10, 786)
        Label3.Name = "Label3"
        Label3.Size = New Size(64, 15)
        Label3.TabIndex = 14
        Label3.Text = "Files Failed"
        ' 
        ' lblTimeInfo
        ' 
        lblTimeInfo.AutoSize = True
        lblTimeInfo.Location = New Point(147, 715)
        lblTimeInfo.Name = "lblTimeInfo"
        lblTimeInfo.Size = New Size(28, 15)
        lblTimeInfo.TabIndex = 16
        lblTimeInfo.Text = "0:00"
        ' 
        ' chkRetryFailed
        ' 
        chkRetryFailed.AutoSize = True
        chkRetryFailed.Location = New Point(520, 226)
        chkRetryFailed.Name = "chkRetryFailed"
        chkRetryFailed.Size = New Size(138, 19)
        chkRetryFailed.TabIndex = 18
        chkRetryFailed.Text = "Retry failed files once"
        chkRetryFailed.UseVisualStyleBackColor = True
        chkRetryFailed.Visible = False
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.Items.AddRange(New ToolStripItem() {FileToolStripMenuItem, SettingsToolStripMenuItem1, HelpToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Size = New Size(677, 24)
        MenuStrip1.TabIndex = 19
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' FileToolStripMenuItem
        ' 
        FileToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {menuSelectFolder})
        FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        FileToolStripMenuItem.Size = New Size(37, 20)
        FileToolStripMenuItem.Text = "File"
        ' 
        ' menuSelectFolder
        ' 
        menuSelectFolder.Name = "menuSelectFolder"
        menuSelectFolder.Size = New Size(144, 22)
        menuSelectFolder.Text = "Sellect Folder"
        ' 
        ' SettingsToolStripMenuItem1
        ' 
        SettingsToolStripMenuItem1.DropDownItems.AddRange(New ToolStripItem() {menuSkipIfSynced, MaxConcurrentThreadsToolStripMenuItem, chkFallbackUnsynced})
        SettingsToolStripMenuItem1.Name = "SettingsToolStripMenuItem1"
        SettingsToolStripMenuItem1.Size = New Size(61, 20)
        SettingsToolStripMenuItem1.Text = "Settings"
        ' 
        ' menuSkipIfSynced
        ' 
        menuSkipIfSynced.Checked = True
        menuSkipIfSynced.CheckOnClick = True
        menuSkipIfSynced.CheckState = CheckState.Checked
        menuSkipIfSynced.Name = "menuSkipIfSynced"
        menuSkipIfSynced.Size = New Size(223, 22)
        menuSkipIfSynced.Text = "Skip Files With Synced Lyrics"
        ' 
        ' MaxConcurrentThreadsToolStripMenuItem
        ' 
        MaxConcurrentThreadsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {txtMaxConcurrent})
        MaxConcurrentThreadsToolStripMenuItem.Name = "MaxConcurrentThreadsToolStripMenuItem"
        MaxConcurrentThreadsToolStripMenuItem.Size = New Size(223, 22)
        MaxConcurrentThreadsToolStripMenuItem.Text = "Max Concurrent Threads"
        ' 
        ' txtMaxConcurrent
        ' 
        txtMaxConcurrent.Name = "txtMaxConcurrent"
        txtMaxConcurrent.Size = New Size(100, 23)
        txtMaxConcurrent.Text = "10"
        ' 
        ' chkFallbackUnsynced
        ' 
        chkFallbackUnsynced.Checked = True
        chkFallbackUnsynced.CheckOnClick = True
        chkFallbackUnsynced.CheckState = CheckState.Checked
        chkFallbackUnsynced.Name = "chkFallbackUnsynced"
        chkFallbackUnsynced.Size = New Size(223, 22)
        chkFallbackUnsynced.Text = "Use Unsynced as fallback"
        ' 
        ' HelpToolStripMenuItem
        ' 
        HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        HelpToolStripMenuItem.Size = New Size(44, 20)
        HelpToolStripMenuItem.Text = "Help"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        AutoSize = True
        BackColor = SystemColors.ControlDarkDark
        ClientSize = New Size(677, 809)
        Controls.Add(chkRetryFailed)
        Controls.Add(lblTimeInfo)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(lvFileResults)
        Controls.Add(lblFailed)
        Controls.Add(lblProgress)
        Controls.Add(ProgressBar)
        Controls.Add(lstLog)
        Controls.Add(btnStart)
        Controls.Add(MenuStrip1)
        ForeColor = SystemColors.ButtonHighlight
        FormBorderStyle = FormBorderStyle.FixedSingle
        MainMenuStrip = MenuStrip1
        Name = "Form1"
        Text = "LYRSYNC V1.0"
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents btnStart As Button
    Friend WithEvents lstLog As ListBox
    Friend WithEvents ProgressBar As ProgressBar
    Friend WithEvents lblProgress As Label
    Friend WithEvents lblFailed As Label
    Friend WithEvents lvFileResults As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents lblTimeInfo As Label
    Friend WithEvents chkRetryFailed As CheckBox
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents menuSelectFolder As ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents menuSkipIfSynced As ToolStripMenuItem
    Friend WithEvents MaxConcurrentThreadsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents txtMaxConcurrent As ToolStripTextBox
    Friend WithEvents chkFallbackUnsynced As ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As ToolStripMenuItem

End Class
