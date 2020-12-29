VERSION 5.00
Begin VB.Form Form1 
   BorderStyle     =   1  'Fixed Single
   Caption         =   " dbAnimator"
   ClientHeight    =   2625
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   4560
   Icon            =   "ProcMainForm.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   ScaleHeight     =   175
   ScaleMode       =   3  'Pixel
   ScaleWidth      =   304
   StartUpPosition =   3  'Windows Default
   Begin VB.Timer Timer1 
      Enabled         =   0   'False
      Interval        =   5000
      Left            =   1080
      Top             =   2040
   End
   Begin VB.CommandButton Command2 
      Caption         =   "S&tats"
      Enabled         =   0   'False
      Height          =   330
      Left            =   180
      TabIndex        =   13
      Top             =   2130
      Width           =   825
   End
   Begin VB.TextBox Text6 
      Height          =   300
      IMEMode         =   3  'DISABLE
      Left            =   2880
      PasswordChar    =   "*"
      TabIndex        =   12
      Top             =   2130
      Width           =   1500
   End
   Begin VB.TextBox Text5 
      Height          =   300
      Left            =   2880
      TabIndex        =   10
      Text            =   "sa"
      Top             =   1740
      Width           =   1500
   End
   Begin VB.TextBox Text4 
      Height          =   300
      Left            =   2880
      TabIndex        =   8
      Text            =   "Test"
      Top             =   1350
      Width           =   1500
   End
   Begin VB.TextBox Text3 
      Height          =   300
      Left            =   2880
      TabIndex        =   6
      Text            =   "localhost"
      Top             =   960
      Width           =   1500
   End
   Begin VB.TextBox Text2 
      Height          =   300
      Left            =   2880
      TabIndex        =   4
      Text            =   "100"
      Top             =   570
      Width           =   1500
   End
   Begin VB.TextBox Text1 
      BeginProperty DataFormat 
         Type            =   1
         Format          =   "0"
         HaveTrueFalseNull=   0
         FirstDayOfWeek  =   0
         FirstWeekOfYear =   0
         LCID            =   1033
         SubFormatType   =   1
      EndProperty
      Height          =   300
      Left            =   2880
      MaxLength       =   3
      TabIndex        =   2
      Text            =   "10"
      Top             =   180
      Width           =   1500
   End
   Begin VB.CommandButton Command1 
      Caption         =   "&Start"
      BeginProperty Font 
         Name            =   "MS Sans Serif"
         Size            =   9.75
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      Height          =   690
      Left            =   180
      TabIndex        =   0
      Top             =   180
      Width           =   1275
   End
   Begin VB.Label Author 
      Caption         =   "James W. Dunn"
      Enabled         =   0   'False
      Height          =   375
      Left            =   5520
      TabIndex        =   15
      Top             =   2400
      Visible         =   0   'False
      Width           =   495
   End
   Begin VB.Label Label7 
      Height          =   375
      Left            =   300
      TabIndex        =   14
      Top             =   1095
      Width           =   1215
   End
   Begin VB.Label Label6 
      Caption         =   "&Password:"
      Height          =   255
      Left            =   1860
      TabIndex        =   11
      Top             =   2175
      Width           =   975
   End
   Begin VB.Label Label5 
      Caption         =   "&UserID:"
      Height          =   255
      Left            =   1860
      TabIndex        =   9
      Top             =   1785
      Width           =   975
   End
   Begin VB.Label Label4 
      Caption         =   "&Database:"
      Height          =   255
      Left            =   1860
      TabIndex        =   7
      Top             =   1395
      Width           =   975
   End
   Begin VB.Label Label3 
      Caption         =   "S&erver:"
      Height          =   255
      Left            =   1860
      TabIndex        =   5
      Top             =   1005
      Width           =   975
   End
   Begin VB.Label Label2 
      Caption         =   "I&nterval (ms):"
      Height          =   255
      Left            =   1860
      TabIndex        =   3
      Top             =   615
      Width           =   975
   End
   Begin VB.Label Label1 
      Caption         =   "&Instances:"
      Height          =   255
      Left            =   1860
      TabIndex        =   1
      Top             =   225
      Width           =   975
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
' Author: James William Dunn
' Version: 7.0

Private Declare Sub Sleep Lib "kernel32.dll" (ByVal dwMilliseconds As Long)
Dim x() As ProcWork.CCProc
Dim WithEvents Work As ProcWork.CCProc
Attribute Work.VB_VarHelpID = -1
Private state As Integer  ' 0 = Start   1 = Run   2 = Pause
Private interval As Integer
Private statsLoaded As Boolean

Private Sub Command1_Click()
    Select Case state
      Case 0 ' START -> RUN
        state = 1 ' Enter RUN state
  
        gSize = CInt(Text1.Text)
        ReDim x(gSize)
        ReDim gStats(gSize)
    
        Command1.Caption = "&Pause"
        Text1.Enabled = False
        Text2.Enabled = False
        Text3.Enabled = False
        Text4.Enabled = False
        Text5.Enabled = False
        Text6.Enabled = False
        Label7.Caption = "Launching..."
        interval = CInt(Text2.Text) ' Note: main thread may starve below 16ms
        If interval < 16 Then
          interval = 16
          Text2.Text = "16"
        End If
        MousePointer = 11
        For i = 1 To gSize
          Set x(i) = New CCProc
          If i = 1 Then Set Work = x(i)
          x(i).ConnectString = "provider=sqloledb;server=" & Text3.Text & ";database=" & Text4.Text & ";uid=" & Text5.Text & ";pwd=" & Text6.Text & ";"
          x(i).TimerInterval = interval
          x(i).StartCCProcess
          DoEvents ' Yield for a moment to allow worker startup
          Sleep 97 ' adjust this value to pace the launch sequence
        Next i
        MousePointer = 0
        Label7.Caption = "Running"
        Timer1.Enabled = True
        ' Enable stats button
        Command2.Enabled = True
        Refresh
    
    Case 1 ' RUN -> PAUSE
        state = 2 ' Enter PAUSE state
        Command1.Caption = "&Resume"
        Label7.Caption = "Paused"
        Text2.Enabled = True
        MousePointer = 11
        For i = 1 To gSize
          x(i).PauseCCProcess
        Next i
        MousePointer = 0
        Timer1.Enabled = False
        Refresh
      
    Case 2  ' PAUSE -> RUN
        state = 1 ' Enter RUN state
        Command1.Caption = "&Pause"
        Label7.Caption = "Running"
        Text2.Enabled = False
        MousePointer = 11
        interval = CInt(Text2.Text) ' Note: main thread may starve below 16ms
        If interval < 16 Then
          interval = 16
          Text2.Text = "16"
        End If
        For i = 1 To gSize
          x(i).TimerInterval = interval
          x(i).ResumeCCProcess
        Next i
        MousePointer = 0
        Timer1.Enabled = True
        Refresh
        
    End Select
End Sub

Private Sub Command2_Click()
  MousePointer = 11
  For i = 1 To gSize
    gStats(i).cur = x(i).GetStatCur
    gStats(i).min = x(i).GetStatMin
    gStats(i).avg = x(i).GetStatAvg
    gStats(i).max = x(i).GetStatMax
  Next i
  MousePointer = 0

  Load Form2
  Form2.Show
  statsLoaded = True
End Sub

Private Sub Form_Load()
  state = 0
  interval = 100
  statsLoaded = False
End Sub

Private Sub Form_QueryUnload(Cancel As Integer, UnloadMode As Integer)
    Form1.Label7.Caption = "Terminating..."
    Refresh
    If state = 1 Then
        For i = 1 To gSize
          x(i).PauseCCProcess
          DoEvents ' Yield for a moment to allow work shutdown
          Sleep 60
        Next i
    End If
    If statsLoaded Then Unload Form2
End Sub

Private Sub Timer1_Timer()
  For i = 1 To gSize
    gStats(i).cur = x(i).GetStatCur
    gStats(i).min = x(i).GetStatMin
    gStats(i).avg = x(i).GetStatAvg
    gStats(i).max = x(i).GetStatMax
  Next i
End Sub

Private Sub Work_ErrorEncountered(err1 As String)
  If err1 = "Retry" Then
    Label7.Caption = "Retry encountered in Worker " & CStr(iID)
  Else
    MsgBox err1
    End
  End If
End Sub

' For behavior similar to VB16
Private Sub SelectAllText(tb As TextBox)
  tb.SelStart = 0
  tb.SelLength = Len(tb.Text)
End Sub

Private Sub Text1_GotFocus()
  SelectAllText Text1
End Sub

Private Sub Text2_GotFocus()
  SelectAllText Text2
End Sub

Private Sub Text3_GotFocus()
  SelectAllText Text3
End Sub

Private Sub Text4_GotFocus()
  SelectAllText Text4
End Sub

Private Sub Text5_GotFocus()
  SelectAllText Text5
End Sub

Private Sub Text6_GotFocus()
  SelectAllText Text6
End Sub
