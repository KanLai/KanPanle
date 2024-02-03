using WebApplication1.Model;

namespace WebApplication1.Dto;

public class UserDto
{

    public string? image { get; set; }
    public long id { get; set; }
    public string? name { get; set; }
    public string? token { get; set; }
    public string? LastLoginTime { get; set; }
    public string? LastLoginIp { get; set; }
    public string? avatar { get; set; }




    public static UserDto From(User user, CachedConfigService? cachedConfigService)
    {
        return new UserDto()
        {
            id = user.Id,
            name = user.Name,
            token = user.Token,
            avatar = user.Avatar,
            LastLoginTime = user.LastLoginTime.ToString("yyyy-M-d HH:mm:ss"),
            LastLoginIp = user.LastLoginIp,
            image = cachedConfigService?.Get<string>("baseUrl") + user.Avatar

        };
    }

    public static List<UserDto> From(IEnumerable<User> users, CachedConfigService? cachedConfigService)
    {
        return users.Select(user => From(user, cachedConfigService)).ToList();
    }
}