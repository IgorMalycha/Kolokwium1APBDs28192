using System.ComponentModel.DataAnnotations;

namespace Kolokwium1s28192.DTOs;

public class AddBookGenresDTO
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    [Required]
    public List<int> Genres { get; set; }
    
}