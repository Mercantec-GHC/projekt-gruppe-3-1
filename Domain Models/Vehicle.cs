namespace Domain_Models;

public abstract class Vehicle
{
    private decimal Price;
    private string Brand;
    private string Model;
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

    private void SetBrand(string brand)
    {
        Brand = brand;
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