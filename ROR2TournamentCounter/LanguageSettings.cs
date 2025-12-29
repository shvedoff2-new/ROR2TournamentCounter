using System;
using System.IO;

namespace ROR2TournamentCounter
{
    public static class LanguageSettings
    {
        private static readonly string SettingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ROR2TournamentCounter"
        );

        private static readonly string SettingsFile = Path.Combine(SettingsFolder, "language.txt");

        public static string Language
        {
            get
            {
                try
                {
                    if (File.Exists(SettingsFile))
                    {
                        string lang = File.ReadAllText(SettingsFile).Trim();
                        if (!string.IsNullOrEmpty(lang))
                        {
                            return lang;
                        }
                    }
                }
                catch { }

                return "en"; // По умолчанию английский
            }
            set
            {
                try
                {
                    if (!Directory.Exists(SettingsFolder))
                    {
                        Directory.CreateDirectory(SettingsFolder);
                    }

                    File.WriteAllText(SettingsFile, value);
                }
                catch { }
            }
        }
    }
}