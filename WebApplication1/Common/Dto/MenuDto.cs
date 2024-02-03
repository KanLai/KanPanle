namespace WebApplication1.Dto;

public class MenuDto
{
    public int? id { get; set; }
    public string? title { get; set; }
    public string? url { get; set; }
    public string? icon { get; set; }
    public string? color { get; set; }
    public int ord { get; set; }
    public int belong { get; set; }
    public int status { get; set; }
    public int isTarget { get; set; }
    public int isDing { get; set; }
    public string? bg { get; set; }
    public string? info { get; set; }
    public string? image { get; set; }


    public static MenuDto From(Model.Menu menu, CachedConfigService? cachedConfigService)
    {
        return new MenuDto()
        {
            id = menu.id,
            title = menu.title,
            url = menu.url,
            icon = menu.icon,
            color = menu.color,
            ord = menu.ord,
            belong = menu.belong,
            status = menu.status,
            isTarget = menu.isTarget,
            isDing = menu.isDing,
            bg = menu.bg,
            info = menu.info,
            image = cachedConfigService?.Get<string>("baseUrl") + menu.icon
        };
    }

    public static List<MenuDto> From(IEnumerable<Model.Menu> menus, CachedConfigService? cachedConfigService)
    {
        return menus.Select(menu => From(menu, cachedConfigService)).ToList();
    }
}