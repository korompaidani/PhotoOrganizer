﻿using PhotoOrganizer.Model;
using System.Threading.Tasks;

namespace PhotoOrganizer.UI.Services
{
    public interface ISettingsHandler
    {
        string GetLanguageSettings();
        void LoadAtStartupInitialSettings();
        Task<Settings> LoadSettingsAsync();
        Task SaveSettingsAsync(Settings settings);
        Task ApplySettingsAsync(Settings settings);
    }
}
