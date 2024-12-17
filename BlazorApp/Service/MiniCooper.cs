using System.ComponentModel.DataAnnotations.Schema;
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
        public string FuelType { get; set; } = string.Empty; // Benzin/Diesel/Electricity/Hybrid.
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

        public BaseMiniCooper GetBase()
        {
            return this;
        }

        public void Clear()
        {
            ModelName = string.Empty;
            Generation = 0;
            ModelType = string.Empty;
            Color = string.Empty;
            Price = 0;
            Mileage = 0;
            MaxRange = 0;
            Weight = 0;
            FuelType = string.Empty;
            GearType = string.Empty;
            YearlyTax = 0;
            Base64Images.Clear();
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
        
        public int GetChargeCapacity()
        {
            return ChargeCapacity;
        }

        public float GetKmPrKwh()
        {
            return KmPrKwh;
        }

        public void Clear()
        {
            base.Clear();
            ChargeCapacity = 0;
            KmPrKwh = 0;
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
        
        public int GetTankCapacity()
        {
            return TankCapacity;
        }

        public float GetKmPrLiter()
        {
            return KmPrLiter;
        }

        public int GetGears()
        {
            return Gears;
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

        public string GetFuelType1()
        {
            return FuelType1;
        }
        
        public string GetFuelType2()
        {
            return FuelType2;
        }

        public float GetTankCapacity()
        {
            return TankCapacity;
        }

        public float GetChargeCapacity()
        {
            return ChargeCapacity;
        }

        public float GetKmPrLiter()
        {
            return KmPrLiter;
        }

        public float GetKmPrKwh()
        {
            return KmPrKwh;
        }
        
        public int GetGears()
        {
            return Gears;
        }
    }

    public class FullMiniCooper
    {
        private int CarId { get; set; }
        private int UserId { get; set; }
        private EvMiniCooper? EvCooper { get; set; }
        private FossilMiniCooper? FossilCooper { get; set; }
        private HybridMiniCooper? HybridCooper { get; set; }

        public void SetIds(int carId, int userId)
        {
            CarId = carId;
            UserId = userId;
        }

        public string GetCooperTypeInUse()
        {
            if (EvCooper != null)
                return "ev";
            else if (FossilCooper != null)
                return "fossil";
            else if (HybridCooper != null)
                return "hybrid";
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get cooper type in use.");
                return string.Empty;
            }
        }

        public List<string> GetImages()
        {
            if (EvCooper != null)
                return EvCooper.Base64Images;
            else if (FossilCooper != null)
                return FossilCooper.Base64Images;
            else if (HybridCooper != null)
                return HybridCooper.Base64Images;
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get images.");
                return new List<string>();
            }
        }

        public string GetModelType()
        {
            if (EvCooper != null)
                return EvCooper.ModelType;
            else if (FossilCooper != null)
                return FossilCooper.ModelType;
            else if (HybridCooper != null)
                return HybridCooper.ModelType;
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get model type.");
                return string.Empty;
            }
        }

        public string GetFuelType()
        {
            if (EvCooper != null)
                return EvCooper.FuelType;
            else if (FossilCooper != null)
                return FossilCooper.FuelType;
            else if (HybridCooper != null)
                return HybridCooper.FuelType;
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Can get fuel type.");
                return String.Empty;
            }
        }

        public string GetGearType()
        {
            if (EvCooper != null)
                return EvCooper.GearType;
            else if (FossilCooper != null)
                return FossilCooper.GearType;
            else if (HybridCooper != null)
                return HybridCooper.GearType;
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get gear type.");
                return string.Empty;
            }
        }

        public int GetMileage()
        {
            if (EvCooper != null)
                return EvCooper.Mileage;
            else if (FossilCooper != null)
                return FossilCooper.Mileage;
            else if (HybridCooper != null)
                return HybridCooper.Mileage;
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get mileage.");
                return -1;
            }
        }
        
        public int GetMaxRange()
        {
            if (EvCooper != null)
                return EvCooper.MaxRange;
            else if (FossilCooper != null)
                return FossilCooper.MaxRange;
            else if (HybridCooper != null)
                return HybridCooper.MaxRange;
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get max range.");
                return -1;
            }
        }
        
        public int GetWeight()
        {
            if (EvCooper != null)
                return EvCooper.Weight;
            else if (FossilCooper != null)
                return FossilCooper.Weight;
            else if (HybridCooper != null)
                return HybridCooper.Weight;
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get weight.");
                return -1;
            }
        }

        public decimal GetYearlyTax()
        {
            if (EvCooper != null)
                return EvCooper.YearlyTax;
            else if (FossilCooper != null)
                return FossilCooper.YearlyTax;
            else if (HybridCooper != null)
                return HybridCooper.YearlyTax;
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get yearly tax.");
                return -1;
            }
        }

        public int GetCarId()
        {
            return CarId;
        }

        public int GetUserId()
        {
            return UserId;
        }

        public string GetImageByIndex(int index)
        {
            if (EvCooper != null)
                return EvCooper.Base64Images[index];
            else if (FossilCooper != null)
                return FossilCooper.Base64Images[index];
            else if (HybridCooper != null)
                return HybridCooper.Base64Images[index];
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Can get image by index");
                return string.Empty;
            }
        }

        public string GetFirstImage()
        {
            if (EvCooper != null)
                return EvCooper.Base64Images[0];
            else if (FossilCooper != null)
                return FossilCooper.Base64Images[0];
            else if (HybridCooper != null)
                return HybridCooper.Base64Images[0];
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get first image.");
                return string.Empty;
            }
        }

        public string GetModelName()
        {
            if (EvCooper != null)
                return EvCooper.ModelName;
            else if (FossilCooper != null)
                return FossilCooper.ModelName;
            else if (HybridCooper != null)
                return HybridCooper.ModelName;
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get generation. Cant get model name.");
                return string.Empty;
            }
        }

        public decimal GetPrice()
        {
            if (EvCooper != null)
                return EvCooper.Price;
            else if (FossilCooper != null)
                return FossilCooper.Price;
            else if (HybridCooper != null)
                return HybridCooper.Price;
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get price.");
                return 0;
            }
        }
        
        public string GetColor()
        {
            if (EvCooper != null)
                return EvCooper.Color;
            else if (FossilCooper != null)
                return FossilCooper.Color;
            else if (HybridCooper != null)
                return HybridCooper.Color;
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get price.");
                return "";
            }
        }

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

        public string GetGeneration()
        {
            if (EvCooper != null)
                return EvCooper.Generation.ToString();
            else if (FossilCooper != null)
                return FossilCooper.Generation.ToString();
            else if (HybridCooper != null)
                return HybridCooper.Generation.ToString();
            else
            {
                Console.WriteLine("No car name has been assigned to this object. Cant get generation.");
                return string.Empty;
            }
        }

        public void SetMiniCooper(EvMiniCooper evCooper)
        {
            if (ThereCanOnlyBeOne())
                EvCooper = evCooper;
            else
                Console.WriteLine("A car has already been assigned to this object. Cant set electric cooper.");
        }

        public void SetMiniCooper(FossilMiniCooper fossilCooper)
        {
            if (ThereCanOnlyBeOne())
                FossilCooper = fossilCooper;
            else
                Console.WriteLine("A car has already been assigned to this object. Cant set fossil cooper.");
        }

        public void SetMiniCooper(HybridMiniCooper hybridCooper)
        {
            if (ThereCanOnlyBeOne())
                HybridCooper = hybridCooper;
            else
                Console.WriteLine("A car has already been assigned to this object. Cant set hybrid cooper.");
        }

        public BaseMiniCooper? GetBaseCooper()
        {
            if (EvCooper != null)
            {
                return EvCooper.GetBase();
            }
            else if (FossilCooper != null)
            {
                return FossilCooper.GetBase();
            }
            else if (HybridCooper != null)
            {
                return HybridCooper.GetBase();
            }
            else
            {
                Console.WriteLine("No car has been assigned to this object. Cant get base cooper.");
                return null;
            }
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

        public void PrintAutomatically()
        {
            Console.WriteLine("Printing automatically...");
            Console.WriteLine("Car ID: "+ CarId);
            Console.WriteLine("User ID: "+UserId);
            if (EvCooper != null)
                EvCooper.Print();
            else if (FossilCooper != null)
                FossilCooper.Print();
            else if (HybridCooper != null)
                HybridCooper.Print();
            else
                Console.WriteLine("No car has been assigned to this object. Cant automatically print.");
        }

        public bool HasMultipleCars()
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

        public List<FullMiniCooper> SortByEv(List<FullMiniCooper> fullMiniCoopers)
        {
            List<FullMiniCooper> sortedFullMiniCoopers = new();
            foreach (var fullCooper in fullMiniCoopers)
            {
                if (fullCooper.GetEvCooper() != null)
                    sortedFullMiniCoopers.Add(fullCooper);
            }

            return sortedFullMiniCoopers;
        }

        public List<FullMiniCooper> SortByBenzin(List<FullMiniCooper> fullMiniCoopers)
        {
            List<FullMiniCooper> sortedFullMiniCoopers = new();
            foreach (var fullCooper in fullMiniCoopers)
            {
                if (fullCooper.GetFossilCooper() != null && (fullCooper.GetFossilCooper().FuelType == "Benzin" ||
                                                             fullCooper.GetFossilCooper().FuelType == "Petrol"))
                    sortedFullMiniCoopers.Add(fullCooper);
            }

            return sortedFullMiniCoopers;
        }

        public List<FullMiniCooper> SortByDiesel(List<FullMiniCooper> fullMiniCoopers)
        {
            List<FullMiniCooper> sortedFullMiniCoopers = new();
            foreach (var fullCooper in fullMiniCoopers)
            {
                if (fullCooper.GetFossilCooper() != null && fullCooper.GetFossilCooper().FuelType == "Diesel")
                    sortedFullMiniCoopers.Add(fullCooper);
            }

            return sortedFullMiniCoopers;
        }

        public List<FullMiniCooper> SortByFossil(List<FullMiniCooper> fullMiniCoopers)
        {
            List<FullMiniCooper> sortedFullMiniCoopers = new();
            foreach (var fullCooper in fullMiniCoopers)
            {
                if (fullCooper.GetFossilCooper() != null)
                    sortedFullMiniCoopers.Add(fullCooper);
            }

            return sortedFullMiniCoopers;
        }

        public List<FullMiniCooper> SortByHybrid(List<FullMiniCooper> fullMiniCoopers)
        {
            List<FullMiniCooper> sortedFullMiniCoopers = new();
            foreach (var fullCooper in fullMiniCoopers)
            {
                if (fullCooper.GetHybridCooper() != null)
                    sortedFullMiniCoopers.Add(fullCooper);
            }

            return sortedFullMiniCoopers;
        }

        public void Clear()
        {
            Console.WriteLine("Clearing cars...");
            EvCooper = null;
            FossilCooper = null;
            HybridCooper = null;
        }
    }

    public class FullMiniCoopersState
    {
        public List<FullMiniCooper> FullMiniCoopers { get; set; } = new();
    }
}