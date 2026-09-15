namespace OrderManagement.Application.Abstractions;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);
