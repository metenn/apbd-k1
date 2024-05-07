using APBD_kol1.Models;
using APBD_kol1.Models.DTOs;

namespace APBD_kol1.Repositories;

public interface IBooksRepository
{
    public Task<Book?> GetBook(int bookId);
    public Task<List<Genre>> GetBookGenres(int bookId);
    public Task<bool> GenreExists(int genreId);
    public Task<int> NewBook(NewBookDto newBook);
    public Task InsertBookGenre(int bookId, int genreId);
}