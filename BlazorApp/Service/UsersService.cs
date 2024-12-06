using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualBasic.FileIO;
using System.Drawing;
using static BlazorApp.Service.MiniCooper;

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

        public void Print_2()
        {
            Console.WriteLine($"Id: {Id}\n" +
                              $"Name: {Name}\n" +
                              $"Password: {Password}\n" +
                              $"Mobile: {Mobile}\n" +
                              $"Email: {Email}\n" +
                              $"City: {City}\n" +
                              $"Address: {Address}\n");
        }

        public async Task SetBaseUserModel(User model)
        {
            Name = model.Name;
            Password = model.Password;
            Mobile = model.Mobile;
            Email = model.Email;
            City = model.City;
            Address = model.Address;
        }
    }



    public class EvMiniCooper : User
    {
        public int ChargeCapacity { get; set; }
        public float KmPrKwh { get; set; }

        public void Print()
        {
            Print_2();
            Console.WriteLine($"test");
        }
    }


    public class FullUser
    {
        private EvMiniCooper? EvUser { get; set; }

        public EvMiniCooper? GetEvUser()
        {
            return EvUser;
        }

        public void Clear()
        {
            Console.WriteLine("Clearing users...");
            EvUser = null;
        }

        public void PrintEv()
        {
            if (EvUser == null)
                Console.WriteLine("User is null!");
            else
                EvUser?.Print();
        }

        public void PrintAutomatically()
        {
            if (EvUser != null)
                EvUser.Print();
            else
                Console.WriteLine("No user has been assigned to this object.");
        }

        //public bool ThereCanOnlyBeOne()
        //{
        //    if (EvUser != null)
        //    {
        //        Console.WriteLine("There is already an Electric Cooper");
        //        return false;
        //    }


        //    return true;
        //}

        //public void SetMiniCooper(EvMiniCooper Evuser)
        //{
        //    if (ThereCanOnlyBeOne())
        //        EvUser = Evuser;
        //    else
        //        Console.WriteLine("A car has already been assigned to this object.");
        //}


    }

}