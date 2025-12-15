using System;
using System.Drawing;
using System.Windows.Forms;

public class SimulateDialog : Form
{
    private RichTextBox rtb;
    private Button btnCopy;
    private Button btnClose;

    public SimulateDialog(string text)
    {
        this.Text = "Simulering - Forhåndsvisning";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterParent;
        this.ClientSize = new Size(720, 520);
        this.MaximizeBox = false;

        // Apply dark theme to dialog
        this.BackColor = Theme.Background;
        this.ForeColor = Theme.TextPrimary;

        rtb = new RichTextBox();
        rtb.ReadOnly = true;
        rtb.Font = new Font("Consolas", 10F);
        rtb.Dock = DockStyle.Top;
        rtb.Height = 440;
        rtb.BorderStyle = BorderStyle.None;
        rtb.BackColor = Theme.Surface;
        rtb.ForeColor = Theme.TextPrimary;
        rtb.Text = text;
        this.Controls.Add(rtb);

        var panel = new Panel();
        panel.Dock = DockStyle.Bottom;
        panel.Height = 64;
        panel.Padding = new Padding(8);
        panel.BackColor = Theme.Surface;
        this.Controls.Add(panel);

        // Place buttons with extra spacing between them
        btnCopy = new Button();
        btnCopy.Text = "Kopier";
        btnCopy.Size = new Size(120, 36);
        btnCopy.Location = new Point(this.ClientSize.Width - 320, 12);
        btnCopy.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
        btnCopy.Click += (s, e) => { try { Clipboard.SetText(rtb.Text); btnCopy.Text = "Kopiert"; } catch { } };
        panel.Controls.Add(btnCopy);

        btnClose = new Button();
        btnClose.Text = "Lukk";
        btnClose.Size = new Size(120, 36);
        btnClose.Location = new Point(this.ClientSize.Width - 160, 12);
        btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
        btnClose.Click += (s, e) => this.Close();
        panel.Controls.Add(btnClose);

        // Apply theme to buttons
        Theme.ApplyButtonStyle(btnCopy, Theme.ButtonPrimary, Theme.ButtonPrimaryHover);
        Theme.ApplyButtonStyle(btnClose, Theme.ButtonNeutral, Theme.ButtonNeutralHover);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        try
        {
            int attribute = 20;
            int useDark = 1;
            DwmSetWindowAttribute(this.Handle, attribute, ref useDark, System.Runtime.InteropServices.Marshal.SizeOf<int>());
        }
        catch { }
    }

    [System.Runtime.InteropServices.DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);
}
