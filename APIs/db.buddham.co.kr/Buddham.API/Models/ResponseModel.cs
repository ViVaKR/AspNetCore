namespace Buddham.API.Models;

public class ResponseModel(ResponseCode code, string message, object data)
{
    public ResponseCode ResponseCode { get; set; } = code;
    public string ResponseMessage { get; set; } = message;
    public object? DataSet { get; set; } = data;
}
