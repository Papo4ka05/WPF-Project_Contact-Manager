using System;

namespace ContactManager
{
    public class Contact
    {
        public int? CategoryId { get; set; }
        public DateTime? DateOfBirth { get; set; }  // ? Eigenschaft darf null sein
        public string Email { get; set; }
        public string FirstName { get; set; }
        public int Id { get; set; }
        public string LastName { get; set; }
        public string Note { get; set; }
        public string PhoneNumber { get; set; }
        public int UserId { get; set; }
    }
}