using Application.Common;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services;

public class DataServerApiService
{
    private readonly string _address;
    private readonly HttpClient _client = new HttpClient();
    public DataServerApiService(string address)
    {
        _address = address;
    }
    public async Task<Result<(string, byte[])>> LoadImage(string imageName)
    {
        var response = await _client.GetAsync($"{_address}/images/{imageName}");

        if (!response.IsSuccessStatusCode) 
            return Result<(string, byte[])>.Fail();

        var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
        var imageBytes = await response.Content.ReadAsByteArrayAsync();
        return Result.Ok((contentType, imageBytes));
    }
    public async Task<Result> MoveItem(string itemId, string ownerId)
    {
        var response = await Send("move", new { ItemId =itemId, NewOwnerId  = ownerId});
        if (response is null)
            return Result<List<DataServerItemData>>.Fail();
        return Result.Ok();
    }
    public async Task<Result> HoldItem(string itemId)
    {
        var response = await Send("hold", itemId);
        if (response is null)
            return Result<List<DataServerItemData>>.Fail();
        return Result.Ok();
    }
    public async Task<Result> UnholdItem(string itemId)
    {
        var response = await Send("unhold", itemId);
        if (response is null)
            return Result<List<DataServerItemData>>.Fail();
        return Result.Ok();
    }
    public async Task<Result<List<DataServerItemData>>> LoadUserItems(string userOriginalId)
    {
        var response = await Send("useritems", userOriginalId);
        if(string.IsNullOrEmpty(response))
            return Result<List<DataServerItemData>>.Fail();
        var itemsList = JsonSerializer.Deserialize<List<DataServerItemData>>(response, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        if (itemsList is null)
            return Result<List<DataServerItemData>>.Fail();
        return Result.Ok(itemsList);
    }
    public async Task<Result<string>> RequestUserId(string username, string password)
    {
        var response = await Send("userid", new { Username = username, Password = password });
        if (!Guid.TryParse(response?.Trim('\"'), out Guid playerId))
            return Result<string>.Fail();
        return Result.Ok(playerId.ToString());
    }
    private async Task<string?> Send(string url, object bodyObj)
    {
        var jsonBody = JsonSerializer.Serialize(bodyObj);
        try
        {
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var address = _address + (url.StartsWith("/") ? url : "/" + url);
            HttpResponseMessage response = await _client.PostAsync(address, content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
        catch{}
        return null;
    }
}
public sealed record DataServerUserData(string Id, string Name, string Username);
public sealed record DataServerItemData(string Id, string Name, string Description, ItemType Type, string OwnerId, string? Poster);
