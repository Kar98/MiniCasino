using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiniCasino.Patrons.Person;

namespace MiniCasino.Patrons
{
    public interface iPerson
    {
        string GetFirstname();
        string GetLastname();
        AddressDetails GetAddress();
        int GetAge();
        DateTime GetBirthday();
        char GetSex();
    }
}
