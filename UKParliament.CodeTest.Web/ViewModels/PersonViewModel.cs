namespace UKParliament.CodeTest.Web.ViewModels
{
    public class PersonViewModel
    {
        public required int Id { get; set; }
        public required int DepartmentId { get; set; }
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DOB { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public string? AddressLine4 { get; set; }
        public string? PostCode { get; set; }
       
      
    }
}
