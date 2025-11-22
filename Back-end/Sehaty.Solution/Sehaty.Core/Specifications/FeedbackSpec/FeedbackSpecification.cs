namespace Sehaty.Core.Specifications.FeedbackSpec
{
    public class FeedbackSpecification : BaseSpecefication<Feedback>
    {
        public FeedbackSpecification()
        {
            AddIncludes();
        }
        public FeedbackSpecification(int id) : base(F => F.Id == id)
        {
            AddIncludes();
        }
        public FeedbackSpecification(Expression<Func<Feedback, bool>> criteria) : base(criteria)
        {
            AddIncludes();
        }

        void AddIncludes()
        {
            Includes.Add(F => F.Appointment);
        }

    }
}
