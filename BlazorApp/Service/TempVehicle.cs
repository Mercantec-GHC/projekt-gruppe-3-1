namespace BlazorApp.Service;

public abstract class Vehicle
{
    public decimal Price { get; private set; }
    public string Brand { get; private set; }
    public string Model;
    private int Year;
    private double KmDriven;
    private string Color;
    private int WeightKg;
    private int HorsePower;
    private int Doors;

    public decimal PriceMeth
    {
        get { return Price; }
        set { Price = value; }
    }

    /*public string BrandMeth
    {
        get { return Brand; }
        set { Brand = value; }
    }*/

    public void SetBrand(string brand) => Brand = brand;

    public void PrintBrand()
    {
        Console.WriteLine($"Brand: {Brand}");
    }

    private void SetModel(string model)
    {
        Model = model;
    }

    private void SetYear(int year)
    {
        Year = year;
    }

    private void SetKmDriven(double km)
    {
        KmDriven = km;
    }

    private void SetColor(string color)
    {
        Color = color;
    }

    private void SetWeightKg(int kg)
    {
        WeightKg = kg;
    }

    private void SetHorsePower(int power)
    {
        HorsePower = power;
    }

    private void SetDoors(int doors)
    {
        Doors = doors;
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