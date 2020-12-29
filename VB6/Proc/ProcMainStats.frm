VERSION 5.00
Object = "{831FDD16-0C5C-11D2-A9FC-0000F8754DA1}#2.2#0"; "MSCOMCTL.OCX"
Begin VB.Form Form2 
   BorderStyle     =   1  'Fixed Single
   Caption         =   " Client Statistics"
   ClientHeight    =   7335
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   7440
   Icon            =   "ProcMainStats.frx":0000
   LinkTopic       =   "Form2"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   7335
   ScaleWidth      =   7440
   StartUpPosition =   3  'Windows Default
   Begin VB.Timer Timer1 
      Enabled         =   0   'False
      Interval        =   5000
      Left            =   6360
      Top             =   2640
   End
   Begin MSComctlLib.ListView ListView1 
      Height          =   7095
      Left            =   120
      TabIndex        =   2
      Top             =   120
      Width           =   5895
      _ExtentX        =   10398
      _ExtentY        =   12515
      View            =   3
      LabelWrap       =   -1  'True
      HideSelection   =   -1  'True
      FullRowSelect   =   -1  'True
      GridLines       =   -1  'True
      _Version        =   393217
      ForeColor       =   -2147483640
      BackColor       =   -2147483643
      BorderStyle     =   1
      Appearance      =   1
      NumItems        =   0
   End
   Begin VB.CommandButton Command2 
      Caption         =   "&Copy all"
      Height          =   375
      Left            =   6120
      TabIndex        =   1
      Top             =   600
      Width           =   1215
   End
   Begin VB.CommandButton Command1 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   375
      Left            =   6120
      TabIndex        =   0
      Top             =   120
      Width           =   1215
   End
End
Attribute VB_Name = "Form2"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
' Author: James William Dunn
' Version: 7.0

Private Sub Command1_Click()
  Timer1.Enabled = False
  Unload Me
End Sub

Private Sub Command2_Click()
    Dim str As String
    str = ""
    For i = 1 To gSize
      str = str & i & vbTab
      str = str & (CStr(gStats(i).cur)) & vbTab
      str = str & (CStr(gStats(i).min)) & vbTab
      str = str & (CStr(gStats(i).avg)) & vbTab
      str = str & (CStr(gStats(i).max)) & vbCrLf
    Next i
    Clipboard.Clear
    Clipboard.SetText str
End Sub

Private Sub Form_Load()
    Dim clmX As ColumnHeader
    Set clmX = ListView1.ColumnHeaders.Add(, , "Worker", 750)
    Set clmX = ListView1.ColumnHeaders.Add(, , "Current", 1200)
    Set clmX = ListView1.ColumnHeaders.Add(, , "Minimum", 1200)
    Set clmX = ListView1.ColumnHeaders.Add(, , "Average", 1200)
    Set clmX = ListView1.ColumnHeaders.Add(, , "Maximum", 1200)
    Dim itmX As ListItem
        Debug.Print DoEvents()
  For i = 1 To gSize
    Set itmX = ListView1.ListItems.Add(, , CStr(i))
    itmX.SubItems(1) = CStr(gStats(i).cur)
    itmX.SubItems(2) = CStr(gStats(i).min)
    itmX.SubItems(3) = CStr(gStats(i).avg)
    itmX.SubItems(4) = CStr(gStats(i).max)
  Next i
  Timer1.Enabled = True
End Sub

Private Sub Timer1_Timer()
  Dim itm As ListItem
  Set itm = ListView1.GetFirstVisible
  Dim idx As Integer
  idx = itm.Index
  ListView1.ListItems.Clear
  For i = 1 To gSize
    Set itmX = ListView1.ListItems.Add(, , CStr(i))
    itmX.SubItems(1) = CStr(gStats(i).cur)
    itmX.SubItems(2) = CStr(gStats(i).min)
    itmX.SubItems(3) = CStr(gStats(i).avg)
    itmX.SubItems(4) = CStr(gStats(i).max)
  Next i
  If gSize > 32 Then
    ListView1.ListItems.Item(idx + 31).EnsureVisible
  End If
End Sub
