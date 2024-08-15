using System;

namespace ViVaBM.API.Models;

public class ResponseModel(
    ResponseCode responseCode,
    string message,
    object data)
{
    public ResponseCode ResponseCode { get; set; } = responseCode;
    public string ResponseMessage { get; set; } = message;
    public object ResponseData { get; set; } = data;
}


public enum ResponseCode
{
    NOtSet = 0,
    OK = 1,
    Error = 2,
    Success = 200,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    InternalServerError = 500
}
