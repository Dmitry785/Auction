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
        public async Task<Result<string>> RequestUserId(string username, string password)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jsonBody = JsonSerializer.Serialize(new { Username = username, Password = password });
                    var response = await SendHttpRequest(jsonBody);

                    if (!Guid.TryParse(response, out Guid playerId))
                        return Result<string>.Fail();
                    return Result.Ok(playerId.ToString());
                }
            }
            catch { }
            return Result<string>.Fail();
        }
        private async Task<string?> SendHttpRequest(string jsonBody)
        {
            return Guid.NewGuid().ToString();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(_address, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        if (!Guid.TryParse(result, out Guid playerId))
                            return null;
                        return playerId.ToString();
                    }
                }
            }
            catch{}
            return null;
        }
    }
}
