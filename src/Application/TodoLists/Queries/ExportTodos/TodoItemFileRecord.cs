using WASMClean.Application.Common.Mappings;
using WASMClean.Domain.Entities;

namespace WASMClean.Application.TodoLists.Queries.ExportTodos;

public class TodoItemRecord : IMapFrom<TodoItem>
{
    public string? Title { get; set; }

    public bool Done { get; set; }
}
