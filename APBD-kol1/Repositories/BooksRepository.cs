using System.Data;
using System.Diagnostics;
using APBD_kol1.Models;
using APBD_kol1.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_kol1.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly IConfiguration _configuration;

    public BooksRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Book?> GetBook(int bookId)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM books WHERE PK = @ID";
        command.Parameters.AddWithValue("@ID", bookId);
        await using var reader = await command.ExecuteReaderAsync();
        var pkOrdinal = reader.GetOrdinal("PK");
        var titleOrdinal = reader.GetOrdinal("title");
        bool exists = await reader.ReadAsync();
        if (!exists) return null;
        return new Book()
        {
            PK = reader.GetInt32(pkOrdinal),
            title = reader.GetString(titleOrdinal)
        };
    }

    public async Task<List<Genre>> GetBookGenres(int bookId)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText =
            """
            SELECT * FROM genres
                            INNER JOIN books_genres ON genres.PK = books_genres.FK_genre
                            WHERE books_genres.FK_book = @ID;
            """;
        command.Parameters.AddWithValue("@ID", bookId);
        await using var reader = await command.ExecuteReaderAsync();
        var pkOrdinal = reader.GetOrdinal("PK");
        var nameOrdinal = reader.GetOrdinal("name");
        var genres = new List<Genre>();
        while (await reader.ReadAsync())
        {
            genres.Add(new Genre()
                {
                    PK = reader.GetInt32(pkOrdinal),
                    name = reader.GetString(nameOrdinal)
                }
            );
        }

        return genres;
    }

    public async Task<bool> GenreExists(int genreId)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM genres WHERE PK = @ID";
        command.Parameters.AddWithValue("@ID", genreId);
        var res = await command.ExecuteNonQueryAsync();
        if (res > 1) return true;
        return false;
    }

    public async Task<int> NewBook(NewBookDto newBook)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO books OUTPUT INSERTED.PK VALUES ( @BN )";
        command.Parameters.AddWithValue("@BN", newBook.Title);
        var res = await command.ExecuteScalarAsync();
        if (res is null) throw new DuplicateNameException("Książka już istnieje!");
        return (int)res;
    }

    public async Task InsertBookGenre(int bookId, int genreId)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO books_genres VALUES( @BN, @GN );";
        command.Parameters.AddWithValue("@BN", bookId);
        command.Parameters.AddWithValue("@GN", genreId);
        var res = await command.ExecuteNonQueryAsync();
        if (res < 0) throw new DuplicateNameException("Książka już istnieje!");
        return;
    }
}