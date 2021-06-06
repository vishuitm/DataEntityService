
namespace DataEntityService.Entities
{
    using System;

    public class Guest
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Void { get; set; }
    }
}