using KR.CO.TEXT.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace KR.CO.TEXT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet]
        // public IEnumerable<Student> GetIpAddress()
        public ActionResult<IEnumerable<Student>> GetStudents()
        {
            return CollegeRepository.Students;
        }

        [HttpGet("{id:int}")]
        public Student GetStudent(int id)
        {
            
            return CollegeRepository.Students.FirstOrDefault(x => x.Id == id) ?? null!;
        }
        // {
        //     try
        //     {
        //         var ipAddress = HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR") ?? HttpContext.Connection.RemoteIpAddress?.ToString();
        //         var ipAddressWithoutPort = ipAddress?.Split(':')[0];

        //         var ipApiResponse = await _ipApiClient.Get(ipAddressWithoutPort, token);

        //         var response = new
        //         {
        //             IpAddress = ipAddressWithoutPort,
        //             ipApiResponse?.Country,
        //             Region = ipApiResponse?.RegionName,
        //             ipApiResponse?.City,
        //             ipApiResponse?.District,
        //             PostCode = ipApiResponse?.Zip,
        //             Longitude = ipApiResponse?.Lon.GetValueOrDefault(),
        //             Latitude = ipApiResponse?.Lat.GetValueOrDefault(),
        //         };

        //         return Ok(response);
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //     }
        // }
    }
}

//! new controller $ dotnet new apicontroller -o Controllers -n Student --namespace KR.CO.TEXT.API.Models 
//! run https      $ dotnet run --launch-profile https
//! watch          $ dotnet watch --launch-profile https

//? GET : 할 일 항목 모두 가져오기
//? GET : ID 로 항목 가져오기
//? POST : 새 항목 추가
//? PUT : 기존 항목 업데이트
//? DELETE : 항목 삭제

//! $ dotnet new class -o Data -n TodoContext
