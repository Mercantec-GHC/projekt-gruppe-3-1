namespace BlazorApp.Service;

public class UsersService
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Mobile { get; set; }
        public string Email { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public void SetUser(int id, string name, string password, int mobile, string email, string city, string address)
        {
            Id = id;
            Name = name;
            Password = password;
            Mobile = mobile;
            Email = email;
            City = city;
            Address = address;
        }

        public void Print()
        {
            Console.WriteLine($"Id: {Id}\n" +
                              $"Name: {Name}\n" +
                              $"Password: {Password}\n" +
                              $"Mobile: {Mobile}\n" +
                              $"Email: {Email}\n" +
                              $"City: {City}\n" +
                              $"Address: {Address}\n");
        }
    }
    
}