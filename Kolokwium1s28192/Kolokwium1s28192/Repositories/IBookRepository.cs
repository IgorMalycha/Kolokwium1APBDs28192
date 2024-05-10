using Kolokwium1s28192.DTOs;

namespace Kolokwium1s28192.Repositories;

public interface IBookRepository
{
    Task<bool> DoesBookExist(int id);
    Task<GetBookGenresDTO> GetBookGenres(int id);
    Task<bool> DoesGenreExist(int id);
    Task<int> PutBookGetId(AddBookGenresDTO addBookGenresDto);
    Task PutGenreToBook(int id, int genre);
}