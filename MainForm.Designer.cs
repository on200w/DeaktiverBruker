partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private DeaktiverBruker.Controls.ThemedDatePicker datePicker;
    private DeaktiverBruker.Controls.ThemedTimePicker timePicker;
    private System.Windows.Forms.Button btnSchedule;
    private System.Windows.Forms.Button btnTest;
    private System.Windows.Forms.Button btnActivate;
    private System.Windows.Forms.Button btnOpenScheduler;
    private System.Windows.Forms.PictureBox btnSettings;
    private System.Windows.Forms.Label lblDate;
    private System.Windows.Forms.Label lblTime;
    private System.Windows.Forms.Panel headerPanel;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblSubtitle;
    private System.Windows.Forms.Panel cardSchedule;
    private System.Windows.Forms.Label lblScheduleSection;
    private System.Windows.Forms.Label lblScheduleHelp;
    private System.Windows.Forms.Panel cardMaintenance;
    private System.Windows.Forms.Label lblMaintenanceSection;
    private System.Windows.Forms.RichTextBox txtStatus;
    private System.Windows.Forms.Label lblCredit;

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
        this.datePicker = new DeaktiverBruker.Controls.ThemedDatePicker();
        this.timePicker = new DeaktiverBruker.Controls.ThemedTimePicker();
        this.btnSchedule = new System.Windows.Forms.Button();
        this.btnTest = new System.Windows.Forms.Button();
        this.btnActivate = new System.Windows.Forms.Button();
        this.btnOpenScheduler = new System.Windows.Forms.Button();
        this.btnSettings = new System.Windows.Forms.PictureBox();
        this.lblDate = new System.Windows.Forms.Label();
        this.lblTime = new System.Windows.Forms.Label();
        this.headerPanel = new System.Windows.Forms.Panel();
        this.lblTitle = new System.Windows.Forms.Label();
        this.lblSubtitle = new System.Windows.Forms.Label();
        this.cardSchedule = new System.Windows.Forms.Panel();
        this.lblScheduleSection = new System.Windows.Forms.Label();
        this.lblScheduleHelp = new System.Windows.Forms.Label();
        this.cardMaintenance = new System.Windows.Forms.Panel();
        this.lblMaintenanceSection = new System.Windows.Forms.Label();
        this.txtStatus = new System.Windows.Forms.RichTextBox();
        this.lblCredit = new System.Windows.Forms.Label();
        this.headerPanel.SuspendLayout();
        this.cardSchedule.SuspendLayout();
        this.cardMaintenance.SuspendLayout();
        this.SuspendLayout();
        
        // 
        // headerPanel
        //
        this.headerPanel.BackColor = System.Drawing.Color.FromArgb(28, 28, 28);
        this.headerPanel.Controls.Add(this.lblTitle);
        this.headerPanel.Controls.Add(this.lblSubtitle);
        this.headerPanel.Controls.Add(this.btnSettings);
        this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
        this.headerPanel.Location = new System.Drawing.Point(0, 0);
        this.headerPanel.Name = "headerPanel";
        this.headerPanel.Size = new System.Drawing.Size(608, 92);
        this.headerPanel.TabIndex = 0;
        
        // 
        // lblTitle
        // 
        this.lblTitle.AutoSize = true;
        this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
        this.lblTitle.ForeColor = System.Drawing.Color.White;
        this.lblTitle.Location = new System.Drawing.Point(24, 12);
        this.lblTitle.Name = "lblTitle";
        this.lblTitle.Size = new System.Drawing.Size(192, 32);
        this.lblTitle.TabIndex = 0;
        this.lblTitle.Text = "DeaktiverBruker";
        
        // 
        // lblSubtitle
        // 
        this.lblSubtitle.AutoSize = true;
        this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10.5F);
        this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
        this.lblSubtitle.Location = new System.Drawing.Point(28, 50);
        this.lblSubtitle.Name = "lblSubtitle";
        this.lblSubtitle.Size = new System.Drawing.Size(322, 19);
        this.lblSubtitle.TabIndex = 1;
        this.lblSubtitle.Text = "Sett lånetid for PC og administrer planlagte jobber";
        
        // 
        // cardSchedule
        // 
        this.cardSchedule.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
        this.cardSchedule.Controls.Add(this.lblScheduleSection);
        this.cardSchedule.Controls.Add(this.lblScheduleHelp);
        this.cardSchedule.Controls.Add(this.lblDate);
        this.cardSchedule.Controls.Add(this.datePicker);
        this.cardSchedule.Controls.Add(this.lblTime);
        this.cardSchedule.Controls.Add(this.timePicker);
        this.cardSchedule.Controls.Add(this.btnSchedule);
        this.cardSchedule.Controls.Add(this.btnTest);
        this.cardSchedule.Location = new System.Drawing.Point(16, 100);
        this.cardSchedule.Name = "cardSchedule";
        this.cardSchedule.Padding = new System.Windows.Forms.Padding(20);
        this.cardSchedule.Size = new System.Drawing.Size(576, 152);
        this.cardSchedule.TabIndex = 1;
        
        // 
        // lblScheduleSection
        // 
        this.lblScheduleSection.AutoSize = true;
        this.lblScheduleSection.Font = new System.Drawing.Font("Segoe UI Semibold", 11.5F);
        this.lblScheduleSection.ForeColor = System.Drawing.Color.White;
        this.lblScheduleSection.Location = new System.Drawing.Point(16, 16);
        this.lblScheduleSection.Name = "lblScheduleSection";
        this.lblScheduleSection.Size = new System.Drawing.Size(174, 20);
        this.lblScheduleSection.TabIndex = 0;
        this.lblScheduleSection.Text = "Planlegg deaktivering";
        
        // 
        // lblScheduleHelp
        // 
        this.lblScheduleHelp.AutoSize = false;
        this.lblScheduleHelp.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblScheduleHelp.ForeColor = System.Drawing.Color.FromArgb(160, 160, 160);
        this.lblScheduleHelp.Location = new System.Drawing.Point(16, 40);
        this.lblScheduleHelp.Name = "lblScheduleHelp";
        this.lblScheduleHelp.Size = new System.Drawing.Size(520, 18);
        this.lblScheduleHelp.TabIndex = 1;
        this.lblScheduleHelp.Text = "";
        this.lblScheduleHelp.Visible = false;
        
        // 
        // lblDate
        // 
        this.lblDate.AutoSize = true;
        this.lblDate.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.lblDate.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
        this.lblDate.Location = new System.Drawing.Point(24, 64);
        this.lblDate.Name = "lblDate";
        this.lblDate.Size = new System.Drawing.Size(38, 17);
        this.lblDate.TabIndex = 1;
        this.lblDate.Text = "Dato:";
        
        // 
        // datePicker
        // 
        this.datePicker.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.datePicker.Location = new System.Drawing.Point(120, 60);
        this.datePicker.Name = "datePicker";
        this.datePicker.Size = new System.Drawing.Size(160, 25);
        this.datePicker.TabIndex = 2;
        
        // 
        // lblTime
        // 
        this.lblTime.AutoSize = true;
        this.lblTime.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.lblTime.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
        this.lblTime.Location = new System.Drawing.Point(24, 100);
        this.lblTime.Name = "lblTime";
        this.lblTime.Size = new System.Drawing.Size(73, 17);
        this.lblTime.TabIndex = 3;
        this.lblTime.Text = "Klokkeslett:";
        
        // 
        // timePicker
        // 
        this.timePicker.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.timePicker.Location = new System.Drawing.Point(120, 96);
        this.timePicker.Name = "timePicker";
        
        this.timePicker.Size = new System.Drawing.Size(100, 25);
        this.timePicker.TabIndex = 4;
        
        // 
        // btnSchedule
        // 
        this.btnSchedule.BackColor = System.Drawing.Color.FromArgb(35, 161, 84);
        this.btnSchedule.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnSchedule.FlatAppearance.BorderSize = 0;
        this.btnSchedule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnSchedule.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.btnSchedule.ForeColor = System.Drawing.Color.White;
        this.btnSchedule.Location = new System.Drawing.Point(344, 40);
        this.btnSchedule.Name = "btnSchedule";
        this.btnSchedule.Size = new System.Drawing.Size(220, 42);
        this.btnSchedule.TabIndex = 5;
        this.btnSchedule.Text = "Planlegg deaktivering";
        this.btnSchedule.UseVisualStyleBackColor = false;
        this.btnSchedule.Click += new System.EventHandler(this.btnSchedule_Click);
        // 
        // btnTest
        // 
        this.btnTest.BackColor = System.Drawing.Color.FromArgb(0, 120, 212);
        this.btnTest.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnTest.FlatAppearance.BorderSize = 0;
        this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnTest.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.btnTest.ForeColor = System.Drawing.Color.White;
        this.btnTest.Location = new System.Drawing.Point(344, 88);
        this.btnTest.Name = "btnTest";
        this.btnTest.Size = new System.Drawing.Size(220, 42);
        this.btnTest.TabIndex = 6;
        this.btnTest.Text = "Test / Simuler";
        this.btnTest.UseVisualStyleBackColor = false;
        this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
        
        // 
        // cardMaintenance
        // 
        this.cardMaintenance.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
        this.cardMaintenance.Controls.Add(this.lblMaintenanceSection);
        this.cardMaintenance.Controls.Add(this.btnActivate);
        this.cardMaintenance.Controls.Add(this.btnOpenScheduler);
        this.cardMaintenance.Location = new System.Drawing.Point(16, 264);
        this.cardMaintenance.Name = "cardMaintenance";
        this.cardMaintenance.Padding = new System.Windows.Forms.Padding(20);
        this.cardMaintenance.Size = new System.Drawing.Size(576, 124);
        this.cardMaintenance.TabIndex = 2;
        
        // 
        // lblMaintenanceSection
        // 
        this.lblMaintenanceSection.AutoSize = true;
        this.lblMaintenanceSection.Font = new System.Drawing.Font("Segoe UI Semibold", 11.5F);
        this.lblMaintenanceSection.ForeColor = System.Drawing.Color.White;
        this.lblMaintenanceSection.Location = new System.Drawing.Point(16, 16);
        this.lblMaintenanceSection.Name = "lblMaintenanceSection";
        this.lblMaintenanceSection.Size = new System.Drawing.Size(93, 20);
        this.lblMaintenanceSection.TabIndex = 0;
        this.lblMaintenanceSection.Text = "Vedlikehold";
        
        // 
        // btnActivate
        // 
        this.btnActivate.BackColor = System.Drawing.Color.FromArgb(30, 130, 30);
        this.btnActivate.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnActivate.FlatAppearance.BorderSize = 0;
        this.btnActivate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnActivate.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.btnActivate.ForeColor = System.Drawing.Color.White;
        this.btnActivate.Location = new System.Drawing.Point(64, 56);
        this.btnActivate.Name = "btnActivate";
        this.btnActivate.Size = new System.Drawing.Size(220, 40);
        this.btnActivate.TabIndex = 1;
        this.btnActivate.Text = "Aktiver bruker og rydd opp";
        this.btnActivate.UseVisualStyleBackColor = false;
        this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
        
        // 
        // btnOpenScheduler
        // 
        this.btnOpenScheduler.BackColor = System.Drawing.Color.FromArgb(70, 70, 70);
        this.btnOpenScheduler.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnOpenScheduler.FlatAppearance.BorderSize = 0;
        this.btnOpenScheduler.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnOpenScheduler.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.btnOpenScheduler.ForeColor = System.Drawing.Color.White;
        this.btnOpenScheduler.Location = new System.Drawing.Point(320, 56);
        this.btnOpenScheduler.Name = "btnOpenScheduler";
        this.btnOpenScheduler.Size = new System.Drawing.Size(220, 40);
        this.btnOpenScheduler.TabIndex = 2;
        this.btnOpenScheduler.Text = "Åpne Oppgaveplanlegger";
        this.btnOpenScheduler.UseVisualStyleBackColor = false;
        this.btnOpenScheduler.Click += new System.EventHandler(this.btnOpenScheduler_Click);
        // 
        // btnSettings (picturebox used for crisp transparent icon)
        // 
        this.btnSettings.BackColor = System.Drawing.Color.Transparent;
        this.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
        // place in top-right corner of headerPanel and anchor to right
        this.btnSettings.Location = new System.Drawing.Point(552, 28);
        this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnSettings.Name = "btnSettings";
        this.btnSettings.Size = new System.Drawing.Size(36, 36);
        this.btnSettings.TabIndex = 3;
        this.btnSettings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
        
        // 
        // txtStatus
        // 
        this.txtStatus.BackColor = System.Drawing.Color.FromArgb(28, 28, 28);
        this.txtStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.txtStatus.Font = new System.Drawing.Font("Consolas", 10F);
        this.txtStatus.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
        this.txtStatus.Location = new System.Drawing.Point(16, 400);
        this.txtStatus.Name = "txtStatus";
        this.txtStatus.Padding = new System.Windows.Forms.Padding(12);
        this.txtStatus.ReadOnly = true;
        this.txtStatus.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
        this.txtStatus.Size = new System.Drawing.Size(576, 104);
        this.txtStatus.TabIndex = 3;
        this.txtStatus.Text = "";
        
        // 
        // lblCredit
        // 
        this.lblCredit.AutoSize = false;
        this.lblCredit.Font = new System.Drawing.Font("Segoe UI", 8F);
        this.lblCredit.ForeColor = System.Drawing.Color.FromArgb(150, 150, 150);
        this.lblCredit.BackColor = System.Drawing.Color.Transparent;
        this.lblCredit.Location = new System.Drawing.Point(16, 512);
        this.lblCredit.Name = "lblCredit";
        this.lblCredit.Size = new System.Drawing.Size(300, 16);
        this.lblCredit.TabIndex = 4;
        this.lblCredit.Text = "© 2025 Henrik Storberg";
        this.lblCredit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblCredit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        
        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.FromArgb(24, 24, 24);
        this.ClientSize = new System.Drawing.Size(608, 536);
        this.Controls.Add(this.headerPanel);
        this.Controls.Add(this.cardSchedule);
        this.Controls.Add(this.cardMaintenance);
        this.Controls.Add(this.txtStatus);
        this.Controls.Add(this.lblCredit);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        // Load icon from file if present to avoid resource lookup issues
        try
        {
            var iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon\\icon.ico");
            if (System.IO.File.Exists(iconPath))
            {
                this.Icon = new System.Drawing.Icon(iconPath);
            }
        }
        catch
        {
            this.Icon = null;
        }

        // Load settings button image from file if present (icon\instilling_icon.png)
        try
        {
            // try embedded resource first
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            string resName = null;
            foreach (var n in asm.GetManifestResourceNames())
            {
                if (n.EndsWith("instilling_icon.png", StringComparison.OrdinalIgnoreCase))
                {
                    resName = n;
                    break;
                }
            }

            if (resName != null)
            {
                using (var s = asm.GetManifestResourceStream(resName))
                {
                    if (s != null)
                    {
                        var loaded = System.Drawing.Image.FromStream(s);
                        var bmp = new System.Drawing.Bitmap(loaded.Width, loaded.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using (var g = System.Drawing.Graphics.FromImage(bmp))
                        {
                            g.DrawImage(loaded, 0, 0, loaded.Width, loaded.Height);
                        }
                        // remove near-white/gray background by alpha-thresholding
                        int thr = 230;
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            for (int x = 0; x < bmp.Width; x++)
                            {
                                var c = bmp.GetPixel(x, y);
                                if (c.R >= thr && c.G >= thr && c.B >= thr)
                                {
                                    bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(0, c.R, c.G, c.B));
                                }
                            }
                        }
                        this.btnSettings.Image = bmp;
                        this.btnSettings.BackColor = System.Drawing.Color.Transparent;
                    }
                }
            }
            else
            {
                // fallback to file on disk if present
                var settingsImg = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon\\instilling_icon.png");
                if (System.IO.File.Exists(settingsImg))
                {
                    var loaded = System.Drawing.Image.FromFile(settingsImg);
                    var bmp = new System.Drawing.Bitmap(loaded.Width, loaded.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (var g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        g.DrawImage(loaded, 0, 0, loaded.Width, loaded.Height);
                    }
                    int thr = 230;
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            var c = bmp.GetPixel(x, y);
                            if (c.R >= thr && c.G >= thr && c.B >= thr)
                            {
                                bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(0, c.R, c.G, c.B));
                            }
                        }
                    }
                    this.btnSettings.Image = bmp;
                    this.btnSettings.BackColor = System.Drawing.Color.Transparent;
                }
                else
                {
                    // create a small emoji fallback image
                    var fb = new System.Drawing.Bitmap(36, 36, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (var g = System.Drawing.Graphics.FromImage(fb))
                    {
                        g.Clear(System.Drawing.Color.Transparent);
                        using (var f = new System.Drawing.Font("Segoe UI Emoji", 14F))
                        using (var b = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                        {
                            var sf = new System.Drawing.StringFormat { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center };
                            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                            g.DrawString("🛠️", f, b, new System.Drawing.RectangleF(0, 0, fb.Width, fb.Height), sf);
                        }
                    }
                    this.btnSettings.Image = fb;
                }
            }
        }
        catch
        {
            // fallback to emoji image
            var fb2 = new System.Drawing.Bitmap(36, 36, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = System.Drawing.Graphics.FromImage(fb2))
            {
                g.Clear(System.Drawing.Color.Transparent);
                using (var f = new System.Drawing.Font("Segoe UI Emoji", 14F))
                using (var b = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                {
                    var sf = new System.Drawing.StringFormat { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center };
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    g.DrawString("🛠️", f, b, new System.Drawing.RectangleF(0, 0, fb2.Width, fb2.Height), sf);
                }
            }
            this.btnSettings.Image = fb2;
        }
        this.MaximizeBox = false;
        this.Name = "MainForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        // Keep the real title so Alt+Tab and taskbar show the app name.
        this.Text = "DeaktiverBruker";
        this.headerPanel.ResumeLayout(false);
        this.headerPanel.PerformLayout();
        this.cardSchedule.ResumeLayout(false);
        this.cardSchedule.PerformLayout();
        this.cardMaintenance.ResumeLayout(false);
        this.cardMaintenance.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion
}
