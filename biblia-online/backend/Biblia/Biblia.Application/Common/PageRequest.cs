namespace Biblia.Application.Common;

public sealed record PageRequest(int Page, int PageSize)
{
    public static PageRequest Create(int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;
        return new PageRequest(page, pageSize);
    }

    public int Skip => (Page - 1) * PageSize;
}
