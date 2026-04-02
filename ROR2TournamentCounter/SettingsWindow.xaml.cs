using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ROR2TournamentCounter
{
    public partial class SettingsWindow : Window
    {
        private MainWindow displayWindow;
        private DispatcherTimer displayTimer;
        private Stopwatch stopwatch;
        private bool isRunning;
        private Random random = new Random();

        public SettingsWindow()
        {
            try
            {
                InitializeComponent();
                InitializeTimer();
                this.Loaded += SettingsWindow_Loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }

            displayWindow = new MainWindow();
            Commando.IsChecked = true;
            BestOf.SelectedIndex = 0;
            BestOf.SelectionChanged += BestOf_SelectionChanged;
            if (BestOf.SelectedItem is ComboBoxItem defaultItem)
                displayWindow?.UpdateTournamentMode(defaultItem.Content.ToString());
        }
        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSavedLanguage();
        }

        private void InitializeTimer()
        {
            stopwatch = new Stopwatch();
            displayTimer = new DispatcherTimer();
            displayTimer.Interval = TimeSpan.FromMilliseconds(20);
            displayTimer.Tick += DisplayTimer_Tick;

            isRunning = false;
            UpdateTimeDisplay();
        }
        private int GetMaxCount()
        {
            if (BestOf.SelectedItem is ComboBoxItem item)
            {
                return item.Tag?.ToString() == "bo5" ? 3 : 2;
            }
            return 2;
        }

        private void BestOf_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int max = GetMaxCount();

            if (int.TryParse(Count1.Text, out int c1) && c1 > max)
                Count1.Text = max.ToString();

            if (int.TryParse(Count2.Text, out int c2) && c2 > max)
                Count2.Text = max.ToString();
            if (BestOf.SelectedItem is ComboBoxItem item)
                displayWindow?.UpdateTournamentMode(item.Content.ToString());
        }
        private void LoadSavedLanguage()
        {
            try
            {
                string savedLanguage = LanguageSettings.Language;

                languageComboBox.SelectionChanged -= LanguageComboBox_SelectionChanged;

                foreach (ComboBoxItem item in languageComboBox.Items)
                {
                    if (item.Tag?.ToString() == savedLanguage)
                    {
                        languageComboBox.SelectedItem = item;
                        break;
                    }
                }

                languageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;

                if (savedLanguage != "en")
                {
                    ChangeLanguage(savedLanguage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки языка: {ex.Message}");
                languageComboBox.SelectedIndex = 0;
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (languageComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                string languageCode = selectedItem.Tag.ToString();

                try
                {
                    ChangeLanguage(languageCode);
                    LanguageSettings.Language = languageCode;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка смены языка: {ex.Message}");
                }
            }
        }

        private void ChangeLanguage(string languageCode)
        {
            try
            {
                var newDict = new ResourceDictionary();
                newDict.Source = new Uri($"Resources/Languages/{languageCode}.xaml", UriKind.Relative);
                var languageDicts = Application.Current.Resources.MergedDictionaries
                    .Where(d => d.Source != null && d.Source.OriginalString.Contains("Resources/Languages/"))
                    .ToList();

                foreach (var dict in languageDicts)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(dict);
                }
                Application.Current.Resources.MergedDictionaries.Add(newDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки языка: {ex.Message}");
            }
        }

        private void DisplayTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
        }

        private void UpdateTimeDisplay()
        {
            var elapsed = stopwatch.Elapsed;
            int totalMinutes = (int)elapsed.TotalMinutes;
            int seconds = elapsed.Seconds;
            int milliseconds = elapsed.Milliseconds / 10;
            string fullTime = $"{totalMinutes:D2}:{seconds:D2}.{milliseconds:D2}";
            TimeDisplay.Text = fullTime;
            displayWindow?.UpdateTimeDisplay(fullTime);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                stopwatch.Start();
                displayTimer.Start();
                isRunning = true;
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                stopwatch.Stop();
                displayTimer.Stop();
                isRunning = false;
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
            }
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            stopwatch.Stop();
            displayTimer.Stop();
            isRunning = false;

            stopwatch.Reset();
            UpdateTimeDisplay();

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = true;
        }
        private void Nickname1_TextChanged(object sender, TextChangedEventArgs e)
        {
            displayWindow?.UpdateNickname1(Nickname1.Text);
        }

        private void Nickname2_TextChanged(object sender, TextChangedEventArgs e)
        {
            displayWindow?.UpdateNickname2(Nickname2.Text);
        }

        private void Seed_TextChanged(object sender, TextChangedEventArgs e)
        {
            displayWindow?.UpdateSeed($"{Seed.Text}");
        }
        private void DecrementButton1_Click(object sender, RoutedEventArgs e)
        {
            int currentValue = int.Parse(Count1.Text);
            if (currentValue > 0)
            {
                Count1.Text = (currentValue - 1).ToString();
            }
        }

        private void IncrementButton1_Click(object sender, RoutedEventArgs e)
        {
            int currentValue = int.Parse(Count1.Text);
            if (currentValue < GetMaxCount())
                Count1.Text = (currentValue + 1).ToString();
        }


        private void DecrementButton2_Click(object sender, RoutedEventArgs e)
        {
            int currentValue = int.Parse(Count2.Text);
            if (currentValue > 0)
            {
                Count2.Text = (currentValue - 1).ToString();
            }
        }

        private void IncrementButton2_Click(object sender, RoutedEventArgs e)
        {
            int currentValue = int.Parse(Count2.Text);
            if (currentValue < GetMaxCount())
                Count2.Text = (currentValue + 1).ToString();
        }

        private void Count1_TextChanged(object sender, TextChangedEventArgs e)
        {
            displayWindow?.UpdateCount1(Count1.Text);
        }

        private void Count2_TextChanged(object sender, TextChangedEventArgs e)
        {
            displayWindow?.UpdateCount2(Count2.Text);
        }

        private void Survivor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox?.SelectedItem == null) return;

            string selectedSurvivor = (comboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            displayWindow?.UpdateSurvivor(selectedSurvivor);
        }

        private void Gen_Seed_Click(object sender, RoutedEventArgs e)
        {
            int randomNumber = random.Next(0, 100000);
            Seed.Text = randomNumber.ToString("D5");
            Clipboard.SetText(Seed.Text);
            ShowNotification();
        }
        private void ShowNotification()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.2)
            };
            DoubleAnimation fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5),
                BeginTime = TimeSpan.FromSeconds(1)
            };

            Storyboard.SetTarget(fadeIn, notificationBorder);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));

            Storyboard.SetTarget(fadeOut, notificationBorder);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));

            storyboard.Children.Add(fadeIn);
            storyboard.Children.Add(fadeOut);
            storyboard.Begin();
        }
        private void LoadStreamButton_Click(object sender, RoutedEventArgs e)
        {
            string twitchUrl = StreamUrlTextBox.Text.Trim();
            LoadStream(displayWindow.StreamWebView, twitchUrl);
        }

        private void LoadStream2Button_Click(object sender, RoutedEventArgs e)
        {
            string twitchUrl = StreamUrl2TextBox.Text.Trim();
            LoadStream(displayWindow.Stream2WebView, twitchUrl);
        }

        private void LoadStream3Button_Click(object sender, RoutedEventArgs e)
        {
            string twitchUrl = StreamUrl3TextBox.Text.Trim();
            LoadStream(displayWindow.Stream3WebView, twitchUrl);
        }

        private void LoadStream4Button_Click(object sender, RoutedEventArgs e)
        {
            string twitchUrl = StreamUrl4TextBox.Text.Trim();
            LoadStream(displayWindow.Stream4WebView, twitchUrl);
        }

        private void LoadStream(WebView2 webView, string twitchUrl)
        {
            if (string.IsNullOrEmpty(twitchUrl))
            {
                MessageBox.Show("Введите ссылку на канал Twitch", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string channelName = ExtractChannelName(twitchUrl);

            if (string.IsNullOrEmpty(channelName))
            {
                MessageBox.Show("Неверный формат ссылки Twitch", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            displayWindow?.LoadTwitchStream(webView, channelName);
        }

        private string ExtractChannelName(string input)
        {
            input = input.Trim().ToLower();

            var patterns = new[]
            {
                @"twitch\.tv/(\w+)/?$",
                @"twitch\.tv/(\w+)/.*",
                @"^([a-zA-Z0-9_]{4,25})$"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Groups[1].Value.ToLower();
                }
            }

            return null;
        }
        private void ShowDisplayButton_Click(object sender, RoutedEventArgs e)
        {
            if (displayWindow == null || !displayWindow.IsLoaded)
            {
                displayWindow = new MainWindow();
            }
            displayWindow?.Show();
            displayWindow.Activate();
        }

        private void HideDisplayButton_Click(object sender, RoutedEventArgs e)
        {
            displayWindow?.Hide();
        }

        private void Navigation_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            displayWindow?.Close();
            Close();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void SurvivorRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Tag is string selectedSurvivor)
            {
                displayWindow.UpdateSurvivor(selectedSurvivor);
            }
        }

        private void LanguageCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void AnimateProperty(FrameworkElement element, DependencyProperty property, double from, double to, double durationMs = 300)
        {
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            element.BeginAnimation(property, animation);
        }

        private void checkCoopMode_Checked(object sender, RoutedEventArgs e)
        {
            if (displayWindow == null) return;

            AnimateProperty(displayWindow.mainStreamContainer, FrameworkElement.HeightProperty, displayWindow.mainStreamContainer.ActualHeight, 337);
            AnimateProperty(displayWindow.coopStreamContainer, FrameworkElement.HeightProperty, displayWindow.coopStreamContainer.ActualHeight, 337);
            AnimateProperty(displayWindow.Stream1Container, FrameworkElement.WidthProperty, displayWindow.Stream1Container.ActualWidth, 600);
            AnimateProperty(displayWindow.Stream2Container, FrameworkElement.WidthProperty, displayWindow.Stream2Container.ActualWidth, 600);
            AnimateProperty(displayWindow.sContainers, FrameworkElement.WidthProperty, displayWindow.sContainers.ActualWidth, 1200);
            AnimateProperty(displayWindow.sContainers, FrameworkElement.HeightProperty, displayWindow.sContainers.ActualHeight, 670);
            AnimateProperty(coop3, FrameworkElement.HeightProperty, coop3.ActualHeight, 20);
            AnimateProperty(coop4, FrameworkElement.HeightProperty, coop4.ActualHeight, 20);
            displayWindow.p1.FontSize = 15;
            displayWindow.p2.FontSize = 15;
            displayWindow.p3.FontSize = 15;
            displayWindow.p4.FontSize = 15;
            playerteam1.SetResourceReference(ContentProperty, "team1");
            playerteam2.SetResourceReference(ContentProperty, "team2");
            nameteam1.SetResourceReference(TextBlock.TextProperty, "team");
            nameteam2.SetResourceReference(TextBlock.TextProperty, "team");
        }

        private void checkCoopMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (displayWindow == null) return;

            AnimateProperty(displayWindow.mainStreamContainer, FrameworkElement.HeightProperty, displayWindow.mainStreamContainer.ActualHeight, 478);
            AnimateProperty(displayWindow.coopStreamContainer, FrameworkElement.HeightProperty, displayWindow.coopStreamContainer.ActualHeight, 0);
            AnimateProperty(displayWindow.Stream1Container, FrameworkElement.WidthProperty, displayWindow.Stream1Container.ActualWidth, 850);
            AnimateProperty(displayWindow.Stream2Container, FrameworkElement.WidthProperty, displayWindow.Stream2Container.ActualWidth, 850);
            AnimateProperty(displayWindow.sContainers, FrameworkElement.WidthProperty, displayWindow.sContainers.ActualWidth, 1700);
            AnimateProperty(displayWindow.sContainers, FrameworkElement.HeightProperty, displayWindow.sContainers.ActualHeight, 650);
            AnimateProperty(coop3, FrameworkElement.HeightProperty, coop3.ActualHeight, 0);
            AnimateProperty(coop4, FrameworkElement.HeightProperty, coop4.ActualHeight, 0);
            displayWindow.p1.FontSize = 25;
            displayWindow.p2.FontSize = 25;
            displayWindow.p3.FontSize = 25;
            displayWindow.p4.FontSize = 25;
            playerteam1.SetResourceReference(ContentProperty, "player1");
            playerteam2.SetResourceReference(ContentProperty, "player2");
            nameteam1.SetResourceReference(TextBlock.TextProperty, "nick");
            nameteam2.SetResourceReference(TextBlock.TextProperty, "nick");
        }
    }
}