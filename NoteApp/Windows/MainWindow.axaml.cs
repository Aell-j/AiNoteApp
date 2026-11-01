
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using LocalJsonModule.DTOs.Folders;
using LocalJsonModule.DTOs.Notes;
using LocalJsonModule.Models;
using LocalJsonModule.Services;

namespace NoteApp;

public partial class MainWindow : Window
{
    private enum NoteFilter
    {
        All,
        Favorite,
        Recent,
        Archive
    }

    private const double LeftWidth = 238;
    private const double RightWidth = 390;
    private const double RailWidth = 74;
    
    private readonly IFolderService _folders = null!;
    private readonly INoteService _notes = null!;
    private readonly IAiAnalysisService _ai = null!;
    private readonly LocalAiApiHost _aiHost = null!;
    private readonly User _user = null!;

    private List<Folder> _foldersList = new();
    private List<Note> _notesList = new();

    private NoteFilter _filter = NoteFilter.All;
    private Guid? _selectedFolderId;
    private Note? _editingNote;

    private bool _updatingTree;
    private bool _leftCollapsed;
    private bool _rightCollapsed;
    private bool _aiRunning;

    private CancellationTokenSource? _aiCancellation;

    public MainWindow() => InitializeComponent();

    public MainWindow(
        IFolderService folders,
        INoteService notes,
        User user,
        IAiAnalysisService ai,
        LocalAiApiHost aiHost)
    {
        InitializeComponent();

        _folders = folders;
        _notes = notes;
        _user = user;
        _ai = ai;
        _aiHost = aiHost;

        InitializeUser();
        ConnectEvents();

        Loaded += async (_, _) =>
        {
            await ReloadAsync();
            await PrepareAiPanelAsync();
        };
    }

    // Инициализация

    private void InitializeUser()
    {
        UserNameText.Text = _user.Login;

        UserEmailText.Text = string.IsNullOrWhiteSpace(_user.Email)
            ? "Личный аккаунт"
            : _user.Email;

        UserInitialText.Text = string.IsNullOrEmpty(_user.Login)
            ? "?"
            : _user.Login[..1].ToUpperInvariant();
    }

    private void ConnectEvents()
    {
        AddNoteButton.Click += OnAddNoteClick;
        AddFolderButton.Click += OnAddFolderClick;

        FilterAllButton.Click += (_, _) => SelectFilter(NoteFilter.All);
        FilterFavoriteButton.Click += (_, _) => SelectFilter(NoteFilter.Favorite);
        FilterRecentButton.Click += (_, _) => SelectFilter(NoteFilter.Recent);
        FilterArchiveButton.Click += (_, _) => SelectFilter(NoteFilter.Archive);

        CollapseLeftButton.Click += (_, _) => SetLeftCollapsed(true);
        ExpandLeftButton.Click += (_, _) => SetLeftCollapsed(false);
        CollapseRightButton.Click += (_, _) => SetRightCollapsed(true);
        ExpandRightButton.Click += (_, _) => SetRightCollapsed(false);

        SearchTextBox.TextChanged += (_, _) => RenderNotes();
        NotesTreeView.SelectionChanged += OnTreeSelectionChanged;

        BackButton.Click += (_, _) => CloseEditor();
        SaveButton.Click += OnSaveClick;
        DeleteButton.Click += OnDeleteClick;
        FavoriteButton.Click += OnFavoriteClick;
        ArchiveButton.Click += OnArchiveClick;

        GenerateButton.Click += OnGenerateClick;
    }

    // Загрузка данных

    private async Task ReloadAsync()
    {
        try
        {
            _foldersList = await _folders.GetByOwnerIdAsync(_user.Id);
            _notesList = await _notes.GetByOwnerIdAsync(_user.Id);
        }
        catch (Exception ex)
        {
            ShowMessage("Не удалось загрузить данные: " + ex.Message);
            EmptyStateText.IsVisible = true;
            return;
        }

        RebuildTree();
        RenderNotes();
    }

    // Дерево папок

    private void RebuildTree()
    {
        _updatingTree = true;

        try
        {
            NotesTreeView.Items.Clear();

            foreach (var folder in _foldersList.Where(f => f.ParentFolderId == null))
                NotesTreeView.Items.Add(BuildFolderItem(folder));

            foreach (var note in _notesList.Where(n => n.FolderId == null && !n.IsArchive))
                NotesTreeView.Items.Add(BuildNoteItem(note));
        }
        finally
        {
            _updatingTree = false;
        }
    }

    private TreeViewItem BuildFolderItem(Folder folder)
    {
        var item = new TreeViewItem
        {
            Header = folder.Title,
            Tag = folder
        };

        foreach (var child in _foldersList.Where(f => f.ParentFolderId == folder.Id))
            item.Items.Add(BuildFolderItem(child));

        foreach (var note in _notesList.Where(n => n.FolderId == folder.Id && !n.IsArchive))
            item.Items.Add(BuildNoteItem(note));

        return item;
    }

    private static TreeViewItem BuildNoteItem(Note note)
    {
        return new TreeViewItem
        {
            Header = note.Title,
            Tag = note
        };
    }

    private async void OnTreeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_updatingTree)
            return;

        switch (NotesTreeView.SelectedItem)
        {
            case TreeViewItem { Tag: Note note }:
                await OpenEditorAsync(note);
                break;

            case TreeViewItem { Tag: Folder folder }:
                _selectedFolderId = folder.Id;
                CloseEditor();
                RenderNotes();
                break;

            default:
                _selectedFolderId = null;
                RenderNotes();
                break;
        }
    }

    // Поиск и фильтрация

    private void SelectFilter(NoteFilter filter)
    {
        _filter = filter;
        _selectedFolderId = null;

        UpdateFilterButtons();
        CloseEditor();
        RenderNotes();
    }

    private void UpdateFilterButtons()
    {
        var buttons = new Dictionary<Button, NoteFilter>
        {
            [FilterAllButton] = NoteFilter.All,
            [FilterFavoriteButton] = NoteFilter.Favorite,
            [FilterRecentButton] = NoteFilter.Recent,
            [FilterArchiveButton] = NoteFilter.Archive
        };

        foreach (var (button, filter) in buttons)
        {
            if (filter == _filter)
                button.Classes.Add("active");
            else
                button.Classes.Remove("active");
        }
    }

    private void RenderNotes()
    {
        var notes = GetFilteredNotes();

        var folderTitle = _selectedFolderId is { } id
            ? _foldersList.FirstOrDefault(f => f.Id == id)?.Title
            : null;

        NotesHost.ItemsSource = notes
            .Select(note => new NoteCard(note, GetFolderTitle(note)))
            .ToList();

        EmptyStateText.IsVisible = notes.Count == 0;
        NotesCountBadge.Text = FormatNoteCount(notes.Count);

        SectionHeaderText.Text = folderTitle == null
            ? GetFilterTitle()
            : $"Папка: {folderTitle}";
    }

    private List<Note> GetFilteredNotes()
    {
        IEnumerable<Note> notes = _filter switch
        {
            NoteFilter.Favorite => _notesList.Where(n => !n.IsArchive && n.IsFavorite),

            NoteFilter.Recent => _notesList.Where(n =>
                !n.IsArchive &&
                n.UpdatedAt >= DateTime.UtcNow.AddDays(-7)),

            NoteFilter.Archive => _notesList.Where(n => n.IsArchive),

            _ => _notesList.Where(n => !n.IsArchive)
        };

        if (_selectedFolderId is { } folderId)
            notes = notes.Where(n => n.FolderId == folderId);

        var search = SearchTextBox.Text?.Trim();

        if (!string.IsNullOrEmpty(search))
        {
            notes = notes.Where(n =>
                n.Title.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                n.Content.Contains(search, StringComparison.CurrentCultureIgnoreCase));
        }

        return notes.OrderByDescending(n => n.UpdatedAt).ToList();
    }

    private string GetFilterTitle()
    {
        return _filter switch
        {
            NoteFilter.Favorite => "Избранное",
            NoteFilter.Recent => "Недавние за 7 дней",
            NoteFilter.Archive => "Корзина",
            _ => "Недавно изменённые"
        };
    }

    // Редактор заметок

    private async void OnNotesHostTapped(object? sender, TappedEventArgs e)
    {
        for (var visual = e.Source as Visual;
             visual != null;
             visual = visual.GetVisualParent())
        {
            if (visual.DataContext is NoteCard card)
            {
                await OpenEditorAsync(card.Note);
                return;
            }
        }
    }

    private async Task OpenEditorAsync(Note note)
    {
        var freshNote = await _notes.GetByIdAsync(note.Id) ?? note;

        _editingNote = freshNote;

        EditorTitleTextBox.Text = freshNote.Title;
        EditorContentTextBox.Text = freshNote.Content;
        EditorContextText.Text = GetFolderTitle(freshNote) ?? "Без папки";
        EditorStampText.Text = "изменено " + NoteFormat.FormatDate(freshNote.UpdatedAt);

        UpdateEditorButtons(freshNote);
        SetEditorStatus("");

        NotesListScroll.IsVisible = false;
        EditorView.IsVisible = true;
    }

    private void CloseEditor()
    {
        if (_editingNote == null)
            return;

        _editingNote = null;

        EditorView.IsVisible = false;
        NotesListScroll.IsVisible = true;
    }

    private void UpdateEditorButtons(Note note)
    {
        FavoriteButton.Content = note.IsFavorite
            ? "★ В избранном"
            : "★ В избранное";

        ArchiveButton.Content = note.IsArchive
            ? "Восстановить"
            : "В корзину";
    }

    private void SetEditorStatus(string message, bool isError = true)
    {
        EditorStatusText.Text = message;
        EditorStatusText.Foreground = CreateBrush(
            isError ? "#B84638" : "#427448");
    }

    // Сохранение заметки

    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        if (_editingNote is not { } note)
            return;

        var title = EditorTitleTextBox.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(title))
        {
            SetEditorStatus("Заголовок не может быть пустым.");
            return;
        }

        try
        {
            await _notes.UpdateAsync(note.Id, new UpdateNoteRequest
            {
                Title = title,
                Content = EditorContentTextBox.Text ?? "",
                FolderId = note.FolderId,
                IsFavorite = note.IsFavorite,
                IsArchive = note.IsArchive,
                NoteType = note.NoteType
            });

            EditorStampText.Text =
                "изменено " + NoteFormat.FormatDate(DateTime.UtcNow);

            SetEditorStatus("Сохранено.", false);

            await ReloadAsync();

            _editingNote = await _notes.GetByIdAsync(note.Id) ?? note;
        }
        catch (Exception ex)
        {
            ShowMessage("Не удалось сохранить заметку: " + ex.Message);
        }
    }

    // Действия с заметками

    private async void OnDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (_editingNote is not { } note)
            return;

        try
        {
            await _notes.DeleteAsync(note.Id);

            CloseEditor();
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            ShowMessage("Не удалось удалить заметку: " + ex.Message);
        }
    }

    private async void OnFavoriteClick(object? sender, RoutedEventArgs e)
    {
        if (_editingNote is not { } note)
            return;

        try
        {
            await _notes.EnableToFavoriteAsync(note.Id);
            await UpdateNoteAfterActionAsync(note.Id);
        }
        catch (Exception ex)
        {
            ShowMessage("Не удалось изменить избранное: " + ex.Message);
        }
    }

    private async void OnArchiveClick(object? sender, RoutedEventArgs e)
    {
        if (_editingNote is not { } note)
            return;

        try
        {
            if (note.IsArchive)
                await _notes.DisableToArchiveAsync(note.Id);
            else
                await _notes.EnableToArchiveAsync(note.Id);

            await UpdateNoteAfterActionAsync(note.Id);
        }
        catch (Exception ex)
        {
            ShowMessage("Не удалось изменить архив: " + ex.Message);
        }
    }

    private async Task UpdateNoteAfterActionAsync(Guid noteId)
    {
        var note = await _notes.GetByIdAsync(noteId);

        if (note != null)
        {
            _editingNote = note;
            UpdateEditorButtons(note);
        }

        await ReloadAsync();
    }

    // Создание заметок и папок

    private async void OnAddNoteClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var note = await _notes.CreateAsync(new CreateNoteRequest
            {
                Title = "Новая заметка " + DateTime.Now.ToString("HH:mm:ss"),
                Content = "",
                OwnerId = _user.Id,
                FolderId = _selectedFolderId
            });

            await ReloadAsync();
            await OpenEditorAsync(note);

            EditorTitleTextBox.SelectAll();
            EditorTitleTextBox.Focus();
        }
        catch (Exception ex)
        {
            ShowMessage("Не удалось создать заметку: " + ex.Message);
        }
    }

    private async void OnAddFolderClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            await _folders.CreateAsync(new CreateFolderRequest
            {
                Title = "Новая папка " + DateTime.Now.ToString("HH:mm:ss"),
                OwnerId = _user.Id,
                ParentFolderId = _selectedFolderId
            });

            await ReloadAsync();
        }
        catch (Exception ex)
        {
            ShowMessage("Не удалось создать папку: " + ex.Message);
        }
    }

    // AI-анализ

    private async Task PrepareAiPanelAsync()
    {
        SetAiStatus("Запуск LocalAiApi...", "#C9A227");

        AiHostState state;

        try
        {
            state = await _aiHost.EnsureStartedAsync();
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception ex)
        {
            SetAiStatus("Ошибка запуска LocalAiApi: " + ex.Message, "#B84638");
            return;
        }

        if (!await _ai.IsAvailableAsync())
        {
            SetAiStatus(GetAiHostProblem(state), "#B84638");
            return;
        }

        SetAiStatus(
            state == AiHostState.AlreadyRunning
                ? "LocalAiApi уже запущен · готово"
                : "LocalAiApi запущен автоматически · готово",
            "#427448");
    }

    private static string GetAiHostProblem(AiHostState state)
    {
        return state switch
        {
            AiHostState.BinaryNotFound =>
                "Не найден LocalAiApi.dll. Собери проект.",

            AiHostState.ProcessExited =>
                "LocalAiApi завершил работу. Проверь Ollama.",

            AiHostState.NotResponding =>
                "LocalAiApi не отвечает. Попробуй запустить вручную.",

            _ => "LocalAiApi недоступен."
        };
    }

    private async void OnGenerateClick(object? sender, RoutedEventArgs e)
    {
        if (_aiRunning)
        {
            _aiCancellation?.Cancel();
            return;
        }

        if (_editingNote == null)
        {
            ResponseTextBox.Text = "Сначала открой заметку.";
            return;
        }

        _aiRunning = true;
        _aiCancellation = new CancellationTokenSource();

        GenerateButton.Content = "Остановить генерацию";
        ResponseTextBox.Text = "Модель анализирует заметку...";
        SetAiStatus("Запрос отправлен...", "#C9A227");

        try
        {
            var result = await _ai.AnalyseNoteAsync(
                _editingNote,
                GetFolderTitle(_editingNote),
                _aiCancellation.Token);

            ResponseTextBox.Text = result;
            SetAiStatus("Анализ готов · " + DateTime.Now.ToString("HH:mm"), "#427448");
        }
        catch (OperationCanceledException)
        {
            ResponseTextBox.Text = "Генерация отменена.";
            SetAiStatus("Отменено", "#8D8C9C");
        }
        catch (Exception ex)
        {
            ResponseTextBox.Text = ex.Message;
            SetAiStatus("Ошибка анализа", "#B84638");
        }
        finally
        {
            _aiRunning = false;

            _aiCancellation?.Dispose();
            _aiCancellation = null;

            GenerateButton.Content = "✦  Сгенерировать анализ";
        }
    }

    private void SetAiStatus(string message, string color)
    {
        var brush = CreateBrush(color);

        AiStatusText.Text = message;
        AiStatusText.Foreground = brush;
        AiStatusDot.Foreground = brush;
    }

    // Боковые панели

    private void SetLeftCollapsed(bool collapsed)
    {
        _leftCollapsed = collapsed;

        LeftContent.IsVisible = !collapsed;
        LeftRail.IsVisible = collapsed;

        UpdateColumnWidths();
    }

    private void SetRightCollapsed(bool collapsed)
    {
        _rightCollapsed = collapsed;

        RightContent.IsVisible = !collapsed;
        RightRail.IsVisible = collapsed;

        UpdateColumnWidths();
    }

    private void UpdateColumnWidths()
    {
        var left = _leftCollapsed ? RailWidth : LeftWidth;
        var right = _rightCollapsed ? RailWidth : RightWidth;

        MainGrid.ColumnDefinitions = new ColumnDefinitions($"{left},*,{right}");
    }

    private void OnExitClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    // Вспомогательные методы

    private string? GetFolderTitle(Note note)
    {
        return note.FolderId is { } id
            ? _foldersList.FirstOrDefault(f => f.Id == id)?.Title
            : null;
    }

    private static SolidColorBrush CreateBrush(string color)
    {
        return new SolidColorBrush(Color.Parse(color));
    }

    private static string FormatNoteCount(int count)
    {
        return count switch
        {
            1 => "1 заметка",
            _ => count + " " + GetPlural(count)
        };
    }

    private static string GetPlural(int count)
    {
        var lastDigit = count % 10;
        var lastTwoDigits = count % 100;

        if (lastDigit == 1 && lastTwoDigits != 11)
            return "заметка";

        if (lastDigit >= 2 && lastDigit <= 4 &&
            (lastTwoDigits < 10 || lastTwoDigits >= 20))
            return "заметки";

        return "заметок";
    }
    
    private void ShowMessage(string message)
    {
        MessageText.Text = message;
        MessageOverlay.IsVisible = true;
    }

    private void OnMessageCloseClick(object? sender, RoutedEventArgs e)
    {
        MessageOverlay.IsVisible = false;
    }
}
