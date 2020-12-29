' Author: James William Dunn
' Version: 7.0

Public Class ProcStats
  Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
    Hide()
    Timer1.Enabled = False
  End Sub

  Private Sub Form2_Load(sender As Object, e As EventArgs) Handles Me.Load
    Me.Icon = My.Resources.Proc
    ListView1.Columns.Add("Thread", 75, HorizontalAlignment.Left)
    ListView1.Columns.Add("Current", 75, HorizontalAlignment.Left)
    ListView1.Columns.Add("Minimum", 75, HorizontalAlignment.Left)
    ListView1.Columns.Add("Average", 75, HorizontalAlignment.Left)
    ListView1.Columns.Add("Maximum", 77, HorizontalAlignment.Left)
    ListView1.GridLines = True
    ListView1.FullRowSelect = True
    Dim itemArr(gSize) As ListViewItem
    For i = 0 To gSize - 1
      'ListView1.Items.Add("Thread " & CStr(i) & ": " & gStats(i))
      itemArr(i) = New ListViewItem(CStr(i + 1))
      itemArr(i).SubItems.Add(CStr(gStats(i).cur))
      itemArr(i).SubItems.Add(CStr(gStats(i).min))
      itemArr(i).SubItems.Add(CStr(gStats(i).avg))
      itemArr(i).SubItems.Add(CStr(gStats(i).max))
      ListView1.Items.Add(itemArr(i))
    Next i
    'ListView1.Items.AddRange(itemArr)
    Timer1.Enabled = True
  End Sub

  Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
    For i = 0 To gSize - 1
      'ListView1.Items(i - 1).Text = "Thread " & CStr(i) & ": " & gStats(i)
      ListView1.Items(i).SubItems(1).Text = CStr(gStats(i).cur)
      ListView1.Items(i).SubItems(2).Text = CStr(gStats(i).min)
      ListView1.Items(i).SubItems(3).Text = CStr(gStats(i).avg)
      ListView1.Items(i).SubItems(4).Text = CStr(gStats(i).max)
    Next i
  End Sub

  Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
    Dim str As String = ""

    For i = 0 To gSize - 1
      str = str & ((i + 1).ToString()) & vbTab
      str = str & (CStr(gStats(i).cur)) & vbTab
      str = str & (CStr(gStats(i).min)) & vbTab
      str = str & (CStr(gStats(i).avg)) & vbTab
      str = str & (CStr(gStats(i).max)) & vbCrLf
    Next i
    Clipboard.SetText(str)
  End Sub
End Class