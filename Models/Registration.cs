using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trion.Models
{
    public enum RegistrationStatus
    {
        Registered,
        Canceled,
        Attended
    }
    public class Registration
    {
        public int Id { get; set; }
        [ForeignKey("Event")]
        public int EventId { get; set; }
        
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public IdentityUser User { get; set; }
        public DateOnly Date { get; set; }
        public RegistrationStatus Status { get; set; }
        public string PhoneNumber { get; set; }
        public string RegistrationName{get; set; }


        public Registration()
        {
               

        }
    }
}
