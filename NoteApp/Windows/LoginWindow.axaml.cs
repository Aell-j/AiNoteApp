using System;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LocalJsonModule.DTOs.Users;
using LocalJsonModule.Models;
using LocalJsonModule.Services;
using Microsoft.VisualBasic;
using NoteApp.Windows;
using Tmds.DBus.Protocol;

namespace NoteApp;

public partial class LoginWindow : Window
{
    // Без параметров нужен только XAML-дизайнеру: в рантайме окно создаёт App с сервисами.
    private readonly IAuthService _auth = null!;
    private readonly Func<RegisterWindow> _register = null!;

    
    public User? AuthenticatedUser { get; private set; }
    
    public LoginWindow()
    {
        InitializeComponent();
    }
    public LoginWindow(
        IAuthService authService,
        Func<RegisterWindow> register)
    {
        InitializeComponent();

        _auth = authService;
        _register = register;

        this.FindControl<Button>("LoginButton")!.Click += OnLoginClick;
        this.FindControl<Button>("RegisterButton")!.Click += OnRegisterClick;
    }
    
    private async void OnLoginClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var login = this.FindControl<TextBox>("LoginTextBox")!.Text ?? "";
            var pass = this.FindControl<TextBox>("PasswordTextBox")!.Text ?? "";

            var user = await _auth.LoginAsync(new AuthUserRequest
            {
                Login = login,
                Password = pass
            });

            if (user is null)
            {
                await ShowMessageAsync("Неверный логин или пароль");
                return;
            }
            
            AuthenticatedUser = user;
            Close();
        }
        catch (UnauthorizedAccessException)
        {
            await ShowMessageAsync("Неверный логин или пароль");
        }
        catch (ArgumentException ex)
        {
            await ShowMessageAsync(ex.Message);
        }
        catch (Exception)
        {
            await ShowMessageAsync(
                "Не удалось выполнить вход. Проверьте подключение и попробуйте снова.");
        }
    }

    private async void OnRegisterClick(object? sender, RoutedEventArgs e)
    {
        var registerWindow = _register();

        await registerWindow.ShowDialog(this);
        if (registerWindow.AuthenticatedUser is not null)
        {
            AuthenticatedUser = registerWindow.AuthenticatedUser;
            Close();
        }
    }
    
    
    private Task ShowMessageAsync(string message)
    {
        MessageText.Text = message;
        MessageOverlay.IsVisible = true;

        return Task.CompletedTask;
    }

    private void OnMessageCloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MessageOverlay.IsVisible = false;
    }
}