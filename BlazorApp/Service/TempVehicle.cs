namespace BlazorApp.Service;

public abstract class Vehicle
{
    public decimal Price { get; private set; }
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public double KmDriven { get; private set; }
    public string Color { get; private set; }
    public int WeightKg { get; private set; }
    public int HorsePower { get; private set; }
    public int Doors { get; private set; }
    
    public void SetPrice(decimal price) => Price = price;
    public void SetBrand(string brand) => Brand = brand;
    public void SetModel(string model) => Model = model;
    public void SetYear(int year) => Year = year;
    public void SetKmDriven(double km) => KmDriven = km;
    public void SetColor(string color) => Color = color;
    public void SetWeightKg(int kg) => WeightKg = kg;
    public void SetHorsePower(int horse) => HorsePower = horse;
    public void SetDoors(int doors) => Doors = doors;

    public void PrintBrand()
    {
        Console.WriteLine($"Brand: {Brand}");
    }
}

public class EvCar : Vehicle
{
    private double BatteryCapacity;
}

public class FossilCar : Vehicle
{
    private double TankCapacity;
}