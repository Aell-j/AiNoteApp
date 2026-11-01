using Avalonia.Controls;
using Avalonia.Interactivity;

namespace NoteApp;

public partial class InputDialog : Window
{
    public string? Result { get; private set; }

    public InputDialog() => InitializeComponent();

    public InputDialog(string initialValue) : this()
    {
        this.FindControl<TextBox>("InputBox")!.Text = initialValue;
    }

    private void OnOk(object? sender, RoutedEventArgs e)
    {
        Result = this.FindControl<TextBox>("InputBox")!.Text;
        Close(Result);
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close(null);
}