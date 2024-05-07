using System.Data;
using APBD_kol1.Models;
using APBD_kol1.Models.DTOs;
using APBD_kol1.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace APBD_kol1.Controllers;

// Nie [controller] - w zadaniu path URI jest z małych liter.
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBooksRepository _booksRepository;

    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }

    [HttpPost]
    public async Task<IActionResult> NewBook(NewBookDto newBook)
    {
        try
        {
            int bookId = await _booksRepository.NewBook(new NewBookDto());
            foreach (var genreId in newBook.genres)
            {
                if (!(await _booksRepository.GenreExists(genreId))) throw new ArgumentException("Genre exist");
                await _booksRepository.InsertBookGenre(bookId, genreId);
            }
        }
        catch (Exception e)
        {
            if (e is ArgumentException or DuplicateNameException)
                return BadRequest(e.Message);
            return BadRequest(e.Message);
        }

        return Created();
    }

    [Route("{bookId:int}/genres")]
    [HttpGet]
    public async Task<IActionResult> GetBookWithGenres(int bookId)
    {
        var book = await _booksRepository.GetBook(bookId);
        if (book == null) return NotFound();
        var bookGenres = await _booksRepository.GetBookGenres(bookId);
        var res = new BookWithGenresDto()
        {
            id = book.PK,
            title = book.title,
            genres = bookGenres.Select(b => b.name).ToList()
        };
        return Ok(res);
    }
}