' Author: James William Dunn
' Version: 7.0.0.0

Module ProcModule
  Declare Function GetTickCount Lib "Kernel32" () As Long
  Public Structure Stat
    Public cur As Long
    Public min As Long
    Public max As Long
    Public avg As Long
  End Structure
  Public gStats() As Stat
  Public gSize As Integer
End Module
