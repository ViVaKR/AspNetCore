using System.ComponentModel.DataAnnotations;

namespace ViVaKR.API.DTOs;

public class CreateSubscribeDto
{
    [Required(ErrorMessage = "이메일은 필수입니다.")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "유효한 이메일 형식이 아닙니다.")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
}
