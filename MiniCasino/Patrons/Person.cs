using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Patrons
{
    public abstract class Person : iPerson
    {
        public int Id { get; protected set; }
        public AddressDetails Address { get; protected set; }
        public int Age { get; protected set; }
        public DateTime Birthday { get; protected set; }
        public char Sex { get; protected set; }
        public bool Verified { get; protected set; }
        public string Lastname { get; protected set; }
        public string Firstname { get; protected set; }
        public bool PlayerControlled { get; set; }
        /**public string Lastname { get; protected set; }
        public string Firstname { get; protected set; }*/

        public Person(DateTime bday, char sex, int id = -1)
        {
            this.Address = new AddressDetails();
            CalculateAge(bday);
            this.Birthday = bday;
            this.Sex = sex;
            this.Verified = false;
            this.Id = Id;
        }

        public Person(string address, DateTime bday, char sex, int id = -1)
        {
            this.Address = new AddressDetails(address);
            CalculateAge(bday);
            this.Birthday = bday;
            this.Sex = sex;
            this.Verified = false;
            this.Id = id;
        }
        
        public string GetLastname()
        {
            return Lastname;
        }

        public void SetLastname(string s)
        {
            Lastname = s;
        }

        public string GetFirstname()
        {
            return Firstname;
        }
        public void SetFirstname(string s)
        {
            Firstname = s;
        }

        public bool VerifyPerson(string args)
        {
            //put verification logic here.
            Verified = true;
            return Verified;
        }

        private void CalculateAge(DateTime dt)
        {
            var currentDate = DateTime.Now;
            var diff = currentDate.Subtract(dt);
        }

        public AddressDetails GetAddress()
        {
            return Address;
        }

        public string GetAddressAsString()
        {
            return Address.QualifiedAddress;
        }

        public int GetAge()
        {
            return Age;
        }

        public DateTime GetBirthday()
        {
            return Birthday;
        }

        public char GetSex()
        {
            return Sex;
        }

        public int GetId()
        {
            return Id;
        }

        public class AddressDetails
        {
            public int Unitnumber { get; set; }
            public string Housenumber { get; set; }
            public string Streetname { get; set; }
            public string Streettype { get; set; }
            public string City { get; set; }
            public string Postcode { get; set; }
            public string QualifiedAddress { get; set; }

            public AddressDetails() { }

            public AddressDetails(string fullAddress) {
                QualifiedAddress = fullAddress;
            }

        }


    }
}
