using WASMClean.Application.Common.Interfaces;

namespace WASMClean.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
