namespace Sehaty.Core.Specifications.Notifications_Specs
{
    public class NotificationSpecifications : BaseSpecefication<Notification>
    {
        public NotificationSpecifications()
        {
            AddIncludes();
        }
        public NotificationSpecifications(int id) : base(N => N.Id == id)
        {
            AddIncludes();
        }
        public NotificationSpecifications(Expression<Func<Notification, bool>> criteria) : base(criteria)
        {
            AddIncludes();
        }
        void AddIncludes()
        {
            Includes.Add(P => P.User);

        }
    }
}
