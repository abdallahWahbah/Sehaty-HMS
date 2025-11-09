namespace Sehaty.Core.Specifications.PatientSpec
{
    public class PatientSpecsParams
    {
        public int? Id { get; set; }
        public string Sort { get; set; }
        public string MRN { get; set; }
        public string Name { get; set; }
        public bool Descending { get; set; }

        private int pageSize = 5;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > 10 || value <= 0 ? 10 : value; }
        }

        private int pageIndex;
        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value <= 0 ? 1 : value; }
        }


    }
}
