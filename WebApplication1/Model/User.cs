using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model;

public class User
{
    public long Id { get; set; }
    [Required(ErrorMessage = "昵称不能为空")]
    [MaxLength(50, ErrorMessage = "昵称最大长度为50")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "账号不能为空")]
    [MaxLength(50, ErrorMessage = "账号最大长度为50")]
    public string UserName { get; set; } = "";

    [Required(ErrorMessage = "密码不能为空")]
    [MaxLength(50, ErrorMessage = "密码最大长度为50")]
    public string Password { get; set; } = "";

    [MaxLength(255)] public string Avatar { get; set; } = "";
    public int Status { get; set; }
    [MaxLength(255)]
    public string Token { get; set; } = "";
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public DateTime LastLoginTime { get; set; }
    [MaxLength(50)]
    public string? LastLoginIp { get; set; }

    public object toJson()
    {
        return new
        {
            Id,
            Name,
            Avatar,
            Token,
            LastLoginTime,
            LastLoginIp
        };
    }


}