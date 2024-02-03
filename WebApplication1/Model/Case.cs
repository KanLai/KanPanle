using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication1.Model;

public class Case
{
    public int? id { get; set; }
    [MaxLength(50)]
    public string? title { get; set; }
    public int isDing { get; set; }
    [JsonIgnore]
    public List<Menu>? menus { get; set; }
}