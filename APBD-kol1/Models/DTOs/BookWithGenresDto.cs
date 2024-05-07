using System.ComponentModel.DataAnnotations;

namespace APBD_kol1.Models.DTOs;

public class BookWithGenresDto
{
    public int id { get; set; }
    public string title { get; set; }
    public List<string> genres { get; set; }
}