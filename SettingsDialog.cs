using System;
using System.Drawing;
using System.Windows.Forms;

public class SettingsDialog : Form
{
    ComboBox cmbAccount;
    TextBox txtCaption;
    TextBox txtText;
    Button btnSave;
    Button btnCancel;

    public SettingsDialog()
    {
        this.Text = "Innstillinger";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterParent;
        this.ClientSize = new Size(600, 300);
        this.MaximizeBox = false;
        this.BackColor = Theme.Background;

        var lbl1 = new Label() { Text = "Konto som deaktiveres:", ForeColor = Theme.TextPrimary, Location = new Point(16, 16), AutoSize = true };
        cmbAccount = new ComboBox() { Location = new Point(16, 40), Width = 560, DropDownStyle = ComboBoxStyle.DropDownList };

        var lbl2 = new Label() { Text = "LegalNotice - Overskrift (caption):", ForeColor = Theme.TextPrimary, Location = new Point(16, 76), AutoSize = true };
        txtCaption = new TextBox() { Location = new Point(16, 100), Width = 560 };

        var lbl3 = new Label() { Text = "LegalNotice - Tekst:", ForeColor = Theme.TextPrimary, Location = new Point(16, 136), AutoSize = true };
        txtText = new TextBox() { Location = new Point(16, 160), Width = 560, Height = 68, Multiline = true, ScrollBars = ScrollBars.Vertical };

        // increase spacing between the two bottom buttons
        btnSave = new Button() { Text = "Lagre", Size = new Size(120, 36), Location = new Point(this.ClientSize.Width - 300, this.ClientSize.Height - 52), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
        btnCancel = new Button() { Text = "Avbryt", Size = new Size(120, 36), Location = new Point(this.ClientSize.Width - 150, this.ClientSize.Height - 52), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };

        Theme.ApplyButtonStyle(btnSave, Theme.ButtonPrimary, Theme.ButtonPrimaryHover);
        Theme.ApplyButtonStyle(btnCancel, Theme.ButtonNeutral, Theme.ButtonNeutralHover);

        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

        this.Controls.Add(lbl1);
        this.Controls.Add(cmbAccount);
        this.Controls.Add(lbl2);
        this.Controls.Add(txtCaption);
        this.Controls.Add(lbl3);
        this.Controls.Add(txtText);
        this.Controls.Add(btnSave);
        this.Controls.Add(btnCancel);

        // Load current settings
        // populate account list and set value
        try
        {
            var users = GetLocalUsers();
            if (users != null)
            {
                foreach (var u in users)
                {
                    cmbAccount.Items.Add(u);
                }
            }
        }
        catch { }
        // ensure default program account 'Bruker' is present at top
        if (!cmbAccount.Items.Contains("Bruker"))
            cmbAccount.Items.Insert(0, "Bruker");

        // set selected value if present in list, otherwise leave unselected
        if (!string.IsNullOrWhiteSpace(Settings.Current.AccountName) && cmbAccount.Items.Contains(Settings.Current.AccountName))
            cmbAccount.SelectedItem = Settings.Current.AccountName;
        else if (!string.IsNullOrWhiteSpace(Settings.Current.AccountName) && !cmbAccount.Items.Contains(Settings.Current.AccountName))
        {
            // if saved setting is not in list, insert it right after default
            cmbAccount.Items.Insert(1, Settings.Current.AccountName);
            cmbAccount.SelectedIndex = 1;
        }
        txtCaption.Text = Settings.Current.LegalCaption;
        txtText.Text = Settings.Current.LegalText;

        // Apply dark colors to inputs
        try
        {
            var light = Theme.SurfaceLight;
            cmbAccount.BackColor = light;
            cmbAccount.ForeColor = Theme.TextPrimary;
            cmbAccount.FlatStyle = FlatStyle.Flat;

            txtCaption.BackColor = light;
            txtCaption.ForeColor = Theme.TextPrimary;

            txtText.BackColor = light;
            txtText.ForeColor = Theme.TextPrimary;
        }
        catch { }
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
            int attribute = 20;
            int useDark = 1;
            DwmSetWindowAttribute(this.Handle, attribute, ref useDark, System.Runtime.InteropServices.Marshal.SizeOf<int>());
        }
        catch { }
    }

    [System.Runtime.InteropServices.DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    private string[]? GetLocalUsers()
    {
        // Prefer using WMI to get a reliable list of local accounts
        try
        {
            var users = new System.Collections.Generic.List<string>();
            var exclude = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Administrator",
                "Administratorer",
                "Guest",
                "Gjest",
                "Standardkonto",
                "DefaultAccount",
                "defaultuser0",
                "WDAGUtilityAccount",
                "Public",
                "All Users",
                "DefaultUser",
                "HelpAssistant"
            };

            try
            {
                var scopeQuery = new System.Management.ManagementObjectSearcher("SELECT Name, LocalAccount, Disabled FROM Win32_UserAccount WHERE LocalAccount = True");
                foreach (System.Management.ManagementObject mo in scopeQuery.Get())
                {
                    string? name = mo["Name"]?.ToString();
                    if (string.IsNullOrWhiteSpace(name)) continue;
                    // skip disabled/system accounts
                    try
                    {
                        var disabledObj = mo["Disabled"];
                        if (disabledObj is bool db && db) continue;
                    }
                    catch { }
                    if (exclude.Contains(name)) continue;
                    if (!users.Contains(name, StringComparer.OrdinalIgnoreCase))
                        users.Add(name);
                }
                return users.ToArray();
            }
            catch
            {
                // WMI query failed, fall back to parsing `net user`
            }

            // Fallback: parse `net user` output
            var psi = new System.Diagnostics.ProcessStartInfo("net", "user")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                StandardOutputEncoding = System.Text.Encoding.Default
            };
            using var p = System.Diagnostics.Process.Start(psi);
            if (p == null) return null;
            string outp = p.StandardOutput.ReadToEnd();
            p.WaitForExit(2000);

            var lines = outp.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            bool inList = false;
            var footerKeywords = new[] { "command", "completed", "fullført", "fullfort", "suksess", "successfully", "vellykket" };
            var nameRegex = new System.Text.RegularExpressions.Regex("^[\\p{L}\\p{N}_.\\-]+$", System.Text.RegularExpressions.RegexOptions.Compiled);

            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (!inList)
                {
                    if (line.StartsWith("---") || line.StartsWith("----") || line.IndexOf("---") >= 0)
                    {
                        inList = true;
                        continue;
                    }
                }
                else
                {
                    var lower = line.ToLowerInvariant();
                    bool isFooter = false;
                    foreach (var k in footerKeywords)
                    {
                        if (lower.Contains(k)) { isFooter = true; break; }
                    }
                    if (isFooter) break;

                    var parts = line.Split((char[])null!, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var ppart in parts)
                    {
                        var token = ppart.Trim();
                        if (token.Length == 0) continue;
                        if (token.Length > 50) continue;
                        if (token.Length < 3) continue;
                        if (!nameRegex.IsMatch(token)) continue;
                        if (exclude.Contains(token)) continue;
                        if (!users.Contains(token, StringComparer.OrdinalIgnoreCase))
                            users.Add(token);
                    }
                }
            }
            return users.ToArray();
        }
        catch
        {
            return null;
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        // Update current settings object but only persist when Save pressed
        Settings.Current.AccountName = cmbAccount.Text.Trim();
        Settings.Current.LegalCaption = txtCaption.Text.Trim();
        Settings.Current.LegalText = txtText.Text.Trim();
        try
        {
            Settings.Current.Save();
            this.DialogResult = DialogResult.OK;
        }
        catch
        {
            MessageBox.Show(this, "Kunne ikke lagre innstillinger.", "Feil", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
