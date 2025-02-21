using TodoApi.Models;

namespace TodoApi.DTOs;

public class TodoItemDTO
{
	// 클래스 멤버 정의
	public int Id { get; set; }
	public string? Name { get; set; }
	public bool IsComplete { get; set; }

	public TodoItemDTO() { }

	public TodoItemDTO(Todo todoItem) =>
	(Id, Name, IsComplete) = (todoItem.Id, todoItem.Name, todoItem.IsComplete);
}
