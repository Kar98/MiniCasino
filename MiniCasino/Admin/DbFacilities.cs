using MiniCasino.Patrons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCasino.Admin
{
    //TODO: Create basic DB facilities front end for create/update patron details
    public class DbFacilities
    {

        public int CreatePatron(Patron p)
        {
            var sp = "[dbo].[AddPatron]";
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("@firstname", p.Firstname);
            dict.Add("@lastname", p.Lastname);
            dict.Add("@sex", p.Sex);
            dict.Add("@verified", p.Verified);
            dict.Add("@birthday", p.Birthday);
            dict.Add("@returnid", -1);

            var retId = Db.RunSpWithReturnId(sp, dict);
            return retId;
        }

        public int CreateStaff(Person p, int departmentId)
        {
            var sp = "[dbo].[AddStaff]";
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("@firstname", p.Firstname);
            dict.Add("@lastname", p.Lastname);
            dict.Add("@sex", p.Sex);
            dict.Add("@birthday", p.Birthday);
            dict.Add("@deptid", departmentId);

            var retId = Db.RunSp(sp, dict);

            return retId;
        }
    }
}
