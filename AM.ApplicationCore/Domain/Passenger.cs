using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AM.ApplicationCore.Domain
{
    public class Passenger
    {
        public int Id { get; set; }

        [StringLength(7)]
        public string PassportNumber { get; set; }
        //public FullName FullName { get; set; }

        public FullName FullName { get; set; }

        [Display(Name ="Date of birth")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        
        public string TelNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        //prop de navigation
        public virtual ICollection<Flight> Flights { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }



        //TP1-Q6: Réimplémenter la méthode ToString()
        public override string ToString()
        {
            return /*"FirstName: " + FirstName + " LastName: " + LastName +*/ " date of Birth: "+ BirthDate;
        }

        //TP1-Q10: Créer les trois méthodes bool CheckProfile(...)
        //public bool CheckProfile(string firstName, string lastName)
        //{
        //    return FirstName == firstName && LastName == lastName;
        //}

        //public bool CheckProfile(string firstName, string lastName, string email)
        //{
        //    return FirstName == firstName && LastName == lastName && EmailAddress == email;
        //}

        //public bool CheckProfile(string firstName, string lastName, string email = null)
        //{
        //    if (email != null)
        //        return FirstName == firstName && LastName == lastName && EmailAddress == email;
        //    else
        //        return FirstName == firstName && LastName == lastName;
        //}

        //TP1-Q11.a: Implémenter la méthode PassengerType()
        public virtual void PassengerType()
        {
            Console.WriteLine("I am a Passenger");
        }

    }
}
