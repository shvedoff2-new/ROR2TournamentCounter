using System;
using System.Windows;

namespace ROR2TournamentCounter
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Загружаем английский язык по умолчанию
            try
            {
                var dict = new ResourceDictionary();
                dict.Source = new Uri("Resources/Languages/en.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
            catch { }
        }
    }
}