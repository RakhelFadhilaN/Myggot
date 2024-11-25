namespace Myggot.Classes
{
    // User class implementing the interface
    public class User : IRegistrable
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string TotalPoints { get; set; }
        public string TotalWaste { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Username) &&
                   !string.IsNullOrEmpty(Email) &&
                   !string.IsNullOrEmpty(Password);
        }
    }

    // Farmer class implementing the interface
    public class Farmer : IRegistrable
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string TotalWaste { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Username) &&
                   !string.IsNullOrEmpty(Email) &&
                   !string.IsNullOrEmpty(Password);
        }
    }
}
