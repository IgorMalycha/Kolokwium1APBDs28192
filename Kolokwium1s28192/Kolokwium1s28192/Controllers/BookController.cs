using System.Transactions;
using Kolokwium1s28192.DTOs;
using Kolokwium1s28192.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium1s28192.Controllers;

[Route("api/books")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepository;

    public BookController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }


    [HttpGet("{id}/genres")]
    public async Task<IActionResult> GetBookGenres(int id)
    {
        if (!await _bookRepository.DoesBookExist(id))
        {
            return NotFound("Book with given id does not exist");
        }

        var bookGenres = await _bookRepository.GetBookGenres(id);
        
        return Ok(bookGenres);
    }

    [HttpPost]
    public async Task<IActionResult> AddBookWithGenres(AddBookGenresDTO addBookGenresDto)
    {
        foreach (var genre in addBookGenresDto.Genres)
        {
            if (!await _bookRepository.DoesGenreExist(genre))
            {
                return NotFound("Given Genres does not exist");
            }
        }

        var id = -1;
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            id = await _bookRepository.PutBookGetId(addBookGenresDto);

            foreach (var genre in addBookGenresDto.Genres)
            {
                _bookRepository.PutGenreToBook(id, genre);    
            }
            
            scope.Complete();
        }

        var book = await _bookRepository.GetBookGenres(id);
        return Created("api/books", book);
    }


}