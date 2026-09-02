using NetAuth.Domain.Common;

namespace NetAuth.Domain.Entities
{
    internal class UserProfile : AuditableEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string BloodGroup { get; set; }
        public string PersonalEmail { get; set; }
        public string DateOfJoining { get; set; }
        public string PassportNumber { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string MaritalStatus { get; set; }
        public string WeddingAnniversaryDate { get; set; }
        public string SpouseName { get; set; }
        public string SpouseDOB { get; set; }
        public string HomeAddress1 { get; set; }
        public string HomeAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string HomeAddressCity { get; set; }
        public string HomeAddressState { get; set; }
        public string HomeAddressCountry { get; set; }
        public string HomePhoneNumber { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactNumber { get; set; }
        public string PrimarySkills { get; set; }
        public string SecondarySkills { get; set; }
        public string TertiarySkills { get; set; }
        public string OtherSkills { get; set; }
        public int Branch { get; set; }
        public string LookupCode { get; set; }
        public string OtherId { get; set; }
        public string LinkedInUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string Extension { get; set; }
        public string Email { get; set; }
    }
}
