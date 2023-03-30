using System;
using Xamarin.Forms;
using System.IO;
using System.Reflection;
using Fallout4_Calculator.Services;

namespace Fallout4_Calculator
{
    public partial class App : Application
    {
        public const string DATABASE_NAME = "mainbd.db";

        public static MainDataBaseAsyncRepository database;
        public static MainDataBaseAsyncRepository Database
        {
            get 
            {
                if (database == null)
                {
                    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DATABASE_NAME);
                    if (!File.Exists(dbPath))
                    {
                        var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
                        using (Stream stream = assembly.GetManifestResourceStream($"Fallout4_Calculator.{DATABASE_NAME}"))
                        {
                            using (FileStream fs = new FileStream(dbPath, FileMode.OpenOrCreate))
                            {
                                if (stream != null)
                                {
                                    stream.CopyTo(fs);
                                    fs.Flush();
                                }
                            }
                        }
                    }
                    database = new MainDataBaseAsyncRepository(dbPath);
                }
                return database;
            }
        }
        public App()
        {
            InitializeComponent();
            database = Database;
            MainPage = new AppShell();
        }
        protected override void OnStart()
        {
        }
        protected override void OnSleep()
        {
        }
        protected override void OnResume()
        {
        }
    }
}