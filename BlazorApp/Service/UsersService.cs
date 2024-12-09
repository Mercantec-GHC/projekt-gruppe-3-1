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

        public void Print()
        {
            Console.WriteLine($"Id: {Id}, Name: {Name}, Mobile: {Mobile}, Email: {Email}, City: {City}, Address: {Address}");
        }
    }
}
