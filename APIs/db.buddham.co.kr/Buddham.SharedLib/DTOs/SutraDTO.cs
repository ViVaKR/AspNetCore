using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Buddham.SharedLib.Models;

namespace Buddham.SharedLib.DTOs;

public class SutraDTO
{
    /// <summary>
    /// 번호
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 제목
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; } = string.Empty;

    /// <summary>
    /// 부제목
    /// </summary>
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; } = string.Empty;

    /// <summary>
    /// 저자
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; } = "미상";

    /// <summary>
    /// 번역자
    /// </summary>
    [JsonPropertyName("translator")]
    public string? Translator { get; set; } = string.Empty;

    /// <summary>
    /// 요약
    /// </summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; } = string.Empty;

    /// <summary>
    /// 불경본문
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// 원문
    /// </summary>
    [JsonPropertyName("originalText")]
    public string? OriginalText { get; set; } = string.Empty;


    [JsonPropertyName("annotation")]
    public string? Annotation { get; set; } = string.Empty;


    [EnumDataType(typeof(HangulOrder))]
    [JsonPropertyName("hangulOrder")]
    public HangulOrder? HangulOrder { get; set; }

    /// <summary>
    /// 작성일
    /// </summary>
    [DataType(DataType.DateTime)]
    [JsonPropertyName("created")]
    public DateTime Created { get; set; } = DateTime.Now;


    /// <summary>
    /// 작성자 아이디
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; } = string.Empty;

    /// <summary>
    /// 작성자 이름
    /// </summary>
    [JsonPropertyName("userName")]
    public string? UserName { get; set; } = string.Empty;
}
