using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCrud.BLL
{
    public class FixedText
    {
        public static string  FixedEmail(string email)
        {
            return email.ToLower().Trim();
        }
    }
}
