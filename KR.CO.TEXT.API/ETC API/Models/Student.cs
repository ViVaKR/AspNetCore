using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KR.CO.TEXT.API.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class Student : ControllerBase
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
