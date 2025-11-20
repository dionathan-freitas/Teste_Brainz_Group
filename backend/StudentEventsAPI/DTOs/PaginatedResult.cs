namespace StudentEventsAPI.DTOs;

public record PaginatedResult<T>(IReadOnlyCollection<T> Data, int Page, int PageSize, int TotalCount, int TotalPages);
