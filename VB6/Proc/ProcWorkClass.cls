VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "CCProc"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
' Author: James William Dunn
' Version: 7.0

Private conn As New ADODB.Connection
Private gConnStr As String
Private gInterval As Integer
Private mWorkForm As WorkForm
Private gRetryOccurred As Boolean

Private WithEvents tmrProcessor As Timer
Attribute tmrProcessor.VB_VarHelpID = -1

Private lLoops As Long
Private lMaxTicks As Long
Private lAverageTicks As Long
Private lMinTicks As Long
Private lTotalTicks As Long
Private lDiffTicks As Long

Event ErrorEncountered(err1 As String)

Private Sub Class_Initialize()
  gRetryOccurred = False
  Set mWorkForm = New WorkForm
  Load mWorkForm
  Set tmrProcessor = mWorkForm.Timer1
  gConnStr = "provider=sqloledb;"
  gInterval = 1000
  Randomize
  lMinTicks = 1000
End Sub

Private Sub Class_Terminate()
  On Error Resume Next
  conn.Close

  Set tmrProcessor = Nothing
  Unload mWorkForm
  Set mWorkForm = Nothing
End Sub

Property Let ConnectString(sConnStr As String)
  gConnStr = sConnStr
End Property

Property Let TimerInterval(iInterval As Integer)
  gInterval = iInterval
End Property

Public Sub StartCCProcess()
On Error GoTo abort:

conn.Open gConnStr

tmrProcessor.Enabled = True
tmrProcessor.Interval = gInterval
Exit Sub

abort:
  RaiseEvent ErrorEncountered("Problem encountered opening a connection")
End Sub

Public Sub PauseCCProcess()
  tmrProcessor.Enabled = False
End Sub

Public Sub ResumeCCProcess()
  tmrProcessor.Enabled = True
  tmrProcessor.Interval = gInterval
End Sub

Public Function GetStatCur() As Long
  GetStatCur = lDiffTicks
End Function
Public Function GetStatMin() As Long
  GetStatMin = lMinTicks
End Function
Public Function GetStatMax() As Long
  GetStatMax = lMaxTicks
End Function
Public Function GetStatAvg() As Long
  GetStatAvg = lAverageTicks
End Function
Public Function CheckForRetryError() As Boolean
  CheckForRetryError = gRetryOccurred
End Function

Private Function GetCommand() As String
  Dim sCardNo As String
  Dim cmd As String
  
  sCardNo = Format(CLng(Rnd() * 9999999), "0000000")

  If Rnd > 0.5 Then    ' 50% of the time do a SELECT, otherwise do an UPDATE
    cmd = "GetAccount '37378350" & sCardNo & "'"
  Else
    cmd = "UpdateAccount '37378350" & sCardNo & "', " & _
      Trim(Format(1 + Rnd() * 100, "#.00")) & ", '" & Now & "'"
  End If
  GetCommand = cmd
End Function


Private Sub tmrProcessor_Timer()
  Dim lStartTicks As Long

On Error GoTo abort:

  cmd = GetCommand()
retry:
  lStartTicks = GetTickCount()
  conn.Execute cmd
  lDiffTicks = GetTickCount() - lStartTicks
  lLoops = lLoops + 1
  lTotalTicks = lTotalTicks + lDiffTicks
  lAverageTicks = lTotalTicks / lLoops
  If lDiffTicks < lMinTicks Then lMinTicks = lDiffTicks
  If lDiffTicks > lMaxTicks Then lMaxTicks = lDiffTicks
  Randomize
Exit Sub

abort:
  If Err.Number = -2147217871 Then
    gRetryOccurred = True
    Resume retry
  End If

  RaiseEvent ErrorEncountered("Problem encountered during processing:" & Str(Err.Number))
End Sub

