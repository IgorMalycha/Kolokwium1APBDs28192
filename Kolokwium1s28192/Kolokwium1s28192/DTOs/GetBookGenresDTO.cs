using System.ComponentModel.DataAnnotations;

namespace Kolokwium1s28192.DTOs;

public class GetBookGenresDTO
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    [MaxLength(100)]
    public List<string> Genres { get; set; }
}