using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LocalJsonModule.DTOs.Users;
using LocalJsonModule.Models;
using LocalJsonModule.Services;

namespace NoteApp.Windows;

public partial class RegisterWindow : Window
{
    // Без параметров нужен только XAML-дизайнеру: в рантайме окно создаёт App с сервисами.
    private readonly IRegisterService _register = null!;
    
    public User? AuthenticatedUser { get; private set; }
    
    public RegisterWindow()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
            return;
        
        this.FindControl<Button>("RegisterButton")!.Click += onRegisterClick;
        this.FindControl<Button>("LoginButton")!.Click += onLoginClick;
    }
    
    public RegisterWindow(IRegisterService register, IAuthService auth) : this()
    {
        _register = register;
    }


    private async void onRegisterClick(object? sender, RoutedEventArgs e)
    {
        var email = this.FindControl<TextBox>("EmailTextBox")!.Text ?? "";
        var login = this.FindControl<TextBox>("LoginTextBox")!.Text ?? "";
        var pass  = this.FindControl<TextBox>("PasswordTextBox")!.Text ?? "";

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(login) ||
            string.IsNullOrWhiteSpace(pass))
        {
            await ShowMessageAsync("Заполните все поля регистрации.");
            return;
        }
        
        try
        {
            var user = await _register.RegisterAsync(new RegisterUserRequest
            {
                Login    = login,
                Email    = email,
                Password = pass
            });

            if (user is null)
            {
                await ShowMessageAsync("Регистрация не удалась");
                return;
            }
            
            AuthenticatedUser = user;
            Close();
        }
        catch (Exception ex)
        {
            await ShowMessageAsync("Ошибка регистрации: " + ex.Message);
        }
    }
    private void onLoginClick(object? sender, RoutedEventArgs e)
    {
        AuthenticatedUser = null;
        Close();
    }
    private Task ShowMessageAsync(string message)
    {
        var overlay = this.FindControl<Border>("MessageOverlay");
        var text    = this.FindControl<TextBlock>("MessageText");
        if (overlay is null || text is null) return Task.CompletedTask;

        text.Text = message;
        overlay.IsVisible = true;
        return Task.CompletedTask;
    }

    private void OnMessageCloseClick(object? sender, RoutedEventArgs e)
    {
        this.FindControl<Border>("MessageOverlay")!.IsVisible = false;
    }
}