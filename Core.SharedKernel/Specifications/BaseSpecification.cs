using System.Linq.Expressions;

namespace Core.SharedKernel.Specifications
{
	public interface ISpecification<T>
	{
		Expression<Func<T, bool>> Criteria { get; }
		List<Expression<Func<T, object>>> Includes { get; }
		List<string> IncludeStrings { get; }
		Expression<Func<T, object>>? OrderBy { get; }
		Expression<Func<T, object>>? OrderByDescending { get; }
		Expression<Func<T, object>>? GroupBy { get; }
		int Take { get; }
		int Skip { get; }
		bool IsPagingEnabled { get; }
	}

	public abstract class BaseSpecification<T> : ISpecification<T>
	{
		public Expression<Func<T, bool>> Criteria { get; private set; } = null!;
		public List<Expression<Func<T, object>>> Includes { get; } = [];
		public List<string> IncludeStrings { get; } = [];
		public Expression<Func<T, object>>? OrderBy { get; private set; }
		public Expression<Func<T, object>>? OrderByDescending { get; private set; }
		public Expression<Func<T, object>>? GroupBy { get; private set; }
		public int Take { get; private set; }
		public int Skip { get; private set; }
		public bool IsPagingEnabled { get; private set; }

		protected BaseSpecification(Expression<Func<T, bool>> criteria)
		{
			Criteria = criteria;
		}

		protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
		{
			Includes.Add(includeExpression);
		}

		protected virtual void AddInclude(string includeString)
		{
			IncludeStrings.Add(includeString);
		}

		protected virtual void ApplyPaging(int skip, int take)
		{
			Skip = skip;
			Take = take;
			IsPagingEnabled = true;
		}

		protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
		{
			OrderBy = orderByExpression;
		}

		protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
		{
			OrderByDescending = orderByDescendingExpression;
		}

		protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
		{
			GroupBy = groupByExpression;
		}
	}
}