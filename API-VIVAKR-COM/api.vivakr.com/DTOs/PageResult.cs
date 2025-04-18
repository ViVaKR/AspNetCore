namespace ViVaKR.API.DTOs
{
    // 제네릭 페이징 결과 DTO
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = []; // 현재 페이지의 아이템 목록
        public int TotalCount { get; set; }     // 필터링된 전체 아이템 수 (페이징 전)
    }
}
