// Author: James William Dunn
// Version: 7.0

using System;
using System.Data.SqlClient;
using System.Threading;

namespace Load
{
  class LoadMain
  {
    static int CurThreadID;
    static bool running =true;
    static int numInstances;
    static int numRunning = 0;
    static int interval;
    static string server;
    static string dbname;
    static string uid;
    static string pwd;
    static int BATCHSIZE = 100000;

    // This function returns an INSERT command
    // NOTE: This can be tailored to model different applications.
    static string GetCommand(int id, long lp)
    {
      string sCardNo;
      string rtn;

      sCardNo = String.Format("{0:D2}{1:D5}", id, lp);

      rtn = "INSERT INTO TestTransaction VALUES('37378350" + sCardNo + "', getdate(), 0.0)";
      return rtn;
    }

    static void ThreadTask1()
    {
      string cmdSql;
      SqlCommand cmd;
      SqlConnection conn;
      long lLoops;
      int tid, tcnt=0;

      // Pick up the thread id from the shared data structure
      tid = CurThreadID;

      try
      {
        conn = new SqlConnection("Server=" + server + ";Database=" + dbname + ";User Id=" + uid + ";Password=" + pwd + ";max pool size=" + numInstances + ";");
        conn.Open();
        lLoops = 0;

        while (running && lLoops < BATCHSIZE)
        {
          cmdSql = GetCommand(tid,lLoops); // prepare a command
          cmd = conn.CreateCommand();
          cmd.CommandText = cmdSql;
          cmd.ExecuteScalar(); // send the command to the server
          lLoops++;
          if (lLoops >= BATCHSIZE)
          {
            numRunning--;
            Console.WriteLine(numRunning + " running");
          }
          Thread.Sleep(interval);
          if(tid==0 && ++tcnt>99)
          {
            Console.WriteLine(cmdSql);
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
          numRunning++;
          trd[i].Start();
          Thread.Sleep(97); // Note: adjust this value to pace the launch
        }
        while (numRunning > 0)
        {
          Thread.Sleep(97); // wait until all threads are finished
        }
        Console.WriteLine("Load complete");
      }
      else
      {
        Console.WriteLine("Usage: load instanceCount interval(ms) server dbname uid pwd");
        Console.WriteLine("Recommend: instanceCount=100 interval(ms)=28");
      }    
      }
  }
}
