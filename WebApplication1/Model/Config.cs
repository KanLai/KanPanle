using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model;

public class Config
{
    public int id { get; set; }
    [Required] [MaxLength(50)] public string key { get; set; } = null!;
    [Required] [MaxLength(200)] public string val { get; set; } = null!;
    [Required] [MaxLength(200)] public string placeholder { get; set; } = null!;
}