// Author: James William Dunn
// Version: 7.0

using System;
using System.Data.SqlClient;
using System.Threading;

namespace Proc
{
  class ProcMain
  {
    static int CurThreadID;
    static bool running =true;
    static int numInstances;
    static int interval;
    static string server;
    static string dbname;
    static string uid;
    static string pwd;

    static string GetCommand(Random prng)
    {
      string sCardNo;
      string cmd;

      // First, determine four digits 0000-9999
      // Then append three more digits 000-999
      sCardNo = String.Format("{0:D4}{1:D3}", (long)(prng.NextDouble() * 9999), (long)(prng.NextDouble() * 999));

      // 50% of the time do a SELECT, otherwise do an UPDATE
      if (prng.NextDouble() > 0.5)
        cmd = "GetAccount '37378350" + sCardNo + "'";
      else
        cmd = "UpdateAccount '37378350" + sCardNo + "', " +
        String.Format("{0:#.00}", 1 + prng.NextDouble() * 100) + ", '" + DateTime.Now + "'";
      return cmd;
    }


    static void ThreadTask1()
    {
      string cmdSql;
      SqlCommand cmd;
      SqlConnection conn;
      long lStartTicks, lLoops, lMinTicks, lAverageTicks, lMaxTicks = 0;
      long lTotalTicks = 0, lDiffTicks;
      int tid, tcnt=0;
      Random prng = new Random();

      // Pick up the thread id from the shared data structure
      tid = CurThreadID;

      try
      {
        conn = new SqlConnection("Server=" + server + ";Database=" + dbname + ";User Id=" + uid + ";Password=" + pwd + ";max pool size=" + numInstances + ";");
        conn.Open();
        lLoops = 0;
        lMinTicks = 1000000;

        while ((running))
        {
          cmdSql = GetCommand(prng); // prepare a command
          cmd = conn.CreateCommand();
          cmd.CommandText = cmdSql;
          lStartTicks = DateTimeOffset.Now.ToUnixTimeMilliseconds();
          cmd.ExecuteScalar(); // send the command to the server

          // track statistics
          lDiffTicks =  DateTimeOffset.Now.ToUnixTimeMilliseconds() - lStartTicks;
          lLoops = lLoops + 1;
          lTotalTicks = lTotalTicks + lDiffTicks;
          lAverageTicks = (long)(lTotalTicks / (double)lLoops);
          if (lDiffTicks < lMinTicks)
            lMinTicks = lDiffTicks;
          if (lDiffTicks > lMaxTicks)
            lMaxTicks = lDiffTicks;
          // Stats are as follows
          // current: lDiffTicks
          // minimum: lMinTicks
          // average: lAverageTicks
          // maximum: lMaxTicks

          Thread.Sleep(interval);
          if(tid==0 && ++tcnt>100)
          {
            Console.Write(lDiffTicks);
            Console.WriteLine("ms: "+cmdSql);
            tcnt = 0;
          }
        }
        conn.Close();
      }
      // If anything goes wrong, shut down
      catch (Exception e)
      {
        running = false;
        Console.WriteLine("Exception caught: " + e.Message);
        Environment.Exit(0);
      }
    }

    static void Main(string[] args)
    {
      Thread[] trd = new Thread[101];

      if (args.Length == 6)
      {
        numInstances = Convert.ToInt32(args[0]);
        interval = Convert.ToInt32(args[1]);
        // Note: caution setting this below 16ms (adjust the following with care!)
        if (interval < 16) interval = 16;
        server = args[2];
        dbname = args[3];
        uid = args[4];
        pwd = args[5];
        Array.Resize(ref trd, numInstances);

        Console.WriteLine("Launching...");
        for (var i = 0; i <= numInstances - 1; i++)
        {
          CurThreadID = i;
          trd[i] = new Thread(ThreadTask1);
          trd[i].IsBackground = true;
          trd[i].Start();
          Thread.Sleep(97); // Note: adjust this value to pace the launch
        }
        Console.WriteLine("Running! Press <return> to terminate");
        Console.ReadLine();
      }
      else Console.WriteLine("Usage: proc instanceCount interval(ms) server dbname uid pwd");
    }
  }
}