using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp.Service;

public class Users
{
    public class BaseUsers
    {
        public string ModelName { get; set; } = string.Empty;
        public int Generation { get; set; }
        public string ModelType { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Mileage { get; set; }
        public int MaxRange { get; set; }
        public int Weight { get; set; }
        public string FuelType { get; set; } = string.Empty; // Fossil/Diesel/Electricity/Hybrid.
        public string GearType { get; set; } = string.Empty;
        public decimal YearlyTax { get; set; }

        public void PrintBaseUsers()
        {
            Console.WriteLine($"__Mini-Users__\n" +
                              $"Model name: {ModelName}\n" +
                              $"Generation: {Generation}\n" +
                              $"Model type: {ModelType}\n" +
                              $"Color: {Color}\n" +
                              $"Price: {Price}\n" +
                              $"Mileage: {Mileage}\n" +
                              $"Max range: {MaxRange}\n" +
                              $"Weight: {Weight}\n" +
                              $"Fuel type: {FuelType}\n" +
                              $"Geartype: {GearType}\n" +
                              $"Yearly tax: {YearlyTax}");

        }

        public async Task SetBaseUsersModel(BaseUsers model)
        {
            ModelName = model.ModelName;
            Generation = model.Generation;
            ModelType = model.ModelType;
            Color = model.Color;
            Price = model.Price;
            Mileage = model.Mileage;
            MaxRange = model.MaxRange;
            Weight = model.Weight;
            FuelType = model.FuelType;
            GearType = model.GearType;
            YearlyTax = model.YearlyTax;
        }

        public class EvUsers : BaseUsers
        {
            public int ChargeCapacity { get; set; }
            public float KmPrKwh { get; set; }

            public void Print()
            {
                PrintBaseUsers();
                Console.WriteLine($"Charge capacity: {ChargeCapacity}\n" +
                                  $"Km. pr. kilowatt hour: {KmPrKwh}");
            }
        }
        public class FullMiniUsers
        {
            private EvUsers? EvUsers { get; set; }

            public EvUsers? GetEvUsers()
            {
                return EvUsers;
            }


            public void Clear()
            {
                Console.WriteLine("Clearing cars...");
                EvUsers = null;
            }

            public void PrintEv()
            {
                if (EvUsers == null)
                    Console.WriteLine("Electric Users is null!");
                else
                    EvUsers?.Print();
            }

            public void SetMiniUsers(EvUsers evUsers)
            {
                EvUsers = evUsers;
            }
        }

    }   
}