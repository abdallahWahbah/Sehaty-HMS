using Sehaty.Core.Entites;
using System.Linq.Expressions;

namespace Sehaty.Core.Specefications
{
    public interface ISpecefication<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; }
        public List<string> IncludeStrings { get; set; }
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool HasPagination { get; set; }



    }
}
