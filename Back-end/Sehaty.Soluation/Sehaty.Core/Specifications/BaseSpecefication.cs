using Sehaty.Core.Entites;
using System.Linq.Expressions;

namespace Sehaty.Core.Specefications
{
    public class BaseSpecefication<T> : ISpecefication<T> where T : BaseEntity
    {
        public BaseSpecefication()
        {

        }
        public BaseSpecefication(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<T, bool>> Criteria { get; set; } = null;
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
    }
}
