using Bible.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bible.API.Data;

public class BibleContext(DbContextOptions<BibleContext> options)
    : IdentityDbContext<AppUser>(options)
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<BibleModel> Bibles => Set<BibleModel>();

    public DbSet<TodayMessage> TodayMessages => Set<TodayMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(

            new Category
            {
                Id = 1,
                Testament = Testament.OLD,
                EngName = "Genesis",
                KorName = "창세기",
                EngAbbreviation = "Gen",
                KorAbbreviation = "창",
                ChapterCount = 50,
                VerseCount = 1533,
                Order = 1,
                Description = "창세기는 모세의 저서 중 첫 번째 책이다. 창세기는 성경의 첫 번째 책이며, 성경의 첫 11장은 창조와 인류의 탄생, 그리고 인류의 죄와 죽음에 대한 이야기를 담고 있다. 이후 12장부터는 아브라함과 그의 후손들의 이야기를 담고 있다."
            },

            new Category
            {
                Id = 2,
                Testament = Testament.OLD,
                EngName = "Exodus",
                KorName = "출애굽기",
                EngAbbreviation = "Exo",
                KorAbbreviation = "출",
                ChapterCount = 40,
                VerseCount = 1213,
                Order = 2,
                Description = "출애굽기는 모세의 저서 중 두 번째 책이다. 출애굽기는 이스라엘 백성의 애굽에서의 탈출과 시내산에서의 성벽, 그리고 여호와와의 언약을 다루고 있다."
            },

            new Category
            {
                Id = 3,
                Testament = Testament.OLD,
                EngName = "Leviticus",
                KorName = "레위기",
                EngAbbreviation = "Lev",
                KorAbbreviation = "레",
                ChapterCount = 27,
                VerseCount = 859,
                Order = 3,
                Description = "레위기는 모세의 저서 중 세 번째 책이다. 레위기는 제사장들의 역할과 제사의 종류, 그리고 이스라엘 백성의 거룩함에 대한 법률을 다루고 있다."
            },

            new Category
            {
                Id = 4,
                Testament = Testament.OLD,
                EngName = "Numbers",
                KorName = "민수기",
                EngAbbreviation = "Num",
                KorAbbreviation = "민",
                ChapterCount = 36,
                VerseCount = 1288,
                Order = 4,
                Description = "민수기는 모세의 저서 중 네 번째 책이다. 민수기는 이스라엘 백성의 인구 조사와 여행 기록, 그리고 여호와와의 언약을 다루고 있다."
            },

            new Category
            {
                Id = 5,
                Testament = Testament.OLD,
                EngName = "Deuteronomy",
                KorName = "신명기",
                EngAbbreviation = "Deu",
                KorAbbreviation = "신",
                ChapterCount = 34,
                VerseCount = 959,
                Order = 5,
                Description = "신명기는 모세의 저서 중 다섯 번째 책이다. 신명기는 모세가 이스라엘 백성에게 전한 가르침과 축복, 그리고 여호와와의 언약을 다루고 있다."
            },

            new Category
            {
                Id = 6,
                Testament = Testament.OLD,
                EngName = "Joshua",
                KorName = "여호수아",
                EngAbbreviation = "Jos",
                KorAbbreviation = "수",
                ChapterCount = 24,
                VerseCount = 658,
                Order = 6,
                Description = "여호수아는 구약 성경의 역사책 중 하나이다. 여호수아는 모세의 후계자로서 이스라엘 백성을 여호와의 약속대로 가나안 땅에 인도하는 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 7,
                Testament = Testament.OLD,
                EngName = "Judges",
                KorName = "사사기",
                EngAbbreviation = "Jug",
                KorAbbreviation = "삿",
                ChapterCount = 21,
                VerseCount = 618,
                Order = 7,
                Description = "사사기는 구약 성경의 역사책 중 하나이다. 사사기는 이스라엘 백성의 판사들의 이야기를 다루고 있다."
            },
            new Category
            {
                Id = 8,
                Testament = Testament.OLD,
                EngName = "Ruth",
                KorName = "룻기",
                EngAbbreviation = "Rut",
                KorAbbreviation = "룻",
                ChapterCount = 4,
                VerseCount = 85,
                Order = 8,
                Description = "룻기는 구약 성경의 역사책 중 하나이다. 룻기는 모압 여인 룻의 이스라엘 백성에 대한 충성스러운 사랑 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 9,
                Testament = Testament.OLD,
                EngName = "1 Samuel",
                KorName = "사무엘상",
                EngAbbreviation = "1Sa",
                KorAbbreviation = "삼상",
                ChapterCount = 31,
                VerseCount = 810,
                Order = 9,
                Description = "사무엘상은 구약 성경의 역사책 중 하나이다. 사무엘상은 사무엘과 다윗의 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 10,
                Testament = Testament.OLD,
                EngName = "2 Samuel",
                KorName = "사무엘하",
                EngAbbreviation = "2Sa",
                KorAbbreviation = "삼하",
                ChapterCount = 24,
                VerseCount = 695,
                Order = 10,
                Description = "사무엘하은 구약 성경의 역사책 중 하나이다. 사무엘하은 다윗의 왕위 계승과 그의 왕국의 번성, 그리고 그의 죽음까지의 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 11,
                Testament = Testament.OLD,
                EngName = "1 Kings",
                KorName = "열왕기상",
                EngAbbreviation = "1Ki",
                KorAbbreviation = "왕상",
                ChapterCount = 22,
                VerseCount = 816,
                Order = 11,
                Description = "열왕기상은 구약 성경의 역사책 중 하나이다. 열왕기상은 다윗의 아들 솔로몬과 그의 후손들의 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 12,
                Testament = Testament.OLD,
                EngName = "2 Kings",
                KorName = "열왕하",
                EngAbbreviation = "2Ki",
                KorAbbreviation = "왕하",
                ChapterCount = 25,
                VerseCount = 719,
                Order = 12,
                Description = "열왕하은 구약 성경의 역사책 중 하나이다. 열왕하은 이스라엘 백성의 분열과 그들의 멸망에 대한 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 13,
                Testament = Testament.OLD,
                EngName = "1 Chronicles",
                KorName = "역대상",
                EngAbbreviation = "1Ch",
                KorAbbreviation = "대상",
                ChapterCount = 29,
                VerseCount = 942,
                Order = 13,
                Description = "역대상은 구약 성경의 역사책 중 하나이다. 역대상은 다윗과 솔로몬의 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 14,
                Testament = Testament.OLD,
                EngName = "2 Chronicles",
                KorName = "역대하",
                EngAbbreviation = "2Ch",
                KorAbbreviation = "대하",
                ChapterCount = 36,
                VerseCount = 822,
                Order = 14,
                Description = "역대하은 구약 성경의 역사책 중 하나이다. 역대하은 유다와 이스라엘의 왕들의 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 15,
                Testament = Testament.OLD,
                EngName = "Ezra",
                KorName = "에스라",
                EngAbbreviation = "Ezr",
                KorAbbreviation = "스",
                ChapterCount = 10,
                VerseCount = 280,
                Order = 15,
                Description = "에스라는 구약 성경의 역사책 중 하나이다. 에스라는 바벨론에서 유다 백성이 예루살렘으로 귀환하는 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 16,
                Testament = Testament.OLD,
                EngName = "Nehemiah",
                KorName = "느헤미야",
                EngAbbreviation = "Neh",
                KorAbbreviation = "느",
                ChapterCount = 13,
                VerseCount = 406,
                Order = 16,
                Description = "느헤미야는 구약 성경의 역사책 중 하나이다. 느헤미야는 예루살렘 성벽 건설과 유다 백성의 회개에 대한 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 17,
                Testament = Testament.OLD,
                EngName = "Esther",
                KorName = "에스더",
                EngAbbreviation = "Est",
                KorAbbreviation = "에",
                ChapterCount = 10,
                VerseCount = 167,
                Order = 17,
                Description = "에스더는 구약 성경의 역사책 중 하나이다. 에스더는 바사 왕 아하수에로의 왕비 에스더가 유다 백성을 위해 행한 구원 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 18,
                Testament = Testament.OLD,
                EngName = "Job",
                KorName = "욥기",
                EngAbbreviation = "Job",
                KorAbbreviation = "욥",
                ChapterCount = 42,
                VerseCount = 1070,
                Order = 18,
                Description = "욥기는 구약 성경의 지혜 문학책 중 하나이다. 욥기는 욥의 시련과 그의 믿음에 대한 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 19,
                Testament = Testament.OLD,
                EngName = "Psalms",
                KorName = "시편",
                EngAbbreviation = "Psa",
                KorAbbreviation = "시",
                ChapterCount = 150,
                VerseCount = 2461,
                Order = 19,
                Description = "시편은 구약 성경의 지혜 문학책 중 하나이다. 시편은 다윗과 다른 시인들이 여호와를 찬양하고 기도하는 시를 담고 있다."
            },

            new Category
            {
                Id = 20,
                Testament = Testament.OLD,
                EngName = "Proverbs",
                KorName = "잠언",
                EngAbbreviation = "Pro",
                KorAbbreviation = "잠",
                ChapterCount = 31,
                VerseCount = 915,
                Order = 20,
                Description = "잠언은 구약 성경의 지혜 문학책 중 하나이다. 잠언은 솔로몬의 지혜와 다른 지혜자들의 말을 담고 있다."
            },

            new Category
            {
                Id = 21,
                Testament = Testament.OLD,
                EngName = "Ecclesiastes",
                KorName = "전도서",
                EngAbbreviation = "Ecc",
                KorAbbreviation = "전",
                ChapterCount = 12,
                VerseCount = 222,
                Order = 21,
                Description = "전도서는 구약 성경의 지혜 문학책 중 하나이다. 전도서는 전도자의 삶과 삶의 의미에 대한 고찰을 담고 있다."
            },

            new Category
            {
                Id = 22,
                Testament = Testament.OLD,
                EngName = "Song of Solomon",
                KorName = "아가",
                EngAbbreviation = "Son",
                KorAbbreviation = "아",
                ChapterCount = 8,
                VerseCount = 117,
                Order = 22,
                Description = "아가는 구약 성경의 지혜 문학책 중 하나이다. 아가는 신랑과 신부의 사랑 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 23,
                Testament = Testament.OLD,
                EngName = "Isaiah",
                KorName = "이사야",
                EngAbbreviation = "Isa",
                KorAbbreviation = "사",
                ChapterCount = 66,
                VerseCount = 1292,
                Order = 23,
                Description = "이사야는 구약 성경의 대선지서 중 하나이다. 이사야는 이스라엘 백성에게 하나님의 심판과 구원에 대한 예언을 전한 선지자의 책이다."
            },

            new Category
            {
                Id = 24,
                Testament = Testament.OLD,
                EngName = "Jeremiah",
                KorName = "예레미야",
                EngAbbreviation = "Jer",
                KorAbbreviation = "렘",
                ChapterCount = 52,
                VerseCount = 1364,
                Order = 24,
                Description = "예레미야는 구약 성경의 대선지서 중 하나이다. 예레미야는 유다 백성에게 하나님의 심판과 구원에 대한 예언을 전한 선지자의 책이다."
            },

            new Category
            {
                Id = 25,
                Testament = Testament.OLD,
                EngName = "Lamentations",
                KorName = "예레미야애가",
                EngAbbreviation = "Lam",
                KorAbbreviation = "애",
                ChapterCount = 5,
                VerseCount = 154,
                Order = 25,
                Description = "예레미야애가는 구약 성경의 대선지서 중 하나이다. 예레미야애가는 예루살렘 멸망에 대한 애가를 담고 있다."
            },

            new Category
            {
                Id = 26,
                Testament = Testament.OLD,
                EngName = "Ezekiel",
                KorName = "에스겔",
                EngAbbreviation = "Eze",
                KorAbbreviation = "겔",
                ChapterCount = 48,
                VerseCount = 1273,
                Order = 26,
                Description = "에스겔은 구약 성경의 대선지서 중 하나이다. 에스겔은 바벨론에서 사로잡혀 간 유다 백성에게 하나님의 심판과 구원에 대한 예언을 전한 선지자의 책이다."
            },

            new Category
            {
                Id = 27,
                Testament = Testament.OLD,
                EngName = "Daniel",
                KorName = "다니엘",
                EngAbbreviation = "Dan",
                KorAbbreviation = "단",
                ChapterCount = 12,
                VerseCount = 357,
                Order = 27,
                Description = "다니엘은 구약 성경의 대선지서 중 하나이다. 다니엘은 바벨론에서 사로잡혀 간 유다 백성의 선지자로서 바벨론 왕국의 미래에 대한 예언을 전한 선지자의 책이다."
            },

            new Category
            {
                Id = 28,
                Testament = Testament.OLD,
                EngName = "Hosea",
                KorName = "호세아",
                EngAbbreviation = "Hos",
                KorAbbreviation = "호",
                ChapterCount = 14,
                VerseCount = 197,
                Order = 28,
                Description = "호세아는 구약 성경의 소선지서 중 하나이다. 호세아는 이스라엘 백성에게 하나님의 사랑과 그들의 음란함에 대한 경고를 전한 선지자의 책이다."
            },

            new Category
            {
                Id = 29,
                Testament = Testament.OLD,
                EngName = "Joel",
                KorName = "요엘",
                EngAbbreviation = "Joe",
                KorAbbreviation = "욜",
                ChapterCount = 3,
                VerseCount = 73,
                Order = 29,
                Description = "요엘은 구약 성경의 소선지서 중 하나이다. 요엘은 이스라엘 백성에게 하나님의 심판과 구원에 대한 예언을 전한 선지자의 책이다."
            },

            new Category
            {
                Id = 30,
                Testament = Testament.OLD,
                EngName = "Amos",
                KorName = "아모스",
                EngAbbreviation = "Amo",
                KorAbbreviation = "아",
                ChapterCount = 9,
                VerseCount = 146,
                Order = 30,
                Description = "아모스는 구약 성경의 소선지서 중 하나이다. 아모스는 이스라엘 백성에게 하나님의 심판과 구원에 대한 예언을 전한 선지자의 책이다."
            },

            new Category
            {
                Id = 31,
                Testament = Testament.OLD,
                EngName = "Obadiah",
                KorName = "오바댜",
                EngAbbreviation = "Oba",
                KorAbbreviation = "옵",
                ChapterCount = 1,
                VerseCount = 21,
                Order = 31,
                Description = "오바댜는 구약 성경의 소선지서 중 하나이다. 오바댜는 에돔에 대한 심판과 이스라엘 백성에 대한 구원에 대한 예언을 전한 선지자의 책이다."
            },

            new Category
            {
                Id = 32,
                Testament = Testament.OLD,
                EngName = "Jonah",
                KorName = "요나",
                EngAbbreviation = "Jon",
                KorAbbreviation = "욘",
                ChapterCount = 4,
                VerseCount = 48,
                Order = 32,
                Description = "요나는 구약 성경의 소선지서 중 하나이다. 요나는 니느웨에 대한 하나님의 심판과 그의 은혜에 대한 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 33,
                Testament = Testament.OLD,
                EngName = "Micah",
                KorName = "미가",
                EngAbbreviation = "Mic",
                KorAbbreviation = "미",
                ChapterCount = 7,
                VerseCount = 105,
                Order = 33,
                Description = "미가는 구약 성경의 소선지서 중 하나이다. 미가는 이스라엘 백성에게 하나님의 심판과 구원에 대한 예언을 전한 선지자의 책이다."
            },

            new Category
            {
                Id = 34,
                Testament = Testament.OLD,
                EngName = "Nahum",
                KorName = "나훔",
                EngAbbreviation = "Nah",
                KorAbbreviation = "나",
                ChapterCount = 3,
                VerseCount = 47,
                Order = 34,
                Description = "나훔은 구약 성경의 소선지서 중 하나이다. 나훔은 니느웨에 대한 하나님의 심판에 대한 예언을 전한 선지자의 책이다."
            },

            new Category
            {
                Id = 35,
                Testament = Testament.OLD,
                EngName = "Habakkuk",
                KorName = "하박국",
                EngAbbreviation = "Hab",
                KorAbbreviation = "합",
                ChapterCount = 3,
                VerseCount = 56,
                Order = 35,
                Description = "하박국은 구약 성경의 소선지서 중 하나이다. 하박국은 유다 백성에게 하나님의 심판과 구원에 대한 예언을 전한 선지자의 책이다."
            },

            new Category
            {
                Id = 36,
                Testament = Testament.OLD,
                EngName = "Zephaniah",
                KorName = "스바냐",
                EngAbbreviation = "Zep",
                KorAbbreviation = "스",
                ChapterCount = 3,
                VerseCount = 53,
                Order = 36,
                Description = "스바냐는 구약 성경의 소선지서 중 하나이다. 스바냐는 유다 백성에게 하나님의 심판과 구원에 대한 예언을 전한 선지자의 책이다."
            },

            new Category
            {
                Id = 37,
                Testament = Testament.OLD,
                EngName = "Haggai",
                KorName = "학개",
                EngAbbreviation = "Hag",
                KorAbbreviation = "학",
                ChapterCount = 2,
                VerseCount = 38,
                Order = 37,
                Description = "학개는 구약 성경의 소선지서 중 하나이다. 학개는 예루살렘 성전 건축에 대한 하나님의 경고와 유다 백성의 회개에 대한 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 38,
                Testament = Testament.OLD,
                EngName = "Zechariah",
                KorName = "스가랴",
                EngAbbreviation = "Zec",
                KorAbbreviation = "슥",
                ChapterCount = 14,
                VerseCount = 211,
                Order = 38,
                Description = "스가랴는 구약 성경의 소선지서 중 하나이다. 스가랴는 예루살렘 성전 건축에 대한 하나님의 약속과 유다 백성의 회개에 대한 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 39,
                Testament = Testament.OLD,
                EngName = "Malachi",
                KorName = "말라기",
                EngAbbreviation = "Mal",
                KorAbbreviation = "말",
                ChapterCount = 4,
                VerseCount = 55,
                Order = 39,
                Description = "말라기는 구약 성경의 소선지서 중 하나이다. 말라기는 유다 백성에게 하나님의 사랑과 그들의 음란함에 대한 경고를 전한 선지자의 책이다."
            },

            //=  신약
            new Category
            {
                Id = 40,
                Testament = Testament.NEW,
                EngName = "Matthew",
                KorName = "마태복음",
                EngAbbreviation = "Mat",
                KorAbbreviation = "마",
                ChapterCount = 28,
                VerseCount = 1071,
                Order = 1,
                Description = "마태복음은 신약 성경의 복음서 중 하나이다. 마태복음은 예수 그리스도의 탄생과 그의 사역, 그리고 그의 죽음과 부활에 대한 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 41,
                Testament = Testament.NEW,
                EngName = "Mark",
                KorName = "마가복음",
                EngAbbreviation = "Mar",
                KorAbbreviation = "막",
                ChapterCount = 16,
                VerseCount = 678,
                Order = 2,
                Description = "마가복음은 신약 성경의 복음서 중 하나이다. 마가복음은 예수 그리스도의 사역과 그의 죽음과 부활에 대한 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 42,
                Testament = Testament.NEW,
                EngName = "Luke",
                KorName = "누가복음",
                EngAbbreviation = "Luk",
                KorAbbreviation = "눅",
                ChapterCount = 24,
                VerseCount = 1151,
                Order = 3,
                Description = "누가복음은 신약 성경의 복음서 중 하나이다. 누가복음은 예수 그리스도의 탄생과 그의 사역, 그리고 그의 죽음과 부활에 대한 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 43,
                Testament = Testament.NEW,
                EngName = "John",
                KorName = "요한복음",
                EngAbbreviation = "Joh",
                KorAbbreviation = "요",
                ChapterCount = 21,
                VerseCount = 879,
                Order = 4,
                Description = "요한복음은 신약 성경의 복음서 중 하나이다. 요한복음은 예수 그리스도의 신성과 그의 사역, 그리고 그의 죽음과 부활에 대한 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 44,
                Testament = Testament.NEW,
                EngName = "Acts",
                KorName = "사도행전",
                EngAbbreviation = "Act",
                KorAbbreviation = "행",
                ChapterCount = 28,
                VerseCount = 1007,
                Order = 5,
                Description = "사도행전은 신약 성경의 역사책 중 하나이다. 사도행전은 예수 그리스도의 사역과 그의 사도들의 사역, 그리고 그들의 죽음과 부활에 대한 이야기를 다루고 있다."
            },

            new Category
            {
                Id = 45,
                Testament = Testament.NEW,
                EngName = "Romans",
                KorName = "로마서",
                EngAbbreviation = "Rom",
                KorAbbreviation = "롬",
                ChapterCount = 16,
                VerseCount = 433,
                Order = 6,
                Description = "로마서는 신약 성경의 서신서 중 하나이다. 로마서는 로마인들에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 46,
                Testament = Testament.NEW,
                EngName = "1 Corinthians",
                KorName = "고린도전서",
                EngAbbreviation = "1Co",
                KorAbbreviation = "고전",
                ChapterCount = 16,
                VerseCount = 437,
                Order = 7,
                Description = "고린도전서는 신약 성경의 서신서 중 하나이다. 고린도전서는 고린도인들에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 47,
                Testament = Testament.NEW,
                EngName = "2 Corinthians",
                KorName = "고린도후서",
                EngAbbreviation = "2Co",
                KorAbbreviation = "고후",
                ChapterCount = 13,
                VerseCount = 257,
                Order = 8,
                Description = "고린도후서는 신약 성경의 서신서 중 하나이다. 고린도후서는 고린도인들에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 48,
                Testament = Testament.NEW,
                EngName = "Galatians",
                KorName = "갈라디아서",
                EngAbbreviation = "Gal",
                KorAbbreviation = "갈",
                ChapterCount = 6,
                VerseCount = 149,
                Order = 9,
                Description = "갈라디아서는 신약 성경의 서신서 중 하나이다. 갈라디아서는 갈라디아인들에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 49,
                Testament = Testament.NEW,
                EngName = "Ephesians",
                KorName = "에베소서",
                EngAbbreviation = "Eph",
                KorAbbreviation = "엡",
                ChapterCount = 6,
                VerseCount = 155,
                Order = 10,
                Description = "에베소서는 신약 성경의 서신서 중 하나이다. 에베소서는 에베소인들에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 50,
                Testament = Testament.NEW,
                EngName = "Philippians",
                KorName = "빌립보서",
                EngAbbreviation = "Phi",
                KorAbbreviation = "빌",
                ChapterCount = 4,
                VerseCount = 104,
                Order = 11,
                Description = "빌립보서는 신약 성경의 서신서 중 하나이다. 빌립보서는 빌립보인들에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 51,
                Testament = Testament.NEW,
                EngName = "Colossians",
                KorName = "골로새서",
                EngAbbreviation = "Col",
                KorAbbreviation = "골",
                ChapterCount = 4,
                VerseCount = 95,
                Order = 12,
                Description = "골로새서는 신약 성경의 서신서 중 하나이다. 골로새서는 골로새인들에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 52,
                Testament = Testament.NEW,
                EngName = "1 Thessalonians",
                KorName = "데살로니가전서",
                EngAbbreviation = "1Th",
                KorAbbreviation = "데전",
                ChapterCount = 5,
                VerseCount = 89,
                Order = 13,
                Description = "데살로니가전서는 신약 성경의 서신서 중 하나이다. 데살로니가전서는 데살로니가인들에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 53,
                Testament = Testament.NEW,
                EngName = "2 Thessalonians",
                KorName = "데살로니가후서",
                EngAbbreviation = "2Th",
                KorAbbreviation = "데후",
                ChapterCount = 3,
                VerseCount = 47,
                Order = 14,
                Description = "데살로니가후서는 신약 성경의 서신서 중 하나이다. 데살로니가후서는 데살로니가인들에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 54,
                Testament = Testament.NEW,
                EngName = "1 Timothy",
                KorName = "디모데전서",
                EngAbbreviation = "1Ti",
                KorAbbreviation = "딤전",
                ChapterCount = 6,
                VerseCount = 113,
                Order = 15,
                Description = "디모데전서는 신약 성경의 서신서 중 하나이다. 디모데전서는 디모데에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 55,
                Testament = Testament.NEW,
                EngName = "2 Timothy",
                KorName = "디모데후서",
                EngAbbreviation = "2Ti",
                KorAbbreviation = "딤후",
                ChapterCount = 4,
                VerseCount = 83,
                Order = 16,
                Description = "디모데후서는 신약 성경의 서신서 중 하나이다. 디모데후서는 디모데에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 56,
                Testament = Testament.NEW,
                EngName = "Titus",
                KorName = "디도서",
                EngAbbreviation = "Titus",
                KorAbbreviation = "딛",
                ChapterCount = 3,
                VerseCount = 46,
                Order = 17,
                Description = "디도서는 신약 성경의 서신서 중 하나이다. 디도서는 디도에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 57,
                Testament = Testament.NEW,
                EngName = "Philemon",
                KorName = "빌레몬서",
                EngAbbreviation = "Phm",
                KorAbbreviation = "빌몬",
                ChapterCount = 1,
                VerseCount = 25,
                Order = 18,
                Description = "빌레몬서는 신약 성경의 서신서 중 하나이다. 빌레몬서는 빌레몬에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 58,
                Testament = Testament.NEW,
                EngName = "Hebrews",
                KorName = "히브리서",
                EngAbbreviation = "Heb",
                KorAbbreviation = "히",
                ChapterCount = 13,
                VerseCount = 303,
                Order = 19,
                Description = "히브리서는 신약 성경의 서신서 중 하나이다. 히브리서는 히브리인들에게 복음을 전한 바울의 편지이다."
            },

            new Category
            {
                Id = 59,
                Testament = Testament.NEW,
                EngName = "James",
                KorName = "야고보서",
                EngAbbreviation = "Jam",
                KorAbbreviation = "약",
                ChapterCount = 5,
                VerseCount = 108,
                Order = 20,
                Description = "야고보서는 신약 성경의 서신서 중 하나이다. 야고보서는 야고보에게 복음을 전한 예수 그리스도의 형제의 편지이다."
            },

            new Category
            {
                Id = 60,
                Testament = Testament.NEW,
                EngName = "1 Peter",
                KorName = "베드로전서",
                EngAbbreviation = "1Pe",
                KorAbbreviation = "벧전",
                ChapterCount = 5,
                VerseCount = 105,
                Order = 21,
                Description = "베드로전서는 신약 성경의 서신서 중 하나이다. 베드로전서는 베드로에게 복음을 전한 예수 그리스도의 사도의 편지이다."
            },

            new Category
            {
                Id = 61,
                Testament = Testament.NEW,
                EngName = "2 Peter",
                KorName = "베드로후서",
                EngAbbreviation = "2Pe",
                KorAbbreviation = "벧후",
                ChapterCount = 3,
                VerseCount = 61,
                Order = 22,
                Description = "베드로후서는 신약 성경의 서신서 중 하나이다. 베드로후서는 베드로에게 복음을 전한 예수 그리스도의 사도의 편지이다."
            },

            new Category
            {
                Id = 62,
                Testament = Testament.NEW,
                EngName = "1 John",
                KorName = "요한1서",
                EngAbbreviation = "1Jo",
                KorAbbreviation = "요1",
                ChapterCount = 5,
                VerseCount = 105,
                Order = 23,
                Description = "요한1서는 신약 성경의 서신서 중 하나이다. 요한1서는 요한에게 복음을 전한 예수 그리스도의 사도의 편지이다."
            },

            new Category
            {
                Id = 63,
                Testament = Testament.NEW,
                EngName = "2 John",
                KorName = "요한2서",
                EngAbbreviation = "2Jo",
                KorAbbreviation = "요2",
                ChapterCount = 1,
                VerseCount = 13,
                Order = 24,
                Description = "요한2서는 신약 성경의 서신서 중 하나이다. 요한2서는 요한에게 복음을 전한 예수 그리스도의 사도의 편지이다."
            },

            new Category
            { Id = 64, Testament = Testament.NEW, EngName = "3 John", KorName = "요한3서", EngAbbreviation = "3Jo", KorAbbreviation = "요3", ChapterCount = 1, VerseCount = 15, Order = 25, Description = "요한3서는 신약 성경의 서신서 중 하나이다. 요한3서는 요한에게 복음을 전한 예수 그리스도의 사도의 편지이다." },

            new Category
            {
                Id = 65,
                Testament = Testament.NEW,
                EngName = "Jude",
                KorName = "유다서",
                EngAbbreviation = "Jud",
                KorAbbreviation = "유",
                ChapterCount = 1,
                VerseCount = 25,
                Order = 26,
                Description = "유다서는 신약 성경의 서신서 중 하나이다. 유다서는 유다에게 복음을 전한 예수 그리스도의 형제의 편지이다."
            },

            new Category
            {
                Id = 66,
                Testament = Testament.NEW,
                EngName = "Revelation",
                KorName = "요한계시록",
                EngAbbreviation = "Rev",
                KorAbbreviation = "계",
                ChapterCount = 22,
                VerseCount = 404,
                Order = 27,
                Description = "요한계시록은 신약 성경의 예언서 중 하나이다. 요한계시록은 예수 그리스도의 재림과 그의 왕국의 번성, 그리고 그의 새로운 천지에 대한 이야기를 다루고 있다."
            }
        );
    }
}
