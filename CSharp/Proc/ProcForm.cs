// Author: James William Dunn
// Version: 7.0

using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Threading;

namespace Proc
{
    public partial class ProcForm : Form
    {
      private Thread[] trd = new Thread[101];
      private int state = 0; // 0 = Start   1 = Run   2 = Pause
      private bool gRunning = true;
      private int gInterval = 30;
      private ProcStats fStat = new ProcStats();
      private int CurThreadID = 0;

    public ProcForm()
    {
        InitializeComponent();
    }

    private void ThreadTask1()
    {
      Random prng = new Random();
      string cmdSql;
      SqlCommand cmd;
      SqlConnection conn;
      long lStartTicks, lLoops, lMinTicks, lAverageTicks, lMaxTicks=0;
      long lTotalTicks=0, lDiffTicks;
      int tid;

      // Pick up the thread id from the shared data structure
      tid = CurThreadID;

      try
      {
        conn = new SqlConnection("Server=" + TextBox3.Text + ";Database=" + TextBox4.Text + ";User Id=" + TextBox5.Text + ";Password=" + TextBox6.Text + ";max pool size=" + TextBox1.Text + ";");
        conn.Open();
        lLoops = 0;
        lMinTicks = 1000000;

        while ((gRunning))
        {
          if (state == 1)
          {
            Invoke(new Action(() =>
            {
              cmdSql = GetCommand(prng); // prepare a command
              cmd = conn.CreateCommand();
              cmd.CommandText = cmdSql;
              lStartTicks = ProcModule.GetTickCount64();
              cmd.ExecuteScalar(); // send the command to the server
              // track statistics
              lDiffTicks = ProcModule.GetTickCount64() - lStartTicks;
              lLoops = lLoops + 1;
              if(tid==0 && lLoops % 100 == 0) Console.WriteLine(cmdSql);
              lTotalTicks = lTotalTicks + lDiffTicks;
              lAverageTicks = (long)(lTotalTicks / (double)lLoops);
              if (lDiffTicks < lMinTicks)
                lMinTicks = lDiffTicks;
              if (lDiffTicks > lMaxTicks)
                lMaxTicks = lDiffTicks;
              // post stats to shared data structure
              ProcModule.gStats[tid].cur = lDiffTicks;
              ProcModule.gStats[tid].min = lMinTicks;
              ProcModule.gStats[tid].max = lMaxTicks;
              ProcModule.gStats[tid].avg = lAverageTicks;
            }
)
);
          }
          Thread.Sleep(gInterval);
        }
        conn.Close();
      }
      // If anything goes wrong, shut down
      catch (Exception e)
      {
        gRunning = false;
        state = 0;
        MessageBox.Show("Exception caught: " + e.Message);
        Environment.Exit(0);
      }
    }


    // This function returns a parameterized command (eg. stored procedure or SQL)
    // Predefined as a 50/50 mix of updates and queries
    // NOTE: This can be tailored to model different applications.
    private string GetCommand(Random prng)
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
          String.Format("{0:#.00}", 1 + prng.NextDouble()*100) + ", '" + DateTime.Now + "'";
      // optionally, echo the generated command to the console
      // Console.WriteLine(cmd);
      return cmd;
    }

    // When the form loads...
    private void Form1_Load(Object sender, System.EventArgs e)
    {
      this.Icon = Properties.Resources.Proc;
    }

    // When the form closes...
    private void ProcForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (gRunning)
      {
        gRunning = false;
        Label1.Text = "Terminating...";
        Refresh();
        Application.DoEvents(); // Yield for a moment to allow thread shutdown
        Thread.Sleep(1000);
      }
    }

    private void Button1_Click(object sender, EventArgs e)
    {
      switch (state)
      {
        case 0: // START -> RUN
          {
            state = 1; // Enter RUN state
            Cursor = Cursors.WaitCursor;
            Button1.Text = "&Pause";
            Button2.Enabled = true;
            TextBox1.Enabled = false;
            TextBox2.Enabled = false;
            TextBox3.Enabled = false;
            TextBox4.Enabled = false;
            TextBox5.Enabled = false;
            TextBox6.Enabled = false;
            Label1.Text = "Launching...";
            gInterval = Convert.ToInt32(TextBox2.Text);
            // Note: main thread may starve below 16ms (adjust the following with care!)
            if (gInterval < 16)
            {
              TextBox2.Text = "16";
              gInterval = 16;
            }
            Refresh(); // Update the UI before launch
            ProcModule.gSize = Convert.ToInt32(TextBox1.Text); // Note: first test with a few threads
            Array.Resize(ref ProcModule.gStats, ProcModule.gSize);
            Array.Resize(ref trd, ProcModule.gSize);
            for (var i = 0; i <= ProcModule.gSize - 1; i++)
            {
              CurThreadID = i;
              trd[i] = new Thread(ThreadTask1);
              trd[i].IsBackground = true;
              trd[i].Start();
              Thread.Sleep(97); // Note: adjust this value to pace the launch
            }
            Label1.Text = "Running";
            Cursor = Cursors.Default;
            break;
          }

        case 1: // RUN -> PAUSE
          {
            state = 2; // Enter PAUSE state
            Button1.Text = "&Resume";
            Label1.Text = "Paused";
            TextBox2.Enabled = true;
            break;
          }

        case 2: // PAUSE -> RUN
          {
            state = 1; // Enter RUN state
            Button1.Text = "&Pause";
            TextBox2.Enabled = false;
            gInterval = System.Convert.ToInt32(TextBox2.Text);
            // Note: main thread may starve below 16ms (adjust the following with care!)
            if (gInterval < 16)
            {
              TextBox2.Text = "16";
              gInterval = 16;
            }
            Label1.Text = "Running";
            break;
          }
      }
    }

    private void Button2_Click(object sender, EventArgs e)
    {
      fStat.Show();
    }
  }
}
