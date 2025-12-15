using System;
using System.Linq;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        // If run by scheduled task with --runjob, execute the job headlessly
        if (args.Length > 0 && args[0] == "--runjob")
        {
            string? taskName = null;
            string? deadline = null;
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] == "--taskname" && i + 1 < args.Length) taskName = args[i + 1];
                if (args[i] == "--deadline" && i + 1 < args.Length) deadline = args[i + 1];
            }
            DeactivateService.RunJob(taskName, deadline);
            return;
        }

        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
