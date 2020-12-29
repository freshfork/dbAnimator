' Author: James William Dunn
' Version: 7.0

Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.Threading

Public Class ProcForm
  Private trd(100) As Thread
  Private state As Integer = 0 ' 0 = Start   1 = Run   2 = Pause
  Dim gRunning As Boolean = True
  Dim gInterval As Integer = 30
  Dim fStat As New ProcStats
  Dim CurThreadID As Integer = 0

  ' When the user clicks...
  Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
    Select Case state
      Case 0 ' START -> RUN
        state = 1 ' Enter RUN state
        Cursor = Cursors.WaitCursor
        Button1.Text = "&Pause"
        Button2.Enabled = True
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
        gSize = CInt(TextBox1.Text) ' Note: first test with a few threads
        ReDim gStats(gSize)
        ReDim trd(gSize)
        For i = 0 To gSize - 1
          CurThreadID = i
          trd(i) = New Thread(AddressOf ThreadTask1)
          trd(i).IsBackground = True
          trd(i).Start()
          Thread.Sleep(97) ' Note: adjust this value to pace the launch
        Next i
        Label1.Text = "Running"
        Cursor = Cursors.Default

      Case 1 ' RUN -> PAUSE
        Button1.Text = "&Resume"
        Label1.Text = "Paused"
        TextBox2.Enabled = True
        state = 2 ' Enter PAUSE state

      Case 2 ' PAUSE -> RUN
        Button1.Text = "&Pause"
        TextBox2.Enabled = False
        gInterval = CInt(TextBox2.Text)
        If gInterval < 16 Then
          gInterval = 16
          TextBox2.Text = "16"
        End If
        Label1.Text = "Running"
        state = 1 ' Enter RUN state
    End Select
  End Sub

  ' Each thread opens a connection to the database and then loops.
  ' If state = 1 (RUN) then it also prepares and submits a commands.
  Private Sub ThreadTask1()
    Dim prng As New Random
    Dim cmdSql As String
    Dim cmd As SqlCommand
    Dim conn As SqlConnection
    Dim lStartTicks, lLoops, lMinTicks, lAverageTicks, lMaxTicks As Long
    Dim lTotalTicks, lDiffTicks As Long
    Dim tid As Integer

    ' Pick up the thread id from the shared data structure
    tid = CurThreadID

    Try
      conn = New SqlConnection("Server=" & TextBox3.Text & ";Database=" & TextBox4.Text &
                             ";User Id=" & TextBox5.Text & ";Password=" & TextBox6.Text &
                             ";max pool size=" & TextBox1.Text & ";")
      conn.Open()
      lLoops = 0
      lMinTicks = 1000000

      While (gRunning)
        If state = 1 Then
          Invoke(
          New Action(
              Sub()
                cmdSql = GetCommand(prng) ' prepare a command
                cmd = conn.CreateCommand
                cmd.CommandText = cmdSql
                lStartTicks = GetTickCount()
                cmd.ExecuteScalar() ' send the command to the server
                ' track statistics
                lDiffTicks = GetTickCount() - lStartTicks
                lLoops = lLoops + 1
                lTotalTicks = lTotalTicks + lDiffTicks
                lAverageTicks = lTotalTicks / lLoops
                If lDiffTicks < lMinTicks Then lMinTicks = lDiffTicks
                If lDiffTicks > lMaxTicks Then lMaxTicks = lDiffTicks
                ' post stats to shared data structure
                gStats(tid).cur = lDiffTicks
                gStats(tid).min = lMinTicks
                gStats(tid).max = lMaxTicks
                gStats(tid).avg = lAverageTicks
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

  ' This function returns a parameterized command (eg. stored procedure or SQL)
  ' Predefined as a 50/50 mix of updates and queries
  ' NOTE: This can be tailored to model different applications.
  Private Function GetCommand(prng As Random) As String
    Dim sCardNo As String
    Dim cmd As String

    ' First, determine four digits 0000-9999
    ' Then append three more digits 000-999
    sCardNo = String.Format("{0:D4}{1:D3}", CLng(prng.NextDouble() * 9999), CLng(prng.NextDouble() * 999))

    ' 50% of the time do a SELECT, otherwise do an UPDATE
    If prng.NextDouble() > 0.5 Then
      cmd = "GetAccount '37378350" & sCardNo & "'"
    Else
      cmd = "UpdateAccount '37378350" & sCardNo & "', " &
      Trim(String.Format("{0:#.00}", 1 + prng.NextDouble() * 100)) & ", '" & Now & "'"
    End If
    'optionally, echo the generated command to the console
    'Console.WriteLine(cmd)
    GetCommand = cmd
  End Function

  ' When the form loads...
  Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load
    Me.Icon = My.Resources.Proc
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

  Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
    ProcStats.Show()
  End Sub
End Class
