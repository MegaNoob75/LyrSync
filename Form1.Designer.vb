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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        btnSelectFolder = New Button()
        btnStart = New Button()
        lstLog = New ListBox()
        ProgressBar = New ProgressBar()
        lblProgress = New Label()
        txtFolder = New TextBox()
        lblFailed = New Label()
        lvFileResults = New ListView()
        ColumnHeader1 = New ColumnHeader()
        ColumnHeader2 = New ColumnHeader()
        numMaxConcurrent = New NumericUpDown()
        Label1 = New Label()
        TextBox1 = New TextBox()
        Label2 = New Label()
        Label3 = New Label()
        chkFallbackUnsynced = New CheckBox()
        lblTimeInfo = New Label()
        CType(numMaxConcurrent, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' btnSelectFolder
        ' 
        btnSelectFolder.ForeColor = SystemColors.ActiveCaptionText
        btnSelectFolder.Location = New Point(12, 8)
        btnSelectFolder.Name = "btnSelectFolder"
        btnSelectFolder.Size = New Size(108, 32)
        btnSelectFolder.TabIndex = 0
        btnSelectFolder.Text = "Sellect folder"
        btnSelectFolder.UseVisualStyleBackColor = True
        ' 
        ' btnStart
        ' 
        btnStart.ForeColor = SystemColors.ActiveCaptionText
        btnStart.Location = New Point(880, 833)
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
        lstLog.Location = New Point(12, 43)
        lstLog.Name = "lstLog"
        lstLog.Size = New Size(657, 379)
        lstLog.TabIndex = 3
        ' 
        ' ProgressBar
        ' 
        ProgressBar.Location = New Point(14, 443)
        ProgressBar.Name = "ProgressBar"
        ProgressBar.Size = New Size(655, 23)
        ProgressBar.Style = ProgressBarStyle.Continuous
        ProgressBar.TabIndex = 4
        ' 
        ' lblProgress
        ' 
        lblProgress.AutoSize = True
        lblProgress.Location = New Point(821, 313)
        lblProgress.Name = "lblProgress"
        lblProgress.Size = New Size(30, 15)
        lblProgress.TabIndex = 5
        lblProgress.Text = "0 / 0"
        ' 
        ' txtFolder
        ' 
        txtFolder.Location = New Point(126, 14)
        txtFolder.Name = "txtFolder"
        txtFolder.Size = New Size(543, 23)
        txtFolder.TabIndex = 6
        txtFolder.Text = "Please Sellect Folder To Scan"
        ' 
        ' lblFailed
        ' 
        lblFailed.AutoSize = True
        lblFailed.Location = New Point(821, 352)
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
        lvFileResults.Location = New Point(14, 496)
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
        ' numMaxConcurrent
        ' 
        numMaxConcurrent.Location = New Point(900, 10)
        numMaxConcurrent.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        numMaxConcurrent.Name = "numMaxConcurrent"
        numMaxConcurrent.Size = New Size(50, 23)
        numMaxConcurrent.TabIndex = 10
        numMaxConcurrent.Value = New Decimal(New Integer() {10, 0, 0, 0})
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(719, 17)
        Label1.Name = "Label1"
        Label1.Size = New Size(175, 15)
        Label1.TabIndex = 11
        Label1.Text = "LRCLIB Max Concurrent Queries"
        ' 
        ' TextBox1
        ' 
        TextBox1.BackColor = SystemColors.ControlDarkDark
        TextBox1.ForeColor = SystemColors.Window
        TextBox1.Location = New Point(675, 43)
        TextBox1.Multiline = True
        TextBox1.Name = "TextBox1"
        TextBox1.Size = New Size(336, 235)
        TextBox1.TabIndex = 12
        TextBox1.Text = resources.GetString("TextBox1.Text")
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(682, 313)
        Label2.Name = "Label2"
        Label2.Size = New Size(86, 15)
        Label2.TabIndex = 13
        Label2.Text = "Files Processed"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(682, 352)
        Label3.Name = "Label3"
        Label3.Size = New Size(64, 15)
        Label3.TabIndex = 14
        Label3.Text = "Files Failed"
        ' 
        ' chkFallbackUnsynced
        ' 
        chkFallbackUnsynced.AutoSize = True
        chkFallbackUnsynced.Location = New Point(682, 392)
        chkFallbackUnsynced.Name = "chkFallbackUnsynced"
        chkFallbackUnsynced.Size = New Size(235, 19)
        chkFallbackUnsynced.TabIndex = 15
        chkFallbackUnsynced.Text = "Use unsynced lyrics if synced not found"
        chkFallbackUnsynced.UseVisualStyleBackColor = True
        ' 
        ' lblTimeInfo
        ' 
        lblTimeInfo.AutoSize = True
        lblTimeInfo.Location = New Point(883, 779)
        lblTimeInfo.Name = "lblTimeInfo"
        lblTimeInfo.Size = New Size(28, 15)
        lblTimeInfo.TabIndex = 16
        lblTimeInfo.Text = "0:00"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        AutoSize = True
        BackColor = SystemColors.ControlDarkDark
        ClientSize = New Size(1021, 881)
        Controls.Add(lblTimeInfo)
        Controls.Add(chkFallbackUnsynced)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(TextBox1)
        Controls.Add(Label1)
        Controls.Add(numMaxConcurrent)
        Controls.Add(lvFileResults)
        Controls.Add(lblFailed)
        Controls.Add(txtFolder)
        Controls.Add(lblProgress)
        Controls.Add(ProgressBar)
        Controls.Add(lstLog)
        Controls.Add(btnStart)
        Controls.Add(btnSelectFolder)
        ForeColor = SystemColors.ButtonHighlight
        FormBorderStyle = FormBorderStyle.FixedSingle
        Name = "Form1"
        Text = "LYRSYNC V1.0"
        CType(numMaxConcurrent, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btnSelectFolder As Button
    Friend WithEvents btnStart As Button
    Friend WithEvents lstLog As ListBox
    Friend WithEvents ProgressBar As ProgressBar
    Friend WithEvents lblProgress As Label
    Friend WithEvents txtFolder As TextBox
    Friend WithEvents lblFailed As Label
    Friend WithEvents lvFileResults As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents numMaxConcurrent As NumericUpDown
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents chkFallbackUnsynced As CheckBox
    Friend WithEvents lblTimeInfo As Label

End Class
