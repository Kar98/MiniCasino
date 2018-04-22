using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Patrons
{
    public class Patron : Person
    {
        protected double money = 0f;
        AddressDetails _address;

        public Patron(string address, DateTime bday, char sex) : base(address, bday, sex)
        {

        }

        public Patron(DateTime bday, char sex, bool verified, string firstname, string lastname) : base(bday,sex,firstname,lastname)
        {
            this.Birthday = bday;
            this.Sex = sex;
            this.Verified = verified;
            SetFirstname(firstname);
            SetLastname(lastname);
            _address = new AddressDetails();

        }

        public double Money { get => money; set => money = value; }

        public double GetMoney()
        {
            return money;
        }

    }

}
