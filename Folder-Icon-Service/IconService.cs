using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace FolderIconService
{
    public partial class IconService : ServiceBase
    {
        private Timer _timer;
        private readonly string _configFile;
        private Dictionary<string, string> _folderIcons;

        public IconService()
        {
            InitializeComponent(); // Этот метод берется из IconService.Designer.cs
            ServiceName = "FolderIconService";
            _configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FolderIcons.config");
            _folderIcons = new Dictionary<string, string>();
        }

        protected override void OnStart(string[] args)
        {
            LoadConfiguration();

            _timer = new Timer(30000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();

            ApplyIconsToAllFolders();
            Log("Служба запущена");
        }

        protected override void OnStop()
        {
            _timer?.Stop();
            _timer?.Dispose();
            Log("Служба остановлена");
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                LoadConfiguration();
                ApplyIconsToAllFolders();
            }
            catch (Exception ex)
            {
                Log($"Ошибка в таймере: {ex.Message}");
            }
        }

        private void LoadConfiguration()
        {
            _folderIcons = new Dictionary<string, string>();

            if (File.Exists(_configFile))
            {
                var lines = File.ReadAllLines(_configFile);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]) && !string.IsNullOrWhiteSpace(parts[1]))
                    {
                        _folderIcons[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }
        }

        private void ApplyIconsToAllFolders()
        {
            foreach (var pair in _folderIcons)
            {
                try
                {
                    ApplyIconToFolder(pair.Key, pair.Value);
                }
                catch (Exception ex)
                {
                    Log($"Ошибка применения иконки к {pair.Key}: {ex.Message}");
                }
            }
        }

        private void ApplyIconToFolder(string folderPath, string iconPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"Папка не найдена: {folderPath}");

            if (!File.Exists(iconPath))
                throw new FileNotFoundException($"Файл иконки не найден: {iconPath}");

            string iniPath = Path.Combine(folderPath, "desktop.ini");

            string iniContent = $@"[.ShellClassInfo]
IconResource={iconPath},0
";
            File.WriteAllText(iniPath, iniContent, System.Text.Encoding.Unicode);
            File.SetAttributes(iniPath, FileAttributes.Hidden | FileAttributes.System);

            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            dirInfo.Attributes |= FileAttributes.System;

            RefreshIcons();
        }

        private void RefreshIcons()
        {
            NativeMethods.SHChangeNotify(
                0x8000000,
                0x1000,
                IntPtr.Zero,
                IntPtr.Zero
            );
        }

        private void Log(string message)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServiceLog.txt");
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

            try
            {
                File.AppendAllText(logPath, logEntry + Environment.NewLine);
            }
            catch { }
        }
    }
}