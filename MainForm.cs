using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public partial class MainForm : Form
{
    private const string InternalTitle = "DeaktiverBruker";
    public MainForm()
    {
        InitializeComponent();
        // Ensure header shows the real app name while the window title remains hidden
        lblTitle.Text = InternalTitle;
        this.AccessibleName = InternalTitle;
        lblTitle.AccessibleName = InternalTitle;
        ApplyDarkTheme();
        SetSmartDefaults();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        TryEnableDarkTitleBar();
    }

    private void TryEnableDarkTitleBar()
    {
        try
        {
            // DWMWA_USE_IMMERSIVE_DARK_MODE = 20 (Windows 10 1809+/Windows 11). Fallback to 19 if needed.
            int attribute = 20;
            int useDark = 1;
            int res = DwmSetWindowAttribute(this.Handle, attribute, ref useDark, Marshal.SizeOf<int>());
            if (res != 0)
            {
                // try older attribute id
                attribute = 19;
                DwmSetWindowAttribute(this.Handle, attribute, ref useDark, Marshal.SizeOf<int>());
            }
            // Attempt to hide the caption text by setting the caption/text colors
            // to match the header background so the titlebar text becomes invisible
            try
            {
                // DWMWA_CAPTION_COLOR = 35, DWMWA_TEXT_COLOR = 36
                var bg = this.headerPanel?.BackColor ?? Theme.HeaderBackground;
                uint colorRef = (uint)((bg.B << 16) | (bg.G << 8) | bg.R);
                int attrCaption = 35;
                uint cap = colorRef;
                DwmSetWindowAttributeColor(this.Handle, attrCaption, ref cap, Marshal.SizeOf<uint>());
                int attrText = 36;
                uint txt = colorRef;
                DwmSetWindowAttributeColor(this.Handle, attrText, ref txt, Marshal.SizeOf<uint>());
            }
            catch { }
        }
        catch { }
    }

    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttributeColor(IntPtr hwnd, int dwAttribute, ref uint pvAttribute, int cbAttribute);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

    protected override void WndProc(ref Message m)
    {
        const int WM_NCPAINT = 0x0085;
        const int WM_NCACTIVATE = 0x0086;

        if (m.Msg == WM_NCPAINT || m.Msg == WM_NCACTIVATE)
        {
            // Let the default drawing happen first
            base.WndProc(ref m);

            try
            {
                IntPtr hdc = GetWindowDC(this.Handle);
                if (hdc != IntPtr.Zero)
                {
                    using (var g = Graphics.FromHdc(hdc))
                    {
                        if (GetWindowRect(this.Handle, out var r))
                        {
                            int caption = GetSystemMetrics(4); // SM_CYCAPTION
                            int frame = GetSystemMetrics(33); // SM_CYFRAME
                            int height = caption + frame;
                            // Paint over the left portion of the titlebar where the text appears
                            var rect = new Rectangle(0, 0, this.Width, height);
                            using (var brush = new SolidBrush(headerPanel?.BackColor ?? Theme.HeaderBackground))
                            {
                                g.FillRectangle(brush, rect);
                            }
                        }
                    }
                    ReleaseDC(this.Handle, hdc);
                }
            }
            catch { }

            return;
        }

        base.WndProc(ref m);
    }

    void ApplyDarkTheme()
    {
        // Apply theme colors
        this.BackColor = Theme.Background;
        this.ForeColor = Theme.TextPrimary;
        headerPanel.BackColor = Theme.HeaderBackground;
        
        // Apply fonts
        lblTitle.Font = Theme.TitleFont;
        lblSubtitle.Font = Theme.SubtitleFont;
        lblScheduleSection.Font = Theme.SectionFont;
        lblMaintenanceSection.Font = Theme.SectionFont;
        
        // Card panels
        foreach (Control c in new Control[] { cardSchedule, cardMaintenance })
        {
            c.BackColor = Theme.Surface;
            c.ForeColor = Theme.TextPrimary;
        }
        
        // Status area
        txtStatus.BackColor = Theme.Surface;
        txtStatus.ForeColor = Theme.TextPrimary;
        txtStatus.Font = Theme.BodyFont;
        
        // Input controls: date/time pickers should be darker but slightly lighter than cards
        try
        {
            var light = Theme.SurfaceLight;
            // control colors (calendar popup uses MonthCalendar default styling)
            datePicker.BackColor = light;
            datePicker.ForeColor = Theme.TextPrimary;
            datePicker.Refresh();

            timePicker.BackColor = light;
            timePicker.ForeColor = Theme.TextPrimary;
            timePicker.Refresh();
        }
        catch { }
        
        // Apply button styles with hover effects — all buttons use primary green
        Theme.ApplyButtonStyle(btnSchedule, Theme.ButtonPrimary, Theme.ButtonPrimaryHover);
        Theme.ApplyButtonStyle(btnActivate, Theme.ButtonPrimary, Theme.ButtonPrimaryHover);
        Theme.ApplyButtonStyle(btnOpenScheduler, Theme.ButtonPrimary, Theme.ButtonPrimaryHover);
        // Test button uses primary green style to match other action buttons
        Theme.ApplyButtonStyle(btnTest, Theme.ButtonPrimary, Theme.ButtonPrimaryHover);
        // `btnSettings` is now a PictureBox (icon). No button styling applied.
        
        // Apply theme colors to other controls
        lblDate.Font = Theme.BodyFont;
        lblTime.Font = Theme.BodyFont;
        lblDate.ForeColor = Theme.TextSecondary;
        lblTime.ForeColor = Theme.TextSecondary;
        lblScheduleHelp.Font = Theme.BodyFont;
        lblScheduleHelp.ForeColor = Theme.TextSecondary;
    }
    
    void SetSmartDefaults()
    {
        // Set date to today
        datePicker.Value = DateTime.Now.Date;
        
        // Set time to 10 minutes from now, rounded to nearest 5 minutes
        DateTime future = DateTime.Now.AddMinutes(10);
        int roundedMinutes = (int)(Math.Ceiling(future.Minute / 5.0) * 5);
        if (roundedMinutes >= 60)
        {
            future = future.AddHours(1);
            roundedMinutes = 0;
        }
        timePicker.Value = new DateTime(future.Year, future.Month, future.Day, future.Hour, roundedMinutes, 0);
    }

    private void btnSchedule_Click(object sender, EventArgs e)
    {
        DateTime date = datePicker.Value.Date;
        DateTime time = timePicker.Value;
        DateTime when = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0);

        if (when <= DateTime.Now)
        {
            AppendStatus("Velg et tidspunkt i fremtiden.", StatusType.Error);
            return;
        }

        try
        {
            string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? Application.ExecutablePath;
            string taskNameId = "DeactivateBruker_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string result = DeactivateService.CreateScheduledTaskUnified(when, exePath, taskNameId);
            if (result.StartsWith("ERR:"))
            {
                AppendStatus(result, StatusType.Error);
            }
            else if (result.StartsWith("OK:"))
            {
                string full = result.Substring(3).Trim();
                string verify = DeactivateService.VerifyScheduledTask(full);
                if (!string.IsNullOrWhiteSpace(verify) && !verify.StartsWith("ERR:"))
                {
                    AppendStatus($"Bekreftet: {full}\n{verify}", StatusType.Success);
                }
                else
                {
                    AppendStatus($"Oppgave registrert, men ikke funnet i søk: {verify}", StatusType.Info);
                }
            }
            else
            {
                AppendStatus("Svar: " + result, StatusType.Info);
            }
        }
        catch (Exception ex)
        {
            AppendStatus("Feil: " + ex.Message, StatusType.Error);
        }
    }

    private void btnTest_Click(object sender, EventArgs e)
    {
        DateTime date = datePicker.Value.Date;
        DateTime time = timePicker.Value;
        DateTime when = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0);

        if (when <= DateTime.Now)
        {
            AppendStatus("Velg et tidspunkt i fremtiden for simulering.", StatusType.Error);
            return;
        }

        try
        {
            string summary = DeactivateService.SimulateSchedule(when);
            using var dlg = new SimulateDialog(summary);
            dlg.ShowDialog(this);
            AppendStatus("Simulering vist i dialog.", StatusType.Info);
        }
        catch (Exception ex)
        {
            AppendStatus("Feil ved simulering: " + ex.Message, StatusType.Error);
        }
    }

    private void btnActivate_Click(object? sender, EventArgs e)
    {
        try
        {
            string resultMsg = DeactivateService.ActivateAndCleanup();
            bool isError = false;
            if (!string.IsNullOrWhiteSpace(resultMsg))
            {
                if (resultMsg.IndexOf("ERR:", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    resultMsg.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    resultMsg.IndexOf("kunne", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    resultMsg.IndexOf("feil", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    isError = true;
                }
            }
            AppendStatus(resultMsg, isError ? StatusType.Error : StatusType.Success);
        }
        catch (Exception ex)
        {
            AppendStatus("Feil ved aktivering/rydding: " + ex.Message, StatusType.Error);
        }
    }

    private void btnOpenScheduler_Click(object? sender, EventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo("taskschd.msc") { UseShellExecute = true });
            AppendStatus("Åpnet Oppgaveplanlegger.", StatusType.Success);
        }
        catch (Exception ex)
        {
            AppendStatus("Kunne ikke åpne Oppgaveplanlegger: " + ex.Message, StatusType.Error);
        }
    }

    private void btnSettings_Click(object? sender, EventArgs e)
    {
        try
        {
            using var dlg = new SettingsDialog();
            var res = dlg.ShowDialog(this);
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                // Reload settings in memory (Settings.Current already updated by dialog Save)
                Settings.Reload();
                AppendStatus("Innstillinger lagret.", StatusType.Success);
            }
            else
            {
                AppendStatus("Innstillinger ikke endret.", StatusType.Info);
            }
        }
        catch (Exception ex)
        {
            AppendStatus("Kunne ikke åpne innstillinger: " + ex.Message, StatusType.Error);
        }
    }

    private void AppendStatus(string message, StatusType type = StatusType.Info)
    {
        var ts = DateTime.Now.ToString("HH:mm:ss");
        // If any line in the message contains known error patterns, treat whole entry as Error
        if (!string.IsNullOrWhiteSpace(message))
        {
            var low = message.ToLowerInvariant();
            string[] errTokens = new[] { "err:", "feil", "error", "kunne ikke", "ingen planlagte oppgaver funnet", "ingen planlagte oppgaver", "fant ikke brukernavnet", "powershell-fallback feilet", "fallback feilet", "kan ikke" };
            foreach (var t in errTokens)
            {
                if (low.Contains(t))
                {
                    type = StatusType.Error;
                    break;
                }
            }
        }
        string badge = type switch
        {
            StatusType.Success => "✓",
            StatusType.Error => "✗",
            StatusType.Info => "ℹ",
            _ => "·"
        };

        // Use RichTextBox styling for clearer log output
        try
        {
            var rtb = txtStatus; // now a RichTextBox
            int start = rtb.TextLength;

            // Timestamp (dim)
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionColor = Color.FromArgb(170, 170, 170);
            rtb.SelectionFont = new Font("Consolas", 9F, FontStyle.Regular);
            rtb.AppendText($"[{ts}] ");

            // Badge (colored)
            Color badgeColor = type switch
            {
                StatusType.Success => Theme.StatusOk,
                StatusType.Error => Theme.StatusError,
                StatusType.Info => Theme.StatusInfo,
                _ => Theme.TextPrimary
            };
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionColor = badgeColor;
            rtb.SelectionFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            rtb.AppendText(badge + " ");

            // Message (primary)
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionColor = Theme.TextPrimary;
            rtb.SelectionFont = new Font("Consolas", 9F, FontStyle.Regular);
            rtb.AppendText(message + "\r\n\r\n");

            // Auto-scroll
            rtb.SelectionStart = rtb.TextLength;
            rtb.ScrollToCaret();

            // Trim if too long (keep last ~1000 lines)
            const int maxLines = 1000;
            if (rtb.Lines.Length > maxLines)
            {
                int remove = rtb.GetFirstCharIndexFromLine(0 + (rtb.Lines.Length - maxLines));
                rtb.Select(0, remove);
                rtb.SelectedText = "";
            }
        }
        catch
        {
            // Fallback to simple append if something goes wrong
            txtStatus.AppendText($"[{ts}] {badge} {message}\r\n\r\n");
        }
    }
    
    enum StatusType { Success, Error, Info }
}
