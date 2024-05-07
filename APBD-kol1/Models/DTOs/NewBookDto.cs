using System.ComponentModel.DataAnnotations;

namespace APBD_kol1.Models.DTOs;

public class NewBookDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    public List<int> genres { get; set; }
}