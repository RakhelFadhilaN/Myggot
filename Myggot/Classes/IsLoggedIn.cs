using System.Windows.Forms;

namespace Myggot.Classes
{
    public class LoggedInAccount
    {
        public IRegistrable Account { get; private set; }
        public bool IsLoggedIn { get; private set; }

        public LoggedInAccount()
        {
            IsLoggedIn = false;
        }

        // Set the logged-in account
        public void SetLoggedInAccount(IRegistrable account)
        {
            if (account == null)
            {
                MessageBox.Show("Attempted to set a null account.", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Account = account;
            IsLoggedIn = true;
        }


        // Corrected method: get the logged-in account's ID (either UserId or FarmerId)
        public int? GetLoggedInAccountId()
        {
            if (IsLoggedIn)
            {
                if (Account is User user)
                {
                    return user.Id;  // Assuming UserId is an int or nullable int
                }
                else if (Account is Farmer farmer)
                {
                    return farmer.Id;  // Assuming FarmerId is an int or nullable int
                }
            }
            return null;
        }

        // Check if logged-in account is a Farmer
        public bool IsFarmer()
        {
            return Account is Farmer;
        }

        // Check if logged-in account is a User
        public bool IsUser()
        {
            return Account is User;
        }
    }
}
