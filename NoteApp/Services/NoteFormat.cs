using System;
using System.Globalization;

namespace NoteApp;

internal static class NoteFormat
{
    private static readonly CultureInfo RuCulture = new("ru-RU");

    public static string Shorten(string text, int max)
    {
        var flat = text.Trim();
        return flat.Length <= max
            ? flat
            : flat[..max].TrimEnd() + "…";
    }

    public static string FormatDate(DateTime utc)
    {
        var local = utc.ToLocalTime();
        var today = DateTime.Today;

        if (local.Date == today)
        {
            return "Сегодня, " + local.ToString("HH:mm");
        }

        if (local.Date == today.AddDays(-1))
        {
            return "Вчера, " + local.ToString("HH:mm");
        }

        return local.ToString("d MMMM", RuCulture);
    }
}
