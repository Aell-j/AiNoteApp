
using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LocalJsonModule.Data;
using LocalJsonModule.Models;
using LocalJsonModule.Repositories;
using LocalJsonModule.Repositories.postgresSQL;
using LocalJsonModule.Services;
using NoteApp.Windows;

namespace NoteApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var pathProvider = new DataPathProvider();
        
        //   var connectionFactory = new DbConnectionFactory(
        //       "Host=94.41.85.115;Port=54321;Database=webnotes;Username=postgres;Password=1111");
        //   IUserRepository userRepo = new UserPostgresRepository(connectionFactory);
        IUserRepository userRepo = new UserJsonRepository(pathProvider);

        var folderRepo = new FolderJsonRepository(pathProvider);
        var noteRepo = new NoteJsonRepository(pathProvider);

        var password = new PasswordService();
        var auth = new AuthService(userRepo, password);
        var register = new RegisterService(userRepo, password);

        var folders = new FolderService(folderRepo, userRepo, noteRepo);
        var notes = new NoteService(noteRepo, userRepo, folderRepo);

        var aiBaseUrl = "http://127.0.0.1:5156/";
        var ai = new AiAnalysisService(aiBaseUrl);
        var aiHost = new LocalAiApiHost(aiBaseUrl);

        if (ApplicationLifetime is
            IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += (_, _) => aiHost.Dispose();

           /* var loginWindow = new LoginWindow(
                auth,
                () => new RegisterWindow(register, auth)
            );

            desktop.MainWindow = loginWindow;

            loginWindow.Closed += (_, _) =>
            {
                if (loginWindow.AuthenticatedUser is not null)
                {
                    var mainWindow = new MainWindow(
                        folders,
                        notes,
                        loginWindow.AuthenticatedUser,
                        ai,
                        aiHost
                    );

                    desktop.MainWindow = mainWindow;
                    mainWindow.Show();
                }
                else
                {
                    desktop.Shutdown();
                }
            };*/
           
            // Тестовый запуск без входа в аккаунт так как база лежит :((((((
            var offlineUserId = new Guid("9d99977f-5ae8-4231-954c-01aabff0677a");

            var debugUser = Task.Run(
                () => userRepo.GetByIdAsync(offlineUserId)).GetAwaiter().GetResult();

            if (debugUser is null)
            {
                debugUser = new User
                {
                    Id = offlineUserId,
                    Login = "offline",
                    Email = "offline@localhost",
                    CreatedAt = DateTime.UtcNow,
                };

                Task.Run(() => userRepo.AddAsync(debugUser)).GetAwaiter().GetResult();
            }

            desktop.MainWindow = new MainWindow(
                folders,
                notes,
                debugUser,
                ai,
                aiHost
            );
        }

        base.OnFrameworkInitializationCompleted();
    }
}
