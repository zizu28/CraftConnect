using System.Linq.Expressions;

namespace Core.SharedKernel.Contracts
{
	public class PagedResult<T>
	{
		public IEnumerable<T> Items { get; set; } = [];
		public int TotalCount { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
		public bool HasNextPage => PageNumber < TotalPages;
		public bool HasPreviousPage => PageNumber > 1;
	}

	public class QueryParameters
	{
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string? SearchTerm { get; set; }
		public string? SortBy { get; set; }
		public bool SortDescending { get; set; } = false;
	}

	public interface IQueryableRepository<T> : IRepository<T> where T : class
	{
		Task<PagedResult<T>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default);
		Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, QueryParameters parameters, CancellationToken cancellationToken = default);
		Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
	}
}