using System.Data.SqlClient;
using Kolokwium1s28192.DTOs;

namespace Kolokwium1s28192.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IConfiguration _configuration;
    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> DoesBookExist(int id)
    {
        var query = "SELECT 1 FROM books WHERE PK = @PK";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@PK", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<GetBookGenresDTO> GetBookGenres(int id)
    {
        var query = @"SELECT books.PK, books.title, genres.name FROM books
                        JOIN books_genres ON books_genres.FK_book = books.PK
                        JOIN genres ON genres.PK = books_genres.FK_genre
                        WHERE books.PK = @PK;";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@PK", id);
        
        
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();


        GetBookGenresDTO getBookGenresDto = null;

        var booksPK = reader.GetOrdinal("PK");
        var booksTitle = reader.GetOrdinal("title");
        var booksGenres = reader.GetOrdinal("name");

        while (await reader.ReadAsync())
        {
            if (getBookGenresDto is not null)
            {
                getBookGenresDto.Genres.Add(reader.GetString(booksGenres));
            }
            else
            {
                getBookGenresDto = new GetBookGenresDTO()
                {
                    Id = reader.GetInt32(booksPK),
                    Title = reader.GetString(booksTitle),
                    Genres = new List<string>()
                    {
                        reader.GetString(booksGenres)
                    }

                };
            }
        }
        if (getBookGenresDto is null) throw new Exception();
        
        return getBookGenresDto;
    }

    public async Task<bool> DoesGenreExist(int id)
    {
        var query = "SELECT 1 FROM genres WHERE PK = @PK";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@PK", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<int> PutBookGetId(AddBookGenresDTO addBookGenresDto)
    {
        /*AddBookGenresDTO addBookGenresDtoTmp = new AddBookGenresDTO()
        {
            Title = addBookGenresDto.Title,
            Genres = new List<int>()
        };

        foreach (var genre in addBookGenresDto.Genres)
        {
            addBookGenresDtoTmp.Genres.Add(genre);
        }*/

        var insert = @"INSERT INTO books(title)
                        VALUES (@title);
                        SELECT @@IDENTITY";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
	    
        command.Connection = connection;
        command.CommandText = insert;
	    
        command.Parameters.AddWithValue("@title", addBookGenresDto.Title);

        await connection.OpenAsync();
	    
        var id = await command.ExecuteScalarAsync();

        return Convert.ToInt32(id);
    }

    public async Task PutGenreToBook(int PK, int genre)
    {
        var insert = @"INSERT INTO books_genres(Fk_book, FK_genre)
                        VALUES (@PKBook, @PKGenre);";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
	    
        command.Connection = connection;
        command.CommandText = insert;
	    
        command.Parameters.AddWithValue("@PKBook", PK);
        command.Parameters.AddWithValue("@PKGenre", genre);

        await connection.OpenAsync();
	    
        await command.ExecuteScalarAsync();
    }
}