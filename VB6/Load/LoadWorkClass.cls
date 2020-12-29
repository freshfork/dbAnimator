VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "LoadProc"
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
Private WithEvents tmrProcessor As Timer
Attribute tmrProcessor.VB_VarHelpID = -1
Private loops As Long
Private lCardNo As Long
Private complete As Boolean

Event ErrorEncountered(err1 As String)

Property Let ConnectString(sConnStr As String)
  gConnStr = sConnStr
End Property
Property Let TimerInterval(iInterval As Integer)
  gInterval = iInterval
End Property

Property Let StartNumber(lStartNo As Long)
  lCardNo = lStartNo
End Property

Property Get Running()
  Running = Not complete
End Property
Public Sub StartCCProcess()
On Error GoTo abort:

conn.Open gConnStr

loops = 0
complete = False
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
End Sub
Private Sub Class_Initialize()

   Set mWorkForm = New WorkForm
   Load mWorkForm
   Set tmrProcessor = mWorkForm.Timer1
   gConnStr = "provider=sqloledb;"
   gInterval = 1000
End Sub

Private Sub Class_Terminate()
  On Error Resume Next
  conn.Close

  Set tmrProcessor = Nothing
  Unload mWorkForm
  Set mWorkForm = Nothing
End Sub

Private Function GetCommand() As String
Dim sCardNo As String

sCardNo = Format(lCardNo, "0000000")
GetCommand = "INSERT INTO TestTransaction" _
 & " values ('37378350" & sCardNo & "' , getdate(), 0.0)"
End Function


Private Sub tmrProcessor_Timer()
On Error GoTo abort:

  cmd = GetCommand()
  conn.Execute "BEGIN TRANSACTION"
  conn.Execute cmd
  conn.Execute "COMMIT TRANSACTION"
  lCardNo = lCardNo + 1
  loops = loops + 1
  If loops > 99999 Then
    tmrProcessor.Enabled = False
    complete = True
  End If

Exit Sub

abort:
  tmrProcessor.Enabled = False
  RaiseEvent ErrorEncountered("Problem encountered during processing.")
End Sub

