using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Requests.User
{
    public class UpdateUserRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
    }
}
