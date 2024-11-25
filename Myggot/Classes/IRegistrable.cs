using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myggot.Classes
{
    // Create an interface with common properties
    public interface IRegistrable
    {
        int Id { get; set; }
        string Username { get; set; }
        string Telephone { get; set; }
        string Address { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        bool IsValid();
    }
}

