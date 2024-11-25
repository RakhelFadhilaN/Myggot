using System;
using System.Collections.Generic;

public class User
{
    public int UserID { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int TotalPoint { get; set; }

    private static List<User> users = new List<User>();

    public User() { }

    public void Register(string name, string phoneNumber, string address, string email, string password)
    {
        this.UserID = users.Count + 1;
        this.Name = name;
        this.PhoneNumber = phoneNumber;
        this.Address = address;
        this.Email = email;
        this.Password = password;
        this.TotalPoint = 0;

        users.Add(this);
        Console.WriteLine($"{Name} registrasi berhasil.");
    }

    public bool Login(string email, string password)
    {
        User user = users.Find(u => u.Email == email && u.Password == password);
        if (user != null)
        {
            Console.WriteLine($"{user.Name} login berhasil.");
            return true;
        }
        else
        {
            Console.WriteLine("Kredensial tidak valid.");
            return false;
        }
    }

    public void Logout()
    {
        Console.WriteLine($"{Name} logout.");
    }

    public static User FindUserByEmail(string email)
    {
        return users.Find(u => u.Email == email);
    }
}

public class Farmer : User
{
    public int TotalWaste { get; set; }
    private static List<Farmer> farmers = new List<Farmer>();

    public Farmer() { }

    public void AddWaste(int waste)
    {
        this.TotalWaste += waste;
        Console.WriteLine($"Petani {Name} menambahkan {waste} sampah. Jumlah sampah: {TotalWaste}.");
    }

    public static Farmer FindFarmerByID(int farmerID)
    {
        return farmers.Find(f => f.UserID == farmerID);
    }
}

public class Order
{
    public int OrderID { get; set; }
    public string OrderAddress { get; set; }
    public DateTime OrderDate { get; set; }
    public int OrderUserID { get; set; }
    public int OrderFarmerID { get; set; }
    public int Weight { get; set; }
    public int Point { get; set; }
    private static List<Order> orders = new List<Order>();

    public Order() { }

    public void InitializeOrder(User user, Farmer farmer, int weight)
    {
        this.OrderID = orders.Count + 1;
        this.OrderUserID = user.UserID;
        this.OrderFarmerID = farmer.UserID;
        this.OrderDate = DateTime.Now;
        this.Weight = weight;
        this.Point = CalculatePoints(weight);
        orders.Add(this);

        Console.WriteLine($"Pesanan {OrderID} dibuat oleh {user.Name} dengan petani {farmer.Name}.");
    }

    public void EndOrder()
    {
        Console.WriteLine($"Pesanan {OrderID} selesai pada {OrderDate}.");
    }

    private int CalculatePoints(int weight)
    {
        return weight;
    }
}

public class Feedback
{
    public int FeedbackID { get; set; }
    public string FeedbackText { get; set; }
    public int Star { get; set; }
    private static List<Feedback> feedbacks = new List<Feedback>();

    public Feedback() { }

    public void NewFeedback(int userID, string feedback, int star)
    {
        this.FeedbackID = feedbacks.Count + 1;
        this.FeedbackText = feedback;
        this.Star = star;

        feedbacks.Add(this);
        Console.WriteLine($"Masukan dari {userID}: {feedback} ({star} bintang)");
    }
}

public class PointHistory
{
    public int PointID { get; set; }
    public int Point { get; set; }
    public DateTime PointDate { get; set; }
    private static List<PointHistory> pointHistories = new List<PointHistory>();

    public void AddPoint(User user, int points)
    {
        this.PointID = pointHistories.Count + 1;
        this.Point = points;
        this.PointDate = DateTime.Now;
        user.TotalPoint += points;

        pointHistories.Add(this);
        Console.WriteLine($"{user.Name} Mendapatkan {points} poin. Total poin: {user.TotalPoint}.");
    }

    public void UsePoints(User user, int points)
    {
        if (user.TotalPoint >= points)
        {
            user.TotalPoint -= points;
            Console.WriteLine($"{user.Name} Menggunakan {points} poin. Sisa poin: {user.TotalPoint}.");
        }
        else
        {
            Console.WriteLine("Poin tak mencukupi.");
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Selamat datang di Myggot");

        Console.WriteLine("Registrasi atau login? (register/login)");
        string action = Console.ReadLine().ToLower();

        if (action == "register")
        {
            Console.WriteLine("Masukan nama:");
            string name = Console.ReadLine();

            Console.WriteLine("Masukan nomor telepon:");
            string phoneNumber = Console.ReadLine();

            Console.WriteLine("Masukan alamat:");
            string address = Console.ReadLine();

            Console.WriteLine("Masukan email:");
            string email = Console.ReadLine();

            Console.WriteLine("Masukan sandi:");
            string password = Console.ReadLine();

            User user1 = new User();
            user1.Register(name, phoneNumber, address, email, password);

            MainMenu(user1);
        }
        else if (action == "login")
        {
            Console.WriteLine("Masukan email:");
            string email = Console.ReadLine();

            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();

            User user1 = User.FindUserByEmail(email);
            if (user1 != null && user1.Login(email, password))
            {
                MainMenu(user1);
            }
            else
            {
                Console.WriteLine("Login gagal.");
            }
        }
    }

    static void MainMenu(User user)
    {
        while (true)
        {
            Console.WriteLine("\nAda yang bisa kami bantu?");
            Console.WriteLine("1. Buat pesanan");
            Console.WriteLine("2. Berikan masukan");
            Console.WriteLine("3. Gunakan poin");
            Console.WriteLine("4. Logout");
            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    PlaceOrder(user);
                    break;
                case 2:
                    AddFeedback(user);
                    break;
                case 3:
                    UsePoints(user);
                    break;
                case 4:
                    user.Logout();
                    return;
            }
        }
    }

    static void PlaceOrder(User user)
    {
        Console.WriteLine("Masukan nama petani:");
        string farmerName = Console.ReadLine();

        Farmer farmer = new Farmer();
        farmer.Register(farmerName, "FarmerPhone", "FarmerAddress", "farmer@example.com", "farmerPassword");

        Console.WriteLine("Masukan berat sampah (kg):");
        int weight = Convert.ToInt32(Console.ReadLine());

        Order order = new Order();
        order.InitializeOrder(user, farmer, weight);

        PointHistory pointHistory = new PointHistory();
        pointHistory.AddPoint(user, weight);
    }

    static void AddFeedback(User user)
    {
        Console.WriteLine("Berikan masukan:");
        string feedbackText = Console.ReadLine();

        Console.WriteLine("Berikan rating (1-5):");
        int rating = Convert.ToInt32(Console.ReadLine());

        Feedback feedback = new Feedback();
        feedback.NewFeedback(user.UserID, feedbackText, rating);
    }

    static void UsePoints(User user)
    {
        Console.WriteLine($"Poinmu {user.TotalPoint} points.");
        Console.WriteLine("Poin yang ingin digunakan:");
        int points = Convert.ToInt32(Console.ReadLine());

        PointHistory pointHistory = new PointHistory();
        pointHistory.UsePoints(user, points);
    }
}