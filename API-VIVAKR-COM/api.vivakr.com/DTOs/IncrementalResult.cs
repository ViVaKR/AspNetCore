namespace ViVaKR.API.DTOs;

// 점진적 로딩 결과 DTO
public class IncrementalResult<T>
{
    public List<T> Items { get; set; } = []; // 현재 청크의 아이템 목록
    public int TotalCount { get; set; }     // 전체 아이템 수
}
