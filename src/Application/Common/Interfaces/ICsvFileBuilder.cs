using WASMClean.Application.TodoLists.Queries.ExportTodos;

namespace WASMClean.Application.Common.Interfaces;

public interface ICsvFileBuilder
{
    byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
}
