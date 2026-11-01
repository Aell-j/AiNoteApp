using LocalJsonModule.Models;

namespace LocalJsonModule.Services;

public interface IAiAnalysisService
{
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);

    Task<string> AnalyseNoteAsync(
        Note note,
        string? folderTitle,
        CancellationToken cancellationToken = default);
}
