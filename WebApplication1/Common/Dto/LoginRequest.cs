using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dto;

public sealed class LoginRequest
{
    [MaxLength(20, ErrorMessage = "账号最多不能超过20个字符")]
    [MinLength(5, ErrorMessage = "账号最少需要5个字符")]
    public required string Username { get; set; }
    [MaxLength(20, ErrorMessage = "密码最多不能超过20个字符")]
    [MinLength(5, ErrorMessage = "密码最少需要5个字符")]
    public required string Password { get; set; }
}