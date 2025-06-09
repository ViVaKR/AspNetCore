namespace Buddha.Helpers;

public static class Helper
{
        public static string[] AllowOrigins()
    {
        return
        [
            "http://localhost:3000",
            "https://localhost:3000",
            "https://localhost:58261",
            "https://localhost:55521",
            "https://localhost:55533",
            "https://localhost:55588",
            "http://localhost:55580",
            "https://buddham.co.kr", //--> (1) buddham.co.kr
            "https://api.buddham.co.kr", // * sutra db api
            "https://buddha.buddham.co.kr", // sutra
            "https://code.buddham.co.kr", // 서비스 중
            "https://db.buddham.co.kr", // * sutra db
            "https://iam.buddham.co.kr", // sutra
            "https://sutra.buddham.co.kr", //sutra
            "https://www.buddham.co.kr", // sutra

            "https://vivakr.com", //--> (2) vivakr.com // 서비스 중
            "https://api.vivakr.com", // 서비스 중
            "https://bj.vivakr.com", // 서비스 중
            "https://code.vivakr.com", // 서비스 중
            "https://ns.vivakr.com", // 서비스 중
            "https://www.vivakr.com", // 서비스 중

            "https://text.or.kr", //--> (3) text.or.kr
            "https://api.text.or.kr", // 서비스 중
            "https://bible.text.or.kr", // * bible web
            "https://chat.text.or.kr", // 서비스 중
            "https://code.text.or.kr", // 서비스 중
            "https://iam.text.or.kr", // 서비스 중
            "https://ip.text.or.kr", // * bible api
            "https://ns.text.or.kr", // ? 라운드로빈 테스트
            "https://suda.text.or.kr", // 서비스 중
            "https://writer.text.or.kr", // 서비스 중
            "https://www.text.or.kr", // 서비스 중

            "https://kimbumjun.com", //--> (4) kimbumjun.com // 서비스 중
            "https://api.kimbumjun.com", // 서비스 중
            "https://bj.kimbumjun.com", // 서비스 중
            "https://code.kimbumjun.com", // 서비스 중
            "https://iam.kimbumjun.com", // 서비스 중
            "https://runner.kimbumjun.com", // * RUNNER, NEXT_PUBLIC_RUNNER_URL
            "https://www.kimbumjun.com", // 서비스 중

            "https://kimbumjun.co.kr", //--> (5) kimbumjun.co.kr // 서비스 중
            "https://api.kimbumjun.co.kr", // 서비스 중
            "https://bj.kimbumjun.co.kr", // 서비스 중
            "https://code.kimbumjun.co.kr", // 서비스 중
            "https://iam.kimbumjun.co.kr", // 서비스 중
            "https://ns.kimbumjun.co.kr", // * CHAT, NEXT_PUBLIC_CHAT_URL, chathub
            "https://www.kimbumjun.co.kr", // 서비스 중

            "https://vivabm.com", //--> (6) vivabm.com // 서비스 중
            "https://api.vivabm.com", // * main api for code
            "https://bj.vivabm.com", // 서비스 중
            "https://bm.vivabm.com", // 서비스 중
            "https://code.vivabm.com", // 서비스중
            "https://viv.vivabm.com", // 서비스 중
            "https://www.vivabm.com", // 서비스 중

            "https://writer.or.kr", //--> (7) writer.or.kr, ?
            "https://api.writer.or.kr", // ?
            "https://code.writer.or.kr", // 서비스 중
            "https://iam.writer.or.kr", // 서비스 중
            "https://ip.writer.or.kr", // docker, 5028
            "https://ns.writer.or.kr", // 서비스 중
            "https://www.writer.or.kr" // 서비스 중 
        ];
    }
    
}