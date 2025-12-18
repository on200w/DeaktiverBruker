using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using System.DirectoryServices.AccountManagement;


static class DeactivateService
{
    private static string AccountName => Settings.Current.AccountName ?? "Bruker";

        public static string BuildTaskXml(DateTime when, string exePath, string taskName)
    {
                string startBoundary = when.ToString("s"); // ISO 8601 local time with seconds
        string exeEscaped = System.Security.SecurityElement.Escape(exePath);

                return $@"<?xml version=""1.0"" encoding=""utf-16""?>
<Task version=""1.2"" xmlns=""http://schemas.microsoft.com/windows/2004/02/mit/task"">
  <RegistrationInfo>
    <Date>{DateTime.Now:s}</Date>
    <Author>DeaktiverBruker</Author>
    <Description>Deaktiverer kontoen Bruker på angitt tidspunkt</Description>
  </RegistrationInfo>
  <Triggers>
    <TimeTrigger>
      <StartBoundary>{startBoundary}</StartBoundary>
      <Enabled>true</Enabled>
    </TimeTrigger>
  </Triggers>
  <Principals>
    <Principal id=""Author"">
      <UserId>S-1-5-18</UserId>
      <LogonType>ServiceAccount</LogonType>
      <RunLevel>HighestAvailable</RunLevel>
    </Principal>
  </Principals>
  <Settings>
    <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>
    <DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>
    <StopIfGoingOnBatteries>false</StopIfGoingOnBatteries>
    <AllowHardTerminate>true</AllowHardTerminate>
    <StartWhenAvailable>false</StartWhenAvailable>
    <RunOnlyIfNetworkAvailable>false</RunOnlyIfNetworkAvailable>
    <Enabled>true</Enabled>
    <Hidden>false</Hidden>
    <ExecutionTimeLimit>PT0S</ExecutionTimeLimit>
    <Priority>7</Priority>
  </Settings>
  <Actions Context=""Author"">
    <Exec>
      <Command>{exeEscaped}</Command>
      <Arguments>--runjob --taskname {taskName}</Arguments>
    </Exec>
  </Actions>
</Task>";
    }

    public static string ActivateAndCleanup()
    {
        var sb = new StringBuilder();
        bool hadError = false;
        // 1) Try to activate local account — prefer managed API (no external process)
        try
        {
            using (var ctx = new PrincipalContext(ContextType.Machine))
            using (var user = UserPrincipal.FindByIdentity(ctx, AccountName))
            {
                if (user != null)
                {
                    user.Enabled = true;
                    user.Save();
                    sb.AppendLine($"Konto: Aktivert via API: {AccountName}");
                }
                else
                {
                    sb.AppendLine("(Konto ikke funnet — ignoreres)");
                }
            }
        }
        catch (Exception)
        {
            // fallback to net.exe if AccountManagement fails
            string? netOut = null;
            try { netOut = RunProcessCapture("net", $"user \"{AccountName}\" /active:yes"); } catch (Exception e) { netOut = "ERR: " + e.Message; }
            if (!string.IsNullOrWhiteSpace(netOut))
            {
                sb.AppendLine("Konto: " + netOut.Trim());
                var lower = netOut.ToLowerInvariant();
                bool accountNotFound = lower.Contains("fant ikke brukernavnet") || lower.Contains("could not find the user name") || lower.Contains("user name could not be found");
                if (!accountNotFound)
                {
                    if (lower.Contains("could not") || lower.Contains("err") || lower.Contains("kan ikke") || lower.Contains("feil"))
                        hadError = true;
                }
                else
                {
                    sb.AppendLine("(Konto ikke funnet — ignoreres)");
                }
            }
            else
            {
                sb.AppendLine("Konto: Aktivert (ukjent metode)");
            }
        }

        // 2) Remove Windows legal notice (caption/text) — use registry API where possible
        try
        {
            var regPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
            var beforeCaption = (Microsoft.Win32.Registry.GetValue(regPath, "legalnoticecaption", null) as string) ?? string.Empty;
            var beforeText = (Microsoft.Win32.Registry.GetValue(regPath, "legalnoticetext", null) as string) ?? string.Empty;
            sb.AppendLine($"LegalNotice - før: caption='{beforeCaption}', text='{(beforeText.Length > 64 ? beforeText.Substring(0, 64) + "..." : beforeText)}'");

            Microsoft.Win32.Registry.SetValue(regPath, "legalnoticecaption", "", Microsoft.Win32.RegistryValueKind.String);
            Microsoft.Win32.Registry.SetValue(regPath, "legalnoticetext", "", Microsoft.Win32.RegistryValueKind.String);

            var afterCaption = (Microsoft.Win32.Registry.GetValue(regPath, "legalnoticecaption", null) as string) ?? string.Empty;
            var afterText = (Microsoft.Win32.Registry.GetValue(regPath, "legalnoticetext", null) as string) ?? string.Empty;
            sb.AppendLine($"LegalNotice - etter: caption='{afterCaption}', text='{(afterText.Length > 64 ? afterText.Substring(0, 64) + "..." : afterText)}'");

            if (string.IsNullOrEmpty(beforeCaption) && string.IsNullOrEmpty(beforeText))
            {
                sb.AppendLine("LegalNotice: ingen eksisterende verdi funnet (ingenting å fjerne)");
            }
            else if (string.IsNullOrEmpty(afterCaption) && string.IsNullOrEmpty(afterText))
            {
                sb.AppendLine("LegalNotice: fjernet");
            }
            else
            {
                // Something remained
                var remain = new System.Collections.Generic.List<string>();
                if (!string.IsNullOrEmpty(afterCaption)) remain.Add("caption");
                if (!string.IsNullOrEmpty(afterText)) remain.Add("text");
                sb.AppendLine($"LegalNotice: kunne ikke fjerne: {string.Join(", ", remain)}");
                hadError = true;
            }
        }
        catch
        {
            // fallback to reg.exe calls and then verify
            var reg1 = RunProcessCapture("reg", "add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\" /v legalnoticecaption /t REG_SZ /d \"\" /f");
            var reg2 = RunProcessCapture("reg", "add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\" /v legalnoticetext /t REG_SZ /d \"\" /f");
            if ((reg1 ?? "").IndexOf("ERR:", StringComparison.OrdinalIgnoreCase) >= 0 || (reg2 ?? "").IndexOf("ERR:", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                hadError = true;
                sb.AppendLine("LegalNotice: feilet å fjerne (reg.exe rapporterte feil)");
            }
            else
            {
                // verify by reading back
                try
                {
                    var regPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
                    var afterCaption = (Microsoft.Win32.Registry.GetValue(regPath, "legalnoticecaption", null) as string) ?? string.Empty;
                    var afterText = (Microsoft.Win32.Registry.GetValue(regPath, "legalnoticetext", null) as string) ?? string.Empty;
                    if (string.IsNullOrEmpty(afterCaption) && string.IsNullOrEmpty(afterText))
                    {
                        sb.AppendLine("LegalNotice: fjernet (via reg.exe)");
                    }
                    else
                    {
                        sb.AppendLine("LegalNotice: ikke fjernet fullstendig (etter reg.exe)");
                        hadError = true;
                    }
                }
                catch
                {
                    sb.AppendLine("LegalNotice: usikker status etter reg.exe");
                    hadError = true;
                }
            }
        }

        // 3) Delete all scheduled tasks created by this app (folder or prefix)
        var tasks = ListAppTasks();
        int deleted = 0;
        if (tasks.Length == 0)
        {
            sb.AppendLine("Ingen planlagte oppgaver funnet via schtasks-forespørsel. Forsøker PowerShell-fallback...");
            // fallback: query Scheduled Tasks via PowerShell in the DeaktiverBruker folder and by prefix
            try
            {
                string ps =
                    "$ErrorActionPreference='Stop'; " +
                    "Get-ScheduledTask -TaskPath '\\DeaktiverBruker\\' -ErrorAction SilentlyContinue | Select-Object TaskName, TaskPath | ConvertTo-Json -Compress; " +
                    "Get-ScheduledTask | Where-Object { $_.TaskName -like 'DeactivateBruker_*' } | Select-Object TaskName, TaskPath | ConvertTo-Json -Compress;";
                var psOut = RunProcessCapture("powershell.exe", "-NoProfile -ExecutionPolicy Bypass -Command \"" + ps.Replace("\"","\\\"") + "\"");
                if (!string.IsNullOrWhiteSpace(psOut))
                {
                    // Try parse JSON arrays; split into lines and extract TaskPath/TaskName pairs
                    var lines = psOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (line.TrimStart().StartsWith("[{") || line.TrimStart().StartsWith("{"))
                        {
                            try
                            {
                                // crude parse: look for "TaskPath":"..." and "TaskName":"..."
                                var mPath = System.Text.RegularExpressions.Regex.Match(line, "\"TaskPath\":\"(?<p>.*?)\"");
                                var mName = System.Text.RegularExpressions.Regex.Match(line, "\"TaskName\":\"(?<n>.*?)\"");
                                if (mName.Success)
                                {
                                    string pth = mPath.Success ? mPath.Groups["p"].Value : "\\DeaktiverBruker\\";
                                    string nm = mName.Groups["n"].Value;
                                    // JSON strings may contain escaped backslashes (\\) — unescape so schtasks gets single backslashes
                                    try
                                    {
                                        pth = System.Text.RegularExpressions.Regex.Unescape(pth);
                                        nm = System.Text.RegularExpressions.Regex.Unescape(nm);
                                    }
                                    catch { }
                                    if (!pth.StartsWith("\\")) pth = "\\" + pth;
                                    string full = (pth.EndsWith("\\") ? pth : pth + "\\") + nm;
                                    var delOut = RunProcessCapture("schtasks", $"/Delete /TN \"{full}\" /F");
                                    sb.AppendLine($"Forsøkte slette {full}: {delOut.Trim()}");
                                    if (!((delOut ?? "").IndexOf("ERR:", StringComparison.OrdinalIgnoreCase) >= 0 || (delOut ?? "").IndexOf("ERROR", StringComparison.OrdinalIgnoreCase) >= 0))
                                        deleted++;
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("PowerShell-fallback feilet: " + ex.Message);
                hadError = true;
            }
        }
        else
        {
            foreach (var tn in tasks)
            {
                var delOut = RunProcessCapture("schtasks", $"/Delete /TN \"{tn}\" /F");
                sb.AppendLine($"Slettet {tn}: {delOut.Trim()}");
                if ((delOut ?? "").IndexOf("ERR:", StringComparison.OrdinalIgnoreCase) >= 0 || (delOut ?? "").IndexOf("ERROR", StringComparison.OrdinalIgnoreCase) >= 0)
                    hadError = true;
                else
                    deleted++;
            }
            sb.AppendLine($"Oppgaver slettet: {deleted}");
        }

        // Build a tidy, human-friendly summary
        var status = hadError ? "Aktivert (med feil)" : "Aktivert";
        var final = new StringBuilder();
        final.AppendLine(status + ":");
        final.AppendLine(new string('─', 44));
        var summaryLines = sb.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in summaryLines)
        {
            final.AppendLine(" - " + line.Trim());
        }
        return final.ToString().Trim();
    }

    private static string[] ListAppTasks()
    {
        try
        {
            var output = RunProcessCapture("schtasks", "/Query /FO CSV /NH");
            if (string.IsNullOrWhiteSpace(output)) return Array.Empty<string>();

            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var list = new System.Collections.Generic.List<string>();
            foreach (var line in lines)
            {
                // CSV line starts with "TaskName","..."
                int firstQuote = line.IndexOf('"');
                if (firstQuote < 0) continue;
                int secondQuote = line.IndexOf('"', firstQuote + 1);
                if (secondQuote <= firstQuote) continue;
                string rawName = line.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
                // rawName can be like \\DeaktiverBruker\\DeactivateBruker_... or \\DeactivateBruker_...
                string nameNoLeading = rawName.StartsWith("\\") ? rawName.Substring(1) : rawName;
                if (nameNoLeading.Contains("\\DeaktiverBruker\\", StringComparison.OrdinalIgnoreCase) ||
                    nameNoLeading.StartsWith("DeactivateBruker_", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(rawName); // keep full path for schtasks /Delete
                }
            }
            return list.ToArray();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    public static void RunJob(string? taskName, string? deadlineIso = null)
    {
        // Show message on Windows logon screen via Legal Notice — use registry API to avoid spawning reg.exe
        try
        {
            string caption = Settings.Current.LegalCaption ?? "Låne tid utløpt";
            string text = Settings.Current.LegalText ?? "Denne kontoen er deaktivert av IKT, ta kontakt med IT-ansvarlig for å åpne den";
            try
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "legalnoticecaption", caption, Microsoft.Win32.RegistryValueKind.String);
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "legalnoticetext", text, Microsoft.Win32.RegistryValueKind.String);
            }
            catch
            {
                // fallback
                RunProcessCapture("reg", "add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\" /v legalnoticecaption /t REG_SZ /d \"" + caption.Replace("\"", "\\\"") + "\" /f");
                RunProcessCapture("reg", "add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\" /v legalnoticetext /t REG_SZ /d \"" + text.Replace("\"", "\\\"") + "\" /f");
            }
        }
        catch { }

        if (!string.IsNullOrWhiteSpace(deadlineIso))
        {
            try
            {
                var deadline = DateTime.Parse(deadlineIso);
                if (DateTime.Now < deadline)
                {
                    // Not reached deadline yet (e.g., startup trigger fired early). Do nothing and keep task.
                    return;
                }
            }
            catch { }
        }

        var sessions = GetUserSessions();

        // Disable account via API where possible
        try
        {
            using (var ctx = new PrincipalContext(ContextType.Machine))
            using (var user = UserPrincipal.FindByIdentity(ctx, AccountName))
            {
                if (user != null)
                {
                    user.Enabled = false;
                    user.Save();
                }
                else
                {
                    // fallback
                    RunProcessCapture("net", $"user \"{AccountName}\" /active:no");
                }
            }
        }
        catch
        {
            RunProcessCapture("net", $"user \"{AccountName}\" /active:no");
        }

        foreach (var s in sessions.Where(x => string.Equals(x.Username, AccountName, StringComparison.OrdinalIgnoreCase)))
        {
            RunProcessCapture("logoff", s.SessionId.ToString());
        }

        if (!string.IsNullOrEmpty(taskName))
        {
            // If task was created under a folder, delete with full path first
            string full = $"\\DeaktiverBruker\\{taskName}";
            RunProcessCapture("schtasks", $"/Delete /TN \"{full}\" /F");
            RunProcessCapture("schtasks", $"/Delete /TN \"{taskName}\" /F");
        }
    }

    private record SessionInfo(int SessionId, string Username, string State);

    private static SessionInfo[] GetUserSessions()
    {
        try
        {
            var list = new System.Collections.Generic.List<SessionInfo>();
            // Use WTS APIs to enumerate sessions without starting an external process
            IntPtr pSessions = IntPtr.Zero;
            int sessionCount = 0;
            if (WTSEnumerateSessions(IntPtr.Zero, 0, 1, out pSessions, out sessionCount) && sessionCount > 0)
            {
                int dataSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(WTS_SESSION_INFO));
                IntPtr current = pSessions;
                for (int i = 0; i < sessionCount; i++)
                {
                    var sInfo = System.Runtime.InteropServices.Marshal.PtrToStructure<WTS_SESSION_INFO>(current);
                    string user = QuerySessionString(sInfo.SessionId, WTS_INFO_CLASS.WTSUserName);
                    string state = sInfo.State.ToString();
                    if (!string.IsNullOrEmpty(user))
                        list.Add(new SessionInfo(sInfo.SessionId, user, state));
                    current = IntPtr.Add(current, dataSize);
                }
                WTSFreeMemory(pSessions);
            }
            return list.ToArray();
        }
        catch
        {
            return Array.Empty<SessionInfo>();
        }
    }

    private static string QuerySessionString(int sessionId, WTS_INFO_CLASS infoClass)
    {
        IntPtr ptr = IntPtr.Zero;
        int bytes = 0;
        try
        {
            if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, infoClass, out ptr, out bytes) && ptr != IntPtr.Zero)
            {
                return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ptr) ?? string.Empty;
            }
        }
        catch { }
        finally
        {
            if (ptr != IntPtr.Zero) WTSFreeMemory(ptr);
        }
        return string.Empty;
    }

    private enum WTS_INFO_CLASS
    {
        WTSUserName = 5
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct WTS_SESSION_INFO
    {
        public int SessionId;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
        public string pWinStationName;
        public int State;
    }

    [System.Runtime.InteropServices.DllImport("wtsapi32.dll", SetLastError = true)]
    private static extern bool WTSEnumerateSessions(IntPtr hServer, int reserved, int version, out IntPtr ppSessionInfo, out int pCount);

    [System.Runtime.InteropServices.DllImport("wtsapi32.dll")]
    private static extern void WTSFreeMemory(IntPtr pMemory);

    [System.Runtime.InteropServices.DllImport("wtsapi32.dll", SetLastError = true)]
    private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);

    public static string CreateScheduledTaskWithPowerShell(DateTime when, string exePath, string taskName)
    {
        try
        {
            string whenIso = when.ToString("s"); // 2025-12-10T09:10:00
            string exeEsc = exePath.Replace("'", "''");
            string argsEsc = ($"--runjob --taskname {taskName} --deadline {whenIso}").Replace("'", "''");

            string ps =
                "$ErrorActionPreference='Stop'; Import-Module ScheduledTasks -ErrorAction Stop;" +
                "$svc = New-Object -ComObject 'Schedule.Service'; $svc.Connect();" +
                "$root = $svc.GetFolder('\\'); try { $null = $root.GetFolder('DeaktiverBruker') } catch { $root.CreateFolder('DeaktiverBruker') | Out-Null };" +
                "$runAt = [datetime]'" + whenIso + "';" +
                "$action = New-ScheduledTaskAction -Execute '" + exeEsc + "' -Argument '" + argsEsc + "';" +
                "$trigger = New-ScheduledTaskTrigger -Once -At $runAt;" +
                "$trigger2 = New-ScheduledTaskTrigger -AtStartup;" +
                "$principal = New-ScheduledTaskPrincipal -UserId 'SYSTEM' -LogonType ServiceAccount -RunLevel Highest;" +
                "$settings = New-ScheduledTaskSettingsSet -StartWhenAvailable:$true -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries;" +
                "Register-ScheduledTask -TaskPath '\\DeaktiverBruker\\' -TaskName '" + taskName + "' -Action $action -Trigger @($trigger,$trigger2) -Principal $principal -Settings $settings -Force | Out-Null;" +
                "Start-Sleep -Milliseconds 300; " +
                "$t = Get-ScheduledTask -TaskPath '\\DeaktiverBruker\\' -TaskName '" + taskName + "' -ErrorAction Stop; " +
                "Write-Host ('OK:\\DeaktiverBruker\\" + taskName + "')";

            // Wrap to ensure ERR: on failure
            ps = "try { " + ps + " } catch { Write-Host ('ERR:' + $_.Exception.Message) }";

            string cmdArgs = "-NoProfile -ExecutionPolicy Bypass -Command \"" + ps.Replace("\"", "\\\"") + "\"";
            return RunProcessCapture("powershell.exe", cmdArgs);
        }
        catch (Exception ex)
        {
            return "ERR: " + ex.Message;
        }
    }

    

    public static string CreateScheduledTaskWithSchTasks(DateTime when, string exePath, string taskName)
    {
        try
        {
            // Ensure folder exists via COM
            string psFolder =
                "$svc = New-Object -ComObject 'Schedule.Service'; $svc.Connect();" +
                "$root = $svc.GetFolder('\\'); try { $null = $root.GetFolder('DeaktiverBruker') } catch { $root.CreateFolder('DeaktiverBruker') | Out-Null }";
            string argsCreateFolder = "-NoProfile -ExecutionPolicy Bypass -Command \"" + psFolder.Replace("\"", "\\\"") + "\"";
            RunProcessCapture("powershell.exe", argsCreateFolder);

            string dateMDY = when.ToString("MM/dd/yyyy");
            string timeHM = when.ToString("HH:mm");
            string taskPath = "\\DeaktiverBruker\\" + taskName;
            string tr = "\"" + exePath + "\" --runjob --taskname " + taskName + " --deadline " + when.ToString("s");
            string args = $"/Create /TN \"{taskPath}\" /TR \"{tr}\" /SC ONCE /SD {dateMDY} /ST {timeHM} /RL HIGHEST /RU SYSTEM /F";
            string outp = RunProcessCapture("schtasks", args);
            if (outp.IndexOf("SUCCESS", StringComparison.OrdinalIgnoreCase) >= 0 || outp.IndexOf("opprettet", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // Enable StartWhenAvailable after creation using PowerShell
                string psEnable =
                    "$ErrorActionPreference='Stop'; " +
                    "$settings = New-ScheduledTaskSettingsSet -StartWhenAvailable:$true -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries; " +
                    "Set-ScheduledTask -TaskPath '\\DeaktiverBruker\\' -TaskName '" + taskName.Replace("'","''") + "' -Settings $settings";
                string argsEnable = "-NoProfile -ExecutionPolicy Bypass -Command \"" + psEnable.Replace("\"","\\\"") + "\"";
                RunProcessCapture("powershell.exe", argsEnable);
                return "OK:" + taskPath;
            }
            return "ERR: schtasks: " + outp.Trim();
        }
        catch (Exception ex)
        {
            return "ERR: " + ex.Message;
        }
    }

    public static string CreateScheduledTaskUnified(DateTime when, string exePath, string taskName)
    {
        // Try PowerShell cmdlets first
        var r1 = CreateScheduledTaskWithPowerShell(when, exePath, taskName);
        if (r1.StartsWith("OK:"))
        {
            var verify = VerifyScheduledTask(r1.Substring(3).Trim());
            if (!string.Equals(verify, "NOTFOUND", StringComparison.OrdinalIgnoreCase) && !verify.StartsWith("ERR:"))
                return r1;
        }
        // Fallback to schtasks
        var r2 = CreateScheduledTaskWithSchTasks(when, exePath, taskName);
        if (r2.StartsWith("OK:")) return r2;
        return r1 + "\n" + r2;
    }

    public static string SimulateSchedule(DateTime when)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Simulering: Planlagt deaktivering av bruker (kun forhåndsvisning)");
        sb.AppendLine(new string('-', 56));
        sb.AppendLine($"Tidspunkt: {when:dddd, dd MMMM yyyy   HH:mm}");
        sb.AppendLine($"Oppgavenavn (forslag): \\DeaktiverBruker\\DeactivateBruker_SIM_{when:yyyyMMddHHmm}");
        sb.AppendLine();
        sb.AppendLine("Trigger:");
        sb.AppendLine("  • Kjøres én gang på valgt tidspunkt");
        sb.AppendLine("  • I tillegg: AtStartup (forsøk ved oppstart hvis PC var av)");
        sb.AppendLine();
        sb.AppendLine("Innstillinger:");
        sb.AppendLine("  • StartWhenAvailable = true (prøver å kjøre ved oppstart hvis maskinen var av)");
        sb.AppendLine("  • Kjøres som SYSTEM med høyeste rettigheter");
        sb.AppendLine();
        sb.AppendLine("Kommando (generisk):");
        sb.AppendLine("  DeaktiverBruker.exe --runjob --taskname <TASKNAME> --deadline <ISO_DATETIME>");
        sb.AppendLine();
        sb.AppendLine("Handlinger som ville blitt utført ved kjøring:");
        sb.AppendLine("  • Sette Windows-påloggingsmelding (LegalNotice) via registeret");
        sb.AppendLine("      - Registry keys som oppdateres:");
        sb.AppendLine("          HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\legalnoticecaption");
        sb.AppendLine("          HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\legalnoticetext");
        var caption = Settings.Current.LegalCaption ?? "Låne tid utløpt";
        var text = Settings.Current.LegalText ?? "Denne kontoen er deaktivert av IKT, ta kontakt med IT-ansvarlig for å åpne den igjen";
        sb.AppendLine($"      - Caption (tittel): {caption}");
        sb.AppendLine($"      - Tekst (melding for brukeren): \"{text}\"");
        sb.AppendLine("      - Eksempel-kommandoer (registrere/clear):");
        sb.AppendLine("          reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\" /v legalnoticecaption /t REG_SZ /d \"Låne tid utløpt\" /f");
        sb.AppendLine("          reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\" /v legalnoticetext /t REG_SZ /d \"Denne kontoen er deaktivert av IKT, ta kontakt med IT-ansvarlig for å åpne den igjen\" /f");
        sb.AppendLine($"  • Deaktivere lokal konto '{AccountName}' ved å kjøre: net user \"{AccountName}\" /active:no");
        sb.AppendLine("      - Dette gjør kontoen utilgjengelig ved pålogging");
        sb.AppendLine("  • Logge av eventuelle aktive økter for brukeren:");
        sb.AppendLine("      - Skriptet finner aktive sesjoner for brukernavnet og kjører: logoff <SessionId>");
        sb.AppendLine("  • Slette den planlagte oppgaven etter fullført kjøring (forsøk på både oppgavebane og navn):");
        sb.AppendLine("      - schtasks /Delete /TN \"\\DeaktiverBruker\\<TaskName>\" /F");
        sb.AppendLine("      - schtasks /Delete /TN \"<TaskName>\" /F");
        sb.AppendLine();
        sb.AppendLine("MERK: Dette er kun en simulering — ingen endringer vil bli gjort på systemet.");
        return sb.ToString();
    }

    public static string VerifyScheduledTask(string fullPathOrName)
    {
        try
        {
            string taskPath;
            string taskName;
            if (!string.IsNullOrWhiteSpace(fullPathOrName) && fullPathOrName.StartsWith("\\"))
            {
                int last = fullPathOrName.LastIndexOf('\\');
                if (last <= 0) return "ERR: Ugyldig oppgavenavn.";
                taskPath = fullPathOrName.Substring(0, last + 1); // keep trailing backslash
                taskName = fullPathOrName.Substring(last + 1);
            }
            else
            {
                taskPath = "\\DeaktiverBruker\\";
                taskName = fullPathOrName;
            }

            string ps =
                "$ErrorActionPreference='Stop'; " +
                "$t = Get-ScheduledTask -TaskPath '" + taskPath.Replace("'", "''") + "' -TaskName '" + taskName.Replace("'", "''") + "' -ErrorAction Stop; " +
                "$t | Select-Object TaskName, TaskPath, State, NextRunTime | Format-Table -AutoSize | Out-String";

            string cmdArgs = "-NoProfile -ExecutionPolicy Bypass -Command \"" + ps.Replace("\"", "\\\"") + "\"";
            var output = RunProcessCapture("powershell.exe", cmdArgs);
            if (string.IsNullOrWhiteSpace(output)) return "NOTFOUND";
            return output.Trim();
        }
        catch (Exception ex)
        {
            return "ERR: " + ex.Message;
        }
    }

    private static void SendMessageToSession(int sessionId, string message)
    {
        try
        {
            RunProcessCapture("msg", $"{sessionId} \"{message}\"");
        }
        catch { }
    }

    private static string RunProcessCapture(string filename, string args)
    {
        try
        {
            var psi = new ProcessStartInfo(filename, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using var p = Process.Start(psi) ?? throw new Exception($"Kunne ikke starte {filename}");

            // Read raw bytes from stdout and stderr, then try to decode with several encodings
            using var msOut = new System.IO.MemoryStream();
            using var msErr = new System.IO.MemoryStream();
            p.StandardOutput.BaseStream.CopyTo(msOut);
            p.StandardError.BaseStream.CopyTo(msErr);
            p.WaitForExit();

            var outBytes = msOut.ToArray();
            var errBytes = msErr.ToArray();

            // Candidate encodings in order of preference
            var candidates = new System.Collections.Generic.List<Encoding>();
            try { candidates.Add(Encoding.UTF8); } catch { }
            try { candidates.Add(Encoding.GetEncoding(1252)); } catch { }
            try { candidates.Add(Encoding.GetEncoding(850)); } catch { }
            try { candidates.Add(Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage)); } catch { }

            string? bestOut = null;
            string? bestErr = null;
            int bestScore = -1;
            foreach (var enc in candidates)
            {
                try
                {
                    string o = enc.GetString(outBytes);
                    string e = enc.GetString(errBytes);
                    int score = 0;
                    // boost score for presence of Norwegian letters
                    if (o.IndexOf('æ') >= 0 || o.IndexOf('ø') >= 0 || o.IndexOf('å') >= 0 || o.IndexOf('Æ') >= 0 || o.IndexOf('Ø') >= 0 || o.IndexOf('Å') >= 0) score += 10;
                    if (e.IndexOf('æ') >= 0 || e.IndexOf('ø') >= 0 || e.IndexOf('å') >= 0) score += 5;
                    // penalize replacement chars
                    score -= CountChar(o, '\uFFFD');
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestOut = o;
                        bestErr = e;
                    }
                }
                catch { }
            }

            string outp = bestOut ?? Encoding.UTF8.GetString(outBytes);
            string err = bestErr ?? Encoding.UTF8.GetString(errBytes);
            if (!string.IsNullOrEmpty(err)) outp += "\nERR: " + err;
            return outp;
        }
        catch (Exception ex)
        {
            return "ERR: " + ex.Message;
        }
    }

    private static int CountChar(string s, char c)
    {
        if (string.IsNullOrEmpty(s)) return 0;
        int count = 0;
        foreach (var ch in s) if (ch == c) count++;
        return count;
    }
}
