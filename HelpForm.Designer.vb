<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HelpForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        txtHelpContent = New TextBox()
        SuspendLayout()
        ' 
        ' txtHelpContent
        ' 
        txtHelpContent.Dock = DockStyle.Fill
        txtHelpContent.Location = New Point(0, 0)
        txtHelpContent.Multiline = True
        txtHelpContent.Name = "txtHelpContent"
        txtHelpContent.ReadOnly = True
        txtHelpContent.ScrollBars = ScrollBars.Vertical
        txtHelpContent.Size = New Size(800, 450)
        txtHelpContent.TabIndex = 0
        ' 
        ' HelpForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(txtHelpContent)
        Name = "HelpForm"
        Text = "HelpForm"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents txtHelpContent As TextBox
End Class
