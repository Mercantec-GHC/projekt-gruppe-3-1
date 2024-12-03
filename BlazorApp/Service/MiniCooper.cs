using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp.Service;

public class MiniCooper
{
    public class BaseMiniCooper
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
        public List<string> Base64Images { get; set; } = new();
        
        public void PrintBaseMiniCooper()
        {
            Console.WriteLine($"__Mini-Cooper__\n" +
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
            if (Base64Images.Count > 0)
                Console.WriteLine($"Images: {Base64Images.Count}");
            else
                Console.WriteLine("No image ID's added");
        }

        public async Task SetBaseMiniCooperModel(BaseMiniCooper model)
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
            Base64Images = model.Base64Images;
        }

        public async Task AddImage(IBrowserFile image)
        {
            using var memoryStream = new MemoryStream();
            await image.OpenReadStream().CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();
            var base64Image = Convert.ToBase64String(imageBytes);
            Base64Images.Add(base64Image);
        }
    }

    public class EvMiniCooper : BaseMiniCooper
    {
        public int ChargeCapacity { get; set; }
        public float KmPrKwh { get; set; }
        
        public void Print()
        {
            PrintBaseMiniCooper();
            Console.WriteLine($"Charge capacity: {ChargeCapacity}\n" +
                              $"Km. pr. kilowatt hour: {KmPrKwh}");
        }
    }

    public class FossilMiniCooper : BaseMiniCooper
    {
        public int TankCapacity { get; set; }
        public float KmPrLiter { get; set; }
        public int Gears { get; set; } // 0 If auto

        public void Print()
        {
            PrintBaseMiniCooper();
            Console.WriteLine($"Tank capacity: {TankCapacity}\n" +
                              $"Km. pr. liter: {KmPrLiter}\n" +
                              $"Gears: {Gears}");
        }
    }

    public class HybridMiniCooper : BaseMiniCooper
    {
        public string FuelType1 { get; set; } = string.Empty;
        public string FuelType2 { get; set; } = string.Empty;
        public float TankCapacity { get; set; }
        public float ChargeCapacity { get; set; }
        public float KmPrLiter { get; set; }
        public float KmPrKwh { get; set; }
        public int Gears { get; set; } // 0 If auto
        
        public void Print()
        {
            PrintBaseMiniCooper();
            Console.WriteLine($"Fuel type 1: {FuelType1}\n" +
                              $"Fuel type 2: {FuelType2}\n" +
                              $"Tank capacity: {TankCapacity}\n" +
                              $"Charge capacity: {ChargeCapacity}\n" +
                              $"Km. pr. liter: {KmPrLiter}\n" +
                              $"Km. pr. kilowatt hour: {KmPrKwh}\n" +
                              $"Gears: {Gears}");
        }
    }

    public class FullMiniCooper
    {
        private EvMiniCooper? EvCooper { get; set; }
        private FossilMiniCooper? FossilCooper { get; set; }
        private HybridMiniCooper? HybridCooper { get; set; }

        public EvMiniCooper? GetEvCooper()
        {
            return EvCooper;
        }
        
        public FossilMiniCooper? GetFossilCooper()
        {
            return FossilCooper;
        }
        
        public HybridMiniCooper? GetHybridCooper()
        {
            return HybridCooper;
        }

        public void Clear()
        {
            Console.WriteLine("Clearing cars...");
            EvCooper = null;
            FossilCooper = null;
            HybridCooper = null;
        }

        public void PrintEv()
        {
            if (EvCooper == null)
                Console.WriteLine("Electric Cooper is null!");
            else
                EvCooper?.Print();
        }
        
        public void PrintFossil()
        {
            if (FossilCooper == null)
                Console.WriteLine("Fossil Cooper is null!");
            else
                FossilCooper?.Print();
        }
        
        public void PrintHybrid()
        {
            if (HybridCooper == null)
                Console.WriteLine("Hybrid Cooper is null!");
            else
                HybridCooper?.Print();
        }

        public bool ThereCanOnlyBeOne()
        {
            if (EvCooper != null)
            {
                Console.WriteLine("There is already an Electric Cooper");
                return false;
            }
            
            if (FossilCooper != null)
            {
                Console.WriteLine("There is already a Fossil Cooper");
                return false;
            }
            
            if (HybridCooper != null)
            {
                Console.WriteLine("There is already a Hybrid Cooper");
                return false;
            }

            return true;
        }

        public void SetMiniCooper(EvMiniCooper evCooper)
        {
            if (ThereCanOnlyBeOne())
                EvCooper = evCooper;
            else
                Console.WriteLine("A car has already been assigned to this object.");
        }

        public void SetMiniCooper(FossilMiniCooper fossilCooper)
        {
            if (ThereCanOnlyBeOne())
                FossilCooper = fossilCooper;
            else
                Console.WriteLine("A car has already been assigned to this object.");
        }

        public void SetMiniCooper(HybridMiniCooper hybridCooper)
        {
            if (ThereCanOnlyBeOne())
                HybridCooper = hybridCooper;
            else
                Console.WriteLine("A car has already been assigned to this object.");
        }

        private bool HasMultipleCars()
        {
            int carCount = 0;
            if (EvCooper != null)
                carCount++;
            if (FossilCooper != null)
                carCount++;
            if (HybridCooper != null)
                carCount++;

            return carCount != 1;
        }
    }
}