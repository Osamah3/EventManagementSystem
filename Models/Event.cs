using System.ComponentModel.DataAnnotations.Schema;

namespace Trion.Models
{
    public class Event
    {
       
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly Date { get; set; }

       
        public string EventDescription { get; set; }

        public int venueid { get; set; }
        public Venue Venue { get; set; }

        public Event()
        {
                
        }
    }
}
