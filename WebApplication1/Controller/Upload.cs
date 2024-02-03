using Microsoft.AspNetCore.Mvc;
using WebApplication1.Attrs;

namespace WebApplication1.Controller;

public class fileTos
{
    public string? url { get; set; }
    public string? file { get; set; }
}

[AuthorizationMessage("请先登录")]
[ServiceFilter(typeof(AsyncAuthorizationFilter))]
[ApiController]
[Route("[controller]")]
public class Upload : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> upload(IFormFile? file, IWebHostEnvironment env,
        CachedConfigService cachedConfigService)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new Json<string?>()
            {
                code = 0,
                message = "文件为空",
            });
        }

        // 获取文件扩展名
        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        var sourceArray = cachedConfigService.Get<string>("Extension").Split('|');
        // 判断文件扩展名是否合法
        if (string.IsNullOrEmpty(fileExtension) ||
            !sourceArray.Contains(fileExtension.ToLower()))
        {
            return BadRequest(new Json<string?>()
            {
                code = 0,
                message = "文件格式不支持",
            });
        }

        // 使用GUID或者时间戳来生成新的文件名
        var newFileName = $"{Guid.NewGuid()}{fileExtension}";
        const string uploadPathName = "upload";
        var uploadPath = Path.Combine(env.WebRootPath, uploadPathName);
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var path = Path.Combine(uploadPath, newFileName);

        await using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var baseUrl = cachedConfigService.Get<string>("baseUrl");

        return Ok(new Json<fileTos>()
        {
            code = 1,
            message = "上传成功",
            data = new fileTos
            {
                url = $"{baseUrl}/{uploadPathName}/{newFileName}",
                file = $"/{uploadPathName}/{newFileName}"
            }
        });
    }
}