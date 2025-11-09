using Sehaty.Core.Entites;
using System.Linq.Expressions;

namespace Sehaty.Core.Specifications.PatientSpec
{
    public class PatientSearchSpecification : PatientSpecifications
    {
        public PatientSearchSpecification(PatientSpecsParams param)
            : base(P =>
            ((!param.Id.HasValue || P.Id == param.Id)) &&
            ((String.IsNullOrEmpty(param.MRN) || P.MRN.ToLower().Contains(param.MRN))) &&
            ((String.IsNullOrEmpty(param.Name) || P.FirstName.ToLower().Contains(param.Name.ToLower())
            || P.LastName.ToLower().Contains(param.Name.ToLower())))
            )
        {
            ApplySorting(param);
            Pagination(param);
        }

        private void Pagination(PatientSpecsParams param)
        {
            var pageIndex = param.PageIndex <= 0 ? 1 : param.PageIndex;
            var pageSize = param.PageSize <= 0 || param.PageSize > 10 ? 10 : param.PageSize;
            var skip = (pageIndex - 1) * pageSize;

            AddPagination(skip, pageSize);
        }

        private void ApplySorting(PatientSpecsParams param)
        {
            var sortKey = param.Sort?.Trim().ToLower();

            Expression<Func<Patient, object>> sortExpression = sortKey switch
            {
                "id" => p => p.Id,
                "mrn" => p => p.MRN,
                "firstname" => p => p.FirstName,
                "lastname" => p => p.LastName,
                "dateofbirth" => p => p.DateOfBirth,
                "registrationdate" => p => p.RegistrationDate,
                "status" => p => p.Status,
                _ => p => p.FirstName // Default sort
            };

            if (param.Descending)
                AddOrderByDesc(sortExpression);
            else
                AddOrderBy(sortExpression);
        }
    }
}

