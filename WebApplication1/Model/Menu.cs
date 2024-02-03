using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication1.Model;

public class Menu
{
    public int? id { get; set; }
    [Required(ErrorMessage = "title is required")]
    [MaxLength(50)]
    public string title { get; set; } = "";
    public string url { get; set; }= "";
    public string icon { get; set; }= "";
    public string color { get; set; }= "#FFFFFF";
    public int ord { get; set; }
    public int belong { get; set; }
    public int status { get; set; }
    public int isTarget { get; set; }
    public int isDing { get; set; }
    public string bg { get; set; }= "#00000000";
    public string info { get; set; }= "";
    [JsonIgnore]
    public Case? @case { get; set; }


}