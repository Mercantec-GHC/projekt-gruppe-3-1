using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Service;

public class UsersService
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public int Mobile { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;

        public void SetUser(int id, string name, string password, int mobile, string email, string city, string address)
        {
            Console.WriteLine("Setting user...");
            Id = id;
            Name = name;
            Password = password;
            Mobile = mobile;
            Email = email;
            City = city;
            Address = address;
        }

        public void SetUser(User user)
        {
            Console.WriteLine("Setting user...");
            Id = user.Id;
            Name = user.Name;
            Password = user.Password;
            Mobile = user.Mobile;
            Email = user.Email;
            City = user.City;
            Address = user.Address;
        }

        public void Clear()
        {
            Console.WriteLine("Clearing user...");
            Id = 0;
            Name = string.Empty;
            Password = string.Empty;
            Mobile = 0;
            Email = string.Empty;
            City = string.Empty;
            Address = string.Empty;
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