using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Patrons
{
    public abstract class Person
    {
        private string _address;
        private int _age;
        private DateTime _birthday;
        private char _sex;
        private bool _verified;

        public string Address { get => _address; set => _address = value; }
        public int Age { get => _age; set => _age = value; }
        public DateTime Birthday { get => _birthday; set => _birthday = value; }
        public char Sex { get => _sex; set => _sex = value; }
        public bool Verified { get => _verified; set => _verified = value; }

        //private Identification ID;

        public Person(string address, DateTime bday, char sex)
        {
            this.Address = address;
            CalculateAge(bday);
            this.Birthday = bday;
            this.Sex = sex;
            this.Verified = false;
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




    }
}
