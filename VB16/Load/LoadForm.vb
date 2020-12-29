' Author: James William Dunn
' Version: 7.0

Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.Threading

Public Class ProcForm
  Private trd(100) As Thread
  Private Shared prng As New Random
  Private state As Integer = 0 ' 0 = Start   1 = Run   2 = Pause
  Private gRunning As Boolean = True
  Private numRunning As Integer = 0
  Private gInterval As Integer = 30
  Private CurThreadID As Integer = 0
  Private BATCHSIZE As Integer = 100000

  ' When the user clicks...
  Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
    Select Case state
      Case 0 ' START -> RUN
        state = 1 ' Enter RUN state
        Cursor = Cursors.WaitCursor
        Button1.Text = "&Pause"
        TextBox1.Enabled = False
        TextBox2.Enabled = False
        TextBox3.Enabled = False
        TextBox4.Enabled = False
        TextBox5.Enabled = False
        TextBox6.Enabled = False
        Label1.Text = "Launching..."
        gInterval = CInt(TextBox2.Text) ' Note: main thread may starve below 16ms
        If gInterval < 16 Then
          gInterval = 16
          TextBox2.Text = "16"
        End If
        Refresh() ' Update the UI before launch
        Dim gSize As Integer = CInt(TextBox1.Text) ' Note: first test with a few threads
        ReDim trd(gSize)
        For i = 0 To gSize - 1
          CurThreadID = i
          trd(i) = New Thread(AddressOf ThreadTask1)
          trd(i).IsBackground = True
          numRunning += 1
          trd(i).Start()
          Thread.Sleep(97) ' Note: adjust this value to pace the launch
        Next i
        Label1.Text = numRunning & " running"
        Cursor = Cursors.Default

      Case 1 ' RUN -> PAUSE
        state = 2 ' Enter PAUSE state
        Button1.Text = "&Resume"
        Label1.Text = "Paused"
        TextBox2.Enabled = True

      Case 2
        state = 1 ' Enter RUN state
        Button1.Text = "&Pause"
        TextBox2.Enabled = False
        gInterval = CInt(TextBox2.Text)
        If gInterval < 16 Then
          gInterval = 16
          TextBox2.Text = "16"
        End If
        Label1.Text = numRunning & " running"
    End Select
  End Sub

  ' Each thread opens a connection to the database and then loops.
  ' If state = 1 (RUN) then it also prepares and submits a commands.
  Private Sub ThreadTask1()
    Dim cmdSql As String
    Dim cmd As SqlCommand
    Dim conn As SqlConnection
    Dim lLoops As Long
    Dim tid As Integer

    ' Pick up the thread id from the shared data structure
    tid = CurThreadID

    Try
      conn = New SqlConnection("Server=" & TextBox3.Text & ";Database=" & TextBox4.Text &
                             ";User Id=" & TextBox5.Text & ";Password=" & TextBox6.Text &
                             ";max pool size=" & TextBox1.Text & ";")
      conn.Open()
      lLoops = 0

      While (gRunning And lLoops < BATCHSIZE)
        If state = 1 Then
          Invoke(
          New Action(
              Sub()
                cmdSql = GetCommand(tid, lLoops) ' prepare a command
                cmd = conn.CreateCommand
                cmd.CommandText = cmdSql
                cmd.ExecuteScalar() ' send the command to the server
                lLoops += 1
                If (lLoops >= BATCHSIZE) Then
                  numRunning -= 1
                  Label1.Text = numRunning & " running"
                End If
              End Sub
              )
          )
        End If
        Thread.Sleep(gInterval)
      End While
      conn.Close()
      ' If anything goes wrong, shut down
    Catch e As Exception
      gRunning = False
      state = 0
      MsgBox("Exception caught: " + e.Message)
      End
    End Try
  End Sub

  ' This function returns an INSERT command
  ' NOTE: This can be tailored to model different applications.
  Private Function GetCommand(id As Integer, lp As Long) As String
    Dim sCardNo As String

    sCardNo = String.Format("{0:D2}{1:D5}", id, lp)
    GetCommand = "INSERT INTO TestTransaction VALUES('37378350" & sCardNo & "', getdate(), 0.0)"
  End Function

  ' When the form loads...
  Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
    Me.Icon = My.Resources.Load
  End Sub

  ' When the form closes...
  Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
    If gRunning Then
      gRunning = False
      Label1.Text = "Terminating..."
      Refresh()
      Application.DoEvents() ' Yield for a moment to allow thread shutdown
      Thread.Sleep(1000)
    End If
  End Sub

End Class
