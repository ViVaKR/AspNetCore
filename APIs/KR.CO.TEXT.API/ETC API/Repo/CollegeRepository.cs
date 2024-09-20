using KR.CO.TEXT.API.Models;

namespace KR.CO.TEXT.API;

public static class CollegeRepository
{
    public static List<Student> Students { get; set; } = [

        new Student { Id = 1, Name = "Viv"},
        new Student { Id = 2, Name = "KR"},
    ];
}
