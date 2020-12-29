
namespace Proc
{
  partial class ProcStats
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.CopyButton = new System.Windows.Forms.Button();
      this.ListView1 = new System.Windows.Forms.ListView();
      this.OKButton = new System.Windows.Forms.Button();
      this.Timer1 = new System.Windows.Forms.Timer(this.components);
      this.SuspendLayout();
      // 
      // CopyButton
      // 
      this.CopyButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
      this.CopyButton.Location = new System.Drawing.Point(416, 42);
      this.CopyButton.Name = "CopyButton";
      this.CopyButton.Size = new System.Drawing.Size(78, 24);
      this.CopyButton.TabIndex = 4;
      this.CopyButton.Text = "&Copy all";
      this.CopyButton.UseVisualStyleBackColor = true;
      this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
      // 
      // ListView1
      // 
      this.ListView1.Location = new System.Drawing.Point(12, 12);
      this.ListView1.Name = "ListView1";
      this.ListView1.Size = new System.Drawing.Size(398, 476);
      this.ListView1.TabIndex = 3;
      this.ListView1.UseCompatibleStateImageBehavior = false;
      this.ListView1.View = System.Windows.Forms.View.Details;
      // 
      // OKButton
      // 
      this.OKButton.Location = new System.Drawing.Point(416, 12);
      this.OKButton.Name = "OKButton";
      this.OKButton.Size = new System.Drawing.Size(78, 24);
      this.OKButton.TabIndex = 2;
      this.OKButton.Text = "OK";
      this.OKButton.UseVisualStyleBackColor = true;
      this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
      // 
      // ProcStats
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(503, 500);
      this.Controls.Add(this.CopyButton);
      this.Controls.Add(this.ListView1);
      this.Controls.Add(this.OKButton);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ProcStats";
      this.Text = " Client Statistics";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProcStats_FormClosing);
      this.Load += new System.EventHandler(this.ProcStats_Load);
      this.VisibleChanged += new System.EventHandler(this.ProcStats_VisibleChanged);
      this.ResumeLayout(false);

    }

    #endregion

    internal System.Windows.Forms.Button CopyButton;
    internal System.Windows.Forms.ListView ListView1;
    internal System.Windows.Forms.Button OKButton;
    private System.Windows.Forms.Timer Timer1;
  }
}