namespace SkyCommanderProcessor {
  partial class frmMain {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
      this.gridFlights = new SourceGrid.Grid();
      this.label1 = new System.Windows.Forms.Label();
      this.txtPendingRecords = new System.Windows.Forms.TextBox();
      this.btnExit = new System.Windows.Forms.Button();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.tabNotifications = new System.Windows.Forms.TabPage();
      this.gridNotifications = new SourceGrid.Grid();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.txtExceptions = new System.Windows.Forms.TextBox();
      this.btnPDF = new System.Windows.Forms.Button();
      this.txtPendingNotifications = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabNotifications.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.SuspendLayout();
      // 
      // gridFlights
      // 
      this.gridFlights.EnableSort = true;
      this.gridFlights.Location = new System.Drawing.Point(3, 7);
      this.gridFlights.Name = "gridFlights";
      this.gridFlights.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
      this.gridFlights.SelectionMode = SourceGrid.GridSelectionMode.Cell;
      this.gridFlights.Size = new System.Drawing.Size(701, 206);
      this.gridFlights.TabIndex = 0;
      this.gridFlights.TabStop = true;
      this.gridFlights.ToolTipText = "";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 265);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(92, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Pending Records:";
      // 
      // txtPendingRecords
      // 
      this.txtPendingRecords.Location = new System.Drawing.Point(111, 262);
      this.txtPendingRecords.Name = "txtPendingRecords";
      this.txtPendingRecords.ReadOnly = true;
      this.txtPendingRecords.Size = new System.Drawing.Size(48, 20);
      this.txtPendingRecords.TabIndex = 2;
      // 
      // btnExit
      // 
      this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnExit.Location = new System.Drawing.Point(645, 260);
      this.btnExit.Name = "btnExit";
      this.btnExit.Size = new System.Drawing.Size(75, 23);
      this.btnExit.TabIndex = 5;
      this.btnExit.Text = "Exit";
      this.btnExit.UseVisualStyleBackColor = true;
      this.btnExit.Click += new System.EventHandler(this.button1_Click);
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabNotifications);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Location = new System.Drawing.Point(12, 12);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(718, 239);
      this.tabControl1.TabIndex = 6;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.gridFlights);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(710, 213);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Main";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // tabNotifications
      // 
      this.tabNotifications.Controls.Add(this.gridNotifications);
      this.tabNotifications.Location = new System.Drawing.Point(4, 22);
      this.tabNotifications.Name = "tabNotifications";
      this.tabNotifications.Padding = new System.Windows.Forms.Padding(3);
      this.tabNotifications.Size = new System.Drawing.Size(710, 213);
      this.tabNotifications.TabIndex = 2;
      this.tabNotifications.Text = "Notifications";
      this.tabNotifications.UseVisualStyleBackColor = true;
      // 
      // gridNotifications
      // 
      this.gridNotifications.EnableSort = true;
      this.gridNotifications.Location = new System.Drawing.Point(5, 3);
      this.gridNotifications.Name = "gridNotifications";
      this.gridNotifications.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
      this.gridNotifications.SelectionMode = SourceGrid.GridSelectionMode.Cell;
      this.gridNotifications.Size = new System.Drawing.Size(701, 206);
      this.gridNotifications.TabIndex = 1;
      this.gridNotifications.TabStop = true;
      this.gridNotifications.ToolTipText = "";
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.txtExceptions);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(710, 213);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Exceptions";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // txtExceptions
      // 
      this.txtExceptions.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtExceptions.Location = new System.Drawing.Point(3, 3);
      this.txtExceptions.Multiline = true;
      this.txtExceptions.Name = "txtExceptions";
      this.txtExceptions.ReadOnly = true;
      this.txtExceptions.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txtExceptions.Size = new System.Drawing.Size(704, 207);
      this.txtExceptions.TabIndex = 0;
      // 
      // btnPDF
      // 
      this.btnPDF.Location = new System.Drawing.Point(564, 262);
      this.btnPDF.Name = "btnPDF";
      this.btnPDF.Size = new System.Drawing.Size(75, 23);
      this.btnPDF.TabIndex = 7;
      this.btnPDF.Text = "PDF Test";
      this.btnPDF.UseVisualStyleBackColor = true;
      this.btnPDF.Click += new System.EventHandler(this.btnPDF_Click);
      // 
      // txtPendingNotifications
      // 
      this.txtPendingNotifications.Location = new System.Drawing.Point(301, 262);
      this.txtPendingNotifications.Name = "txtPendingNotifications";
      this.txtPendingNotifications.ReadOnly = true;
      this.txtPendingNotifications.Size = new System.Drawing.Size(48, 20);
      this.txtPendingNotifications.TabIndex = 9;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(185, 265);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(110, 13);
      this.label2.TabIndex = 8;
      this.label2.Text = "Pending Notifications:";
      // 
      // frmMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnExit;
      this.ClientSize = new System.Drawing.Size(733, 296);
      this.Controls.Add(this.txtPendingNotifications);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.btnPDF);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.btnExit);
      this.Controls.Add(this.txtPendingRecords);
      this.Controls.Add(this.label1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "frmMain";
      this.Text = "SkyCommander Data Processor";
      this.Load += new System.EventHandler(this.frmMain_Load);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabNotifications.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      this.tabPage2.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private SourceGrid.Grid gridFlights;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtPendingRecords;
    private System.Windows.Forms.Button btnExit;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.TextBox txtExceptions;
    private System.Windows.Forms.TabPage tabNotifications;
    private SourceGrid.Grid gridNotifications;
    private System.Windows.Forms.Button btnPDF;
    private System.Windows.Forms.TextBox txtPendingNotifications;
    private System.Windows.Forms.Label label2;
  }
}

