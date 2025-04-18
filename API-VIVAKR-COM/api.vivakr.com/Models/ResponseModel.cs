using System.Text.Json.Serialization;

namespace ViVaKR.API.Models;

public class ResponseModel(ResponseCode responseCode, string message, object data)
{
    [JsonPropertyName("responseCode")]
    public ResponseCode ResponseCode { get; set; } = responseCode;

    [JsonPropertyName("responseMessage")]
    public string ResponseMessage { get; set; } = message;

    [JsonPropertyName("responseData")]
    public object ResponseData { get; set; } = data;
}


public enum ResponseCode
{
    NotSet = 0,
    OK = 1,
    Error = 2
}
