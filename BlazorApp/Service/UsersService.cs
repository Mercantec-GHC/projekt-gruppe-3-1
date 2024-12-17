using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Service;

public class UsersService
{
    public class User
    {
        public int Id { get; set; }

        // Validering af felter
        [Required(ErrorMessage = "Fulde navn er påkrævet")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Fulde navn skal være mellem 3 og 100 tegn")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kodeord er påkrævet")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Kodeord skal være mindst 6 tegn")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobilnummer er påkrævet")]
        [Range(10000000, 99999999, ErrorMessage = "Mobilnummer skal være et gyldigt 8-cifret nummer")]
        public int Mobile { get; set; }

        [Required(ErrorMessage = "E-mail er påkrævet")]
        [EmailAddress(ErrorMessage = "Ugyldig e-mail adresse")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "By er påkrævet")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adresse er påkrævet")]
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