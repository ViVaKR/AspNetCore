namespace ViVaKR.API.DTOs;

public class CodeQueryParameters
{
    public int Page { get; set; } = 1;        // 요청 페이지 번호 (기본값 1)
    public int PageSize { get; set; } = 25;   // 페이지당 아이템 수 (기본값 25)
    public string? SortField { get; set; }    // 정렬할 필드 이름 (예: "id", "title", "modified")
    public string? SortOrder { get; set; }    // 정렬 순서 ("asc" 또는 "desc")
    public int? CategoryId { get; set; }     // 필터링할 카테고리 ID (선택 사항)
    public string? SearchTerm { get; set; }   // 검색어 (제목, 부제목 등 검색용, 선택 사항)
    public string? UserId { get; set; }       // 필요한 다른 필터 조건 추가 가능 (예: userId)
}

