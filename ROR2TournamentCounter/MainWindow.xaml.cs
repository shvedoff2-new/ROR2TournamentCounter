using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ROR2TournamentCounter
{
    public partial class MainWindow : Window
    {
        private const double BaseFontSize = 75;
        private const int BaseCharLimit = 7;
        private double _maskOffsetX = 0; // смещение паттерна
        private const double Speed = 100; // пикселей в секунду (регулируй под себя)
        private DateTime _lastRender;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                InitializeWebView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void Navigation_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private async void InitializeWebView()
        {
            try
            {
                string runtimePath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "WebView2Runtime"
                );

                // Используем AppData - там всегда есть права на запись
                string userDataPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ROR2TournamentCounter",
                    "WebView2Data"
                );

                // Создаём папку, если её нет
                if (!System.IO.Directory.Exists(userDataPath))
                {
                    System.IO.Directory.CreateDirectory(userDataPath);
                }

                var environment = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(
                    browserExecutableFolder: runtimePath,
                    userDataFolder: userDataPath
                );

                await StreamWebView.EnsureCoreWebView2Async(environment);
                await Stream2WebView.EnsureCoreWebView2Async(environment);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации WebView2: {ex.Message}\n\n{ex.StackTrace}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation moveMaskAnimation = new DoubleAnimation
            {
                From = 0,
                To = 5000,
                Duration = TimeSpan.FromSeconds(25000),
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = false
            };

            MaskTranslateTransform.BeginAnimation(TranslateTransform.XProperty, moveMaskAnimation);
        }

        // Методы для обновления отображения из окна настроек
        public void UpdateTimeDisplay(string timeText)
        {
            Dispatcher.Invoke(() =>
            {
                TimeDisplay.Text = timeText;

                if (timeText.Contains("."))
                {
                    string[] parts = timeText.Split('.');
                    MinutesSecondsRun.Text = parts[0];
                    DotMillisecondsRun.Text = "." + parts[1];
                    string minutesPart = parts[0].Split(':')[0];
                    if (minutesPart.Length >= 3)
                    {
                        TimeDisplayed.FontSize = 43;
                    }
                    else
                    {
                        TimeDisplayed.FontSize = 48;
                    }
                }
                else
                {
                    MinutesSecondsRun.Text = timeText;
                    DotMillisecondsRun.Text = ".00";
                    if (timeText.Contains(":"))
                    {
                        string minutesPart = timeText.Split(':')[0];
                        if (minutesPart.Length >= 3)
                        {
                            TimeDisplayed.FontSize = 43;
                        }
                        else
                        {
                            TimeDisplayed.FontSize = 48;
                        }
                    }
                }
            });
        }

        public void UpdateNickname1(string nickname)
        {
            Dispatcher.Invoke(() =>
            {
                Nickname1Player.Text = nickname;
                UpdateNicknameFont(Nickname1Player);
            });
        }

        public void UpdateNickname2(string nickname)
        {
            Dispatcher.Invoke(() =>
            {
                Nickname2Player.Text = nickname;
                UpdateNicknameFont(Nickname2Player);
            });
        }

        public void UpdateTournamentName(string name)
        {
            Dispatcher.Invoke(() =>
            {
                TournamentNameTB.Text = name;
            });
        }

        public void UpdateSeed(string seed)
        {
            Dispatcher.Invoke(() =>
            {
                SeedTB.Text = seed;
            });
        }

        public void UpdateCount1(string count)
        {
            Dispatcher.Invoke(() =>
            {
                CountPlayer1.Text = count;
            });
        }

        public void UpdateCount2(string count)
        {
            Dispatcher.Invoke(() =>
            {
                CountPlayer2.Text = count;
            });
        }

        public void UpdateSurvivor(string selectedSurvivor)
        {
            Dispatcher.Invoke(() =>
            {
                System.Windows.Media.Color survivorColor = Colors.Gray;

                switch (selectedSurvivor)
                {
                    case "Acrid":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Acrid.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#541a45");
                        break;
                    case "Artificer":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Artificer.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#a9a9a9");
                        break;
                    case "Bandit":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Bandit.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#177978");
                        break;
                    case "Captain":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Captain.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#132240");
                        break;
                    case "CHEF":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/CHEF.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#535b82");
                        break;
                    case "Commando":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Commando.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#826529");
                        break;
                    case "Engineer":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Engineer.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#390882");
                        break;
                    case "False Son":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/False_Son.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#464064");
                        break;
                    case "Heretic":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Heretic.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#450004");
                        break;
                    case "Huntress":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Huntress.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#781038");
                        break;
                    case "Loader":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Loader.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#02326f");
                        break;
                    case "Mercenary":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Mercenary.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#02326f");
                        break;
                    case "MUL-T":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/MUL-T.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#797a4a");
                        break;
                    case "Railgunner":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Railgunner.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#383058");
                        break;
                    case "REX":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/REX.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#5d8f13");
                        break;
                    case "Seeker":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Seeker.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2f6d61");
                        break;
                    case "Void Fiend":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Void_Fiend.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#71297c");
                        break;
                    case "Operator":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Operator.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#025d51");
                        break;
                    case "Drifter":
                        SurvivorPic.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Drifter.png"));
                        survivorColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#974209");
                        break;
                    default:
                        SurvivorPic.Source = null;
                        survivorColor = Colors.Gray;
                        break;
                }
                SurvivorRect.Stroke = new SolidColorBrush(survivorColor);
                SurvivorPolygonMask.Fill = new SolidColorBrush(survivorColor);
            });
        }

        public void LoadTwitchStream(string channelName)
        {
            try
            {
                StreamWebView.Visibility = Visibility.Visible;
                string embedUrl = $"https://player.twitch.tv/?channel={channelName}&parent=localhost&muted=false&autoplay=true";
                StreamWebView.CoreWebView2.Navigate(embedUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки стрима: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Load2TwitchStream(string channelName)
        {
            try
            {
                Stream2WebView.Visibility = Visibility.Visible;
                string embedUrl = $"https://player.twitch.tv/?channel={channelName}&parent=localhost&muted=false&autoplay=true";
                Stream2WebView.CoreWebView2.Navigate(embedUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки стрима: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateNicknameFont(TextBox textBox)
        {
            int textLength = textBox.Text.Length;

            if (textLength <= BaseCharLimit)
            {
                textBox.FontSize = BaseFontSize;
            }
            else
            {
                double scaleFactor = (double)BaseCharLimit / textLength;
                double newFontSize = BaseFontSize * scaleFactor;
                textBox.FontSize = Math.Max(newFontSize, 35);
            }
        }
    }
}