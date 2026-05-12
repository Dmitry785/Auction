using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Program.Controllers;

public class ProxyController(DataServerApiService apiService):Controller
{
    [HttpGet("image/{fileName}")]
    public async Task<IActionResult> GetImage(string fileName)
    {
        var imageDataResult = await apiService.LoadImage(fileName);

        if (imageDataResult.Failed)
            return NotFound();

        var (contentType, imageBytes) = imageDataResult.Data!;
        return File(imageBytes, contentType);
    }
}
