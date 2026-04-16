using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        private void AnimateTranslateX(FrameworkElement element, double to, double durationMs = 300)
        {
            TranslateTransform transform = element.RenderTransform as TranslateTransform;

            if (transform == null)
            {
                transform = new TranslateTransform();
                element.RenderTransform = transform;
            }

            var animation = new DoubleAnimation
            {
                To = to,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            transform.BeginAnimation(TranslateTransform.XProperty, animation);
        }
        private void AnimateTranslateY(FrameworkElement element, double to, double durationMs = 300)
        {
            TranslateTransform transform = element.RenderTransform as TranslateTransform;

            if (transform == null)
            {
                transform = new TranslateTransform();
                element.RenderTransform = transform;
            }

            var animation = new DoubleAnimation
            {
                To = to,
                Duration = TimeSpan.FromMilliseconds(durationMs),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            transform.BeginAnimation(TranslateTransform.YProperty, animation);
        }

        private void checkCoopMode_Checked(object sender, RoutedEventArgs e)
        {
            if (displayWindow == null) return;
            screen_team1.Visibility = Visibility.Visible;
            screen_team2.Visibility = Visibility.Visible;
            screen_3.Visibility = Visibility.Visible;
            screen_4.Visibility = Visibility.Visible;
            screen_3_4.Visibility = Visibility.Visible;
            displayWindow.Stream3Container.Visibility = Visibility.Visible;
            displayWindow.Stream4Container.Visibility = Visibility.Visible;
            screen_all.Visibility = Visibility.Visible;
            Animate(674, 600, 337, 600, 337, 600, 337, 600, 337, 1200, 670, 0, 0, 0, 0, 0, 0, 0, 0);
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
            screen_team1.Visibility = Visibility.Hidden;
            screen_team2.Visibility = Visibility.Hidden;
            screen_3.Visibility = Visibility.Hidden;
            screen_4.Visibility = Visibility.Hidden;
            screen_3_4.Visibility = Visibility.Hidden;
            displayWindow.Stream3Container.Visibility = Visibility.Hidden;
            displayWindow.Stream4Container.Visibility = Visibility.Hidden;
            screen_all.Visibility = Visibility.Hidden;
            Animate(673, 850, 478, 850, 478, 347, 195, 347, 195, 1700, 673, 0, 0, 0, 0, 503, 0, -503, 0);
            AnimateProperty(coop3, FrameworkElement.HeightProperty, coop3.ActualHeight, 0);
            AnimateProperty(coop4, FrameworkElement.HeightProperty, coop4.ActualHeight, 0);
            displayWindow.p1.FontSize = 25;
            displayWindow.p2.FontSize = 25;
            playerteam1.SetResourceReference(ContentProperty, "player1");
            playerteam2.SetResourceReference(ContentProperty, "player2");
            nameteam1.SetResourceReference(TextBlock.TextProperty, "nick");
            nameteam2.SetResourceReference(TextBlock.TextProperty, "nick");
        }

        private void screen_1_Click(object sender, RoutedEventArgs e)
        {
            Animate(650, 1156, 650, 347, 195, 347, 195, 347, 195, 1503, 650, 0, 0, 0, 195, 1156, -455, 0, -65);
        }

        private void screen_2_Click(object sender, RoutedEventArgs e)
        {
            Animate(650, 347, 195, 1156, 650, 347, 195, 347, 195, 1503, 650, 1156, 195, -347, 0, 1156, -65, 0, -455);
        }

        private void screen_1_2_Click(object sender, RoutedEventArgs e)
        {
            Animate(673, 850, 478, 850, 478, 347, 195, 347, 195, 1700, 673, 0, 0, 0, 0, 503, 0, -503, 0);
        }

        private void screen_team1_Click(object sender, RoutedEventArgs e)
        {
            Animate(673, 850, 478, 347, 195, 850, 478, 347, 195, 1700, 673, 0, 0, -850, 478, 850, -195, -503, 0);
        }

        private void screen_3_Click(object sender, RoutedEventArgs e)
        {
            Animate(650, 347, 195, 347, 195, 1156, 650, 347, 195, 1503, 650, 1156, 0, 0, 195, 0, 0, 0, -65);
        }

        private void screen_4_Click(object sender, RoutedEventArgs e)
        {
            Animate(650, 347, 195, 347, 195, 347, 195, 1156, 650, 1503, 650, 1156, 195, 0, 0, 1156, -65, -347, 0);
        }

        private void screen_team2_Click(object sender, RoutedEventArgs e)
        {
            Animate(673, 347, 195, 850, 478, 347, 195, 850, 478, 1700, 673, 503, 478, -850, 0, 850, 0, 0, -195);
        }

        private void screen_all_Click(object sender, RoutedEventArgs e)
        {
            Animate(674, 600, 337, 600, 337, 600, 337, 600, 337, 1200, 670, 0, 0, 0, 0, 0, 0, 0, 0);
        }
        private void screen_3_4_Click(object sender, RoutedEventArgs e)
        {
            Animate(673, 347, 195, 347, 195, 850, 478, 850, 478, 1700, 673, 503, 478, -503, 478, 0, -195, 0, -195);
        }
        private void Animate(int contHeight, int width1, int height1, int width2, int height2, int width3, int height3, int width4, int height4, int sContWidth, int sContHeight, int s1cont_x, int s1cont_y, int s2cont_x, int s2cont_y, int s3cont_x, int s3cont_y, int s4cont_x, int s4cont_y)
        {
            AnimateProperty(displayWindow.mainStreamContainer, FrameworkElement.HeightProperty, displayWindow.mainStreamContainer.ActualHeight, contHeight);
            AnimateProperty(displayWindow.Stream1Container, FrameworkElement.WidthProperty, displayWindow.Stream1Container.ActualWidth, width1);
            AnimateProperty(displayWindow.Stream1Container, FrameworkElement.HeightProperty, displayWindow.Stream1Container.ActualHeight, height1);
            AnimateProperty(displayWindow.Stream2Container, FrameworkElement.WidthProperty, displayWindow.Stream2Container.ActualWidth, width2);
            AnimateProperty(displayWindow.Stream2Container, FrameworkElement.HeightProperty, displayWindow.Stream2Container.ActualHeight, height2);
            AnimateProperty(displayWindow.Stream3Container, FrameworkElement.WidthProperty, displayWindow.Stream3Container.ActualWidth, width3);
            AnimateProperty(displayWindow.Stream3Container, FrameworkElement.HeightProperty, displayWindow.Stream3Container.ActualHeight, height3);
            AnimateProperty(displayWindow.Stream4Container, FrameworkElement.WidthProperty, displayWindow.Stream4Container.ActualWidth, width4);
            AnimateProperty(displayWindow.Stream4Container, FrameworkElement.HeightProperty, displayWindow.Stream4Container.ActualHeight, height4);
            AnimateProperty(displayWindow.sContainers, FrameworkElement.WidthProperty, displayWindow.sContainers.ActualWidth, sContWidth);
            AnimateProperty(displayWindow.sContainers, FrameworkElement.HeightProperty, displayWindow.sContainers.ActualHeight, sContHeight);
            AnimateTranslateX(displayWindow.Stream1Container, s1cont_x);
            AnimateTranslateY(displayWindow.Stream1Container, s1cont_y);
            AnimateTranslateX(displayWindow.Stream2Container, s2cont_x);
            AnimateTranslateY(displayWindow.Stream2Container, s2cont_y);
            AnimateTranslateX(displayWindow.Stream3Container, s3cont_x);
            AnimateTranslateY(displayWindow.Stream3Container, s3cont_y);
            AnimateTranslateX(displayWindow.Stream4Container, s4cont_x);
            AnimateTranslateY(displayWindow.Stream4Container, s4cont_y);
        }
    }
}