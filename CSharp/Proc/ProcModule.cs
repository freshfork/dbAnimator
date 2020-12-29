using System;

namespace Proc
{ 
  static class ProcModule
  {
    [System.Runtime.InteropServices.DllImport("Kernel32")]
    public static extern long GetTickCount64();
    public struct Stat
    {
      public long cur;
      public long min;
      public long max;
      public long avg;
    }
    public static Stat[] gStats;
    public static int gSize;
  }
}