using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DataServerApiService
    {
        private readonly string _address;
        public DataServerApiService(string address)
        {
            _address = address;
        }
        /*public async Task<Result<List<>>> LoadUserItems(string userId)
        {
            var response = await Send("loadUserItems", new { Id=userId });
            var items =
        }*/
        public async Task<Result<string>> RequestUserId(string username, string password)
        {
            var response = await Send("getUserId", new { Username = username, Password = password });
            if (!Guid.TryParse(response, out Guid playerId))
                return Result<string>.Fail();
            return Result.Ok(playerId.ToString());
        }
        private async Task<string?> Send(string url, object bodyObj)
        {
            return await SendHttpRequest(url, JsonSerializer.Serialize(bodyObj));
        }
        private async Task<string?> SendHttpRequest(string url, string jsonBody)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    var address = _address + (url.StartsWith("/") ? url : "/" + url);
                    HttpResponseMessage response = await client.PostAsync(address, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return result;
                    }
                }
            }
            catch{}
            return null;
        }
    }
}
