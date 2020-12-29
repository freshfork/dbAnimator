// Author: James William Dunn
// Version: 7.0

using System;
using System.Windows.Forms;

namespace Proc
{
  public partial class ProcStats : Form
  {
    public ProcStats()
    {
      InitializeComponent();
      Timer1.Interval = 5000;
      Timer1.Tick += new EventHandler(Timer1_Tick);
    }

    private void ProcStats_VisibleChanged(Object sender, EventArgs e)
    {
      if(this.Visible) Timer1.Enabled = true;
    }
    private void ProcStats_Load(object sender, EventArgs e)
    {
      this.Icon = Properties.Resources.Proc;
      ListView1.Columns.Add("Thread", 75, HorizontalAlignment.Left);
      ListView1.Columns.Add("Current", 75, HorizontalAlignment.Left);
      ListView1.Columns.Add("Minimum", 75, HorizontalAlignment.Left);
      ListView1.Columns.Add("Average", 75, HorizontalAlignment.Left);
      ListView1.Columns.Add("Maximum", 77, HorizontalAlignment.Left);
      ListView1.GridLines = true;
      ListView1.FullRowSelect = true;
      ListViewItem[] itemArr = new ListViewItem[ProcModule.gSize + 1];
      for (var i = 0; i < ProcModule.gSize; i++)
      {
        itemArr[i] = new ListViewItem(Convert.ToString(i + 1));
        itemArr[i].SubItems.Add(Convert.ToString(ProcModule.gStats[i].cur));
        itemArr[i].SubItems.Add(Convert.ToString(ProcModule.gStats[i].min));
        itemArr[i].SubItems.Add(Convert.ToString(ProcModule.gStats[i].avg));
        itemArr[i].SubItems.Add(Convert.ToString(ProcModule.gStats[i].max));
        ListView1.Items.Add(itemArr[i]);
      }
    }

    private void Timer1_Tick(object sender, EventArgs e)
    {
      for (var i = 0; i < ProcModule.gSize; i++)
      {
        ListView1.Items[i].SubItems[1].Text = Convert.ToString(ProcModule.gStats[i].cur);
        ListView1.Items[i].SubItems[2].Text = Convert.ToString(ProcModule.gStats[i].min);
        ListView1.Items[i].SubItems[3].Text = Convert.ToString(ProcModule.gStats[i].avg);
        ListView1.Items[i].SubItems[4].Text = Convert.ToString(ProcModule.gStats[i].max);
      }
    }

    private void OKButton_Click(object sender, EventArgs e)
    {
      Hide();
      Timer1.Enabled = false;
    }
    private void ProcStats_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing)
      {
        e.Cancel = true;
        Hide();
        Timer1.Enabled = false;
      }
    }

    private void CopyButton_Click(object sender, EventArgs e)
    {
      string str = "";

      for (var i = 0; i <= ProcModule.gSize - 1; i++)
      {
        str = str + ((i + 1).ToString()) + "\t";
        str = str + (Convert.ToString(ProcModule.gStats[i].cur)) + "\t";
        str = str + (Convert.ToString(ProcModule.gStats[i].min)) + "\t";
        str = str + (Convert.ToString(ProcModule.gStats[i].avg)) + "\t";
        str = str + (Convert.ToString(ProcModule.gStats[i].max)) + "\r\n";
      }
      Clipboard.SetText(str);
    }
  }
}
