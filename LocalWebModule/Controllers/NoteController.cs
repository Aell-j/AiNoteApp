using LocalJsonModule.DTOs.Notes;
using LocalJsonModule.Services;
using Microsoft.AspNetCore.Mvc;

namespace LocalWebModule.Controllers;

[ApiController]
[Route("api/notes")]
public class NoteController : ControllerBase
{
    private readonly INoteService _noteService;

    public NoteController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var notes = await _noteService.GetAllAsync();
            return Ok(notes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var note = await _noteService.GetByIdAsync(id);

            if (note == null)
            {
                return NotFound("Заметка не найдена.");
            }

            return Ok(note);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{ownerId:guid}/owner")]
    public async Task<IActionResult> GetByOwnerId(Guid ownerId)
    {
        try
        {
            var notes = await _noteService.GetByOwnerIdAsync(ownerId);
            return Ok(notes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{folderId:guid}/folder")]
    public async Task<IActionResult> GetByFolderId(Guid folderId)
    {
        try
        {
            var notes = await _noteService.GetByFolderIdAsync(folderId);
            return Ok(notes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNoteRequest request)
    {
        try
        {
            var note = await _noteService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNoteRequest request)
    {
        try
        {
            bool updated = await _noteService.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound("Заметка не найдена.");
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            bool deleted = await _noteService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound("Заметка не найдена.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{ownerId:guid}/owner")]
    public async Task<IActionResult> DeleteByOwnerId(Guid ownerId)
    {
        try
        {
            bool deleted = await _noteService.DeleteByOwnerIdAsync(ownerId);

            if (!deleted)
            {
                return NotFound("У владельца нет заметок.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{folderId:guid}/folder")]
    public async Task<IActionResult> DeleteByFolderId(Guid folderId)
    {
        try
        {
            bool deleted = await _noteService.DeleteByFolderIdAsync(folderId);

            if (!deleted)
            {
                return NotFound("В папке нет заметок.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("{id:guid}/favorite")]
    public async Task<IActionResult> ToggleFavorite(Guid id)
    {
        try
        {
            bool status = await _noteService.EnableToFavoriteAsync(id);
            if (!status)
            {
                return NotFound("Заметка не найдена");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
        return NoContent();
    }

    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> Archive(Guid id)
    {
        try
        {
            bool status = await _noteService.EnableToArchiveAsync(id);
            if (!status)
            {
                return NotFound("Архив пуст");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
        return NoContent();
    }
    
    [HttpPost("{id:guid}/rearchive")]
    public async Task<IActionResult> DeArchive(Guid id)
    {
        try
        {
            bool status = await _noteService.DisableToArchiveAsync(id);
            if (!status)
            {
                return NotFound("Архив пуст");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
        return NoContent();
    }
}
