// Author: James William Dunn
// Version: 7.0

using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Threading;

namespace Load
{
    public partial class LoadForm : Form
    {
    private Thread[] trd = new Thread[100];
    private int state = 0; // 0 = Start   1 = Run   2 = Pause
    private bool gRunning = true;
    private int numRunning = 0;
    private int gInterval = 30;
    private int CurThreadID = 0;
    private int BATCHSIZE = 100000;

    public LoadForm()
    {
        InitializeComponent();
    }

    private void ThreadTask1()
    {
      string cmdSql;
      SqlCommand cmd;
      SqlConnection conn;
      long lLoops;
      int tid;

      // Pick up the thread id from the shared data structure
      tid = CurThreadID;

      try
      {
        conn = new SqlConnection("Server=" + TextBox3.Text + ";Database=" + TextBox4.Text + ";User Id=" + TextBox5.Text + ";Password=" + TextBox6.Text + ";max pool size=" + TextBox1.Text + ";");
        conn.Open();
        lLoops = 0;

        while (gRunning && lLoops < BATCHSIZE)
        {
          if (state == 1)
          {
            Invoke(new Action(() =>
            {
              cmdSql = GetCommand(tid,lLoops); // prepare a command
              cmd = conn.CreateCommand();
              cmd.CommandText = cmdSql;
              cmd.ExecuteScalar(); // send the command to the server
              lLoops++;
              if (lLoops >= BATCHSIZE)
              {
                numRunning--;
                Label1.Text = numRunning + " running";
              }
            }));
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


    // This function returns an INSERT command
    // NOTE: This can be tailored to model different applications.
    private string GetCommand(int id, long lp)
    {
      string sCardNo;
      string rtn;

      sCardNo = String.Format("{0:D2}{1:D5}", id, lp);

      rtn = "INSERT INTO TestTransaction VALUES('37378350" + sCardNo + "', getdate(), 0.0)";
      return rtn;
    }

    // When the form loads...
    private void Form1_Load(Object sender, System.EventArgs e)
    {
      this.Icon = Properties.Resources.Load;
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
            TextBox1.Enabled = false;
            TextBox2.Enabled = false;
            TextBox3.Enabled = false;
            TextBox4.Enabled = false;
            TextBox5.Enabled = false;
            TextBox6.Enabled = false;
            Label1.Text = "Launching...";
            gInterval = Convert.ToInt32(TextBox2.Text); // Note: main thread may starve below 16ms
            if (gInterval < 16)
            {
              gInterval = 16;
              TextBox2.Text = "16";
            }
            Refresh(); // Update the UI before launch
            int gSize = Convert.ToInt32(TextBox1.Text); // Note: first test with a few threads
            Array.Resize(ref trd, gSize);
            for (var i = 0; i <= gSize - 1; i++)
            {
              CurThreadID = i;
              trd[i] = new Thread(ThreadTask1);
              trd[i].IsBackground = true;
              numRunning++;
              trd[i].Start();
              Thread.Sleep(97); // Note: adjust this value to pace the launch
            }
            Label1.Text = numRunning+" running";
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
            if (gInterval < 16)
            {
              gInterval = 16;
              TextBox2.Text = "16";
            }
            Label1.Text = numRunning + " running";
            break;
          }
      }
    }
  }
}
