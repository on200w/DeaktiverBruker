using System;
using System.IO;
using System.Text.Json;

public class AppSettings
{
    public string AccountName { get; set; } = "Bruker";
    public string LegalCaption { get; set; } = "Låne tid utløpt";
    public string LegalText { get; set; } = "Denne kontoen er deaktivert av IKT, ta kontakt med IT-ansvarlig for å åpne den igjen";

    public static AppSettings Load()
    {
        try
        {
            var path = GetSettingsPath();
            if (!File.Exists(path)) return new AppSettings();
            var txt = File.ReadAllText(path);
            var s = JsonSerializer.Deserialize<AppSettings>(txt);
            return s ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public void Save()
    {
        try
        {
            var path = GetSettingsPath();
            var dir = Path.GetDirectoryName(path) ?? Environment.CurrentDirectory;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var txt = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, txt);
        }
        catch { }
    }

    static string GetSettingsPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appData, "DeaktiverBruker", "settings.json");
    }
}

public static class Settings
{
    private static AppSettings? _current;
    public static AppSettings Current => _current ??= AppSettings.Load();

    public static void Reload() => _current = AppSettings.Load();
}
