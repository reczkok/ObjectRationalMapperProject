// Testowa baza danych:

using ObjectRationalMapper.Attributes;

namespace ObjectRationalMapper.DataClass;

public enum EngineType
{
    Diesel,
    Petrol,
    Electric
}

public enum PublicTransportRange
{
    International,
    National,
    Regional,
    Local
}

[Tablename("vehicle", ChildClasses = new[] { typeof(GroundVehicle), typeof(Plane) })]
[Id("id")]
public class Vehicle
{
    [Field("name", typeof(string))] 	
    public string Name { get; set; }
    
    [Field("speed", typeof(double))] 	
    public double Speed { get; set; }
    
    [Field("capacity", typeof(int))] 	
    public int Capacity { get; set; }
}

[Tablename("groundVehicle", ParentClass = typeof(Vehicle), ChildClasses = new[] { typeof(Car), typeof(PublicTransport) })]
public class GroundVehicle : Vehicle // Pusta klasa - przypadek brzegowy
{} 

[Tablename("publicTransport", ParentClass = typeof(GroundVehicle), ChildClasses = new[] { typeof(Bus), typeof(Train) })]
public class PublicTransport : GroundVehicle
{
    [Field("fare", typeof(double))] 
    public double Fare { get; set; }
}

[Tablename("car", ParentClass = typeof(GroundVehicle))]
public class Car : GroundVehicle
{
    [Field("carEngineType", typeof(EngineType))] 
    public EngineType CarEngineType { get; set; }
}

[Tablename("plane", ParentClass = typeof(Vehicle))]
public class Plane : Vehicle
{   
    [Field("wingspan", typeof(double))]
    public double Wingspan { get; set; }
    
    [Field("maxRange", typeof(int))]
    public int MaxRange { get; set; }
}

[Tablename("bus", ParentClass = typeof(PublicTransport))]
public class Bus : PublicTransport
{
    [Field("busEngineType", typeof(EngineType))]
    public EngineType BusEngineType { get; set; }
    
    [Field("busTransportRange", typeof(PublicTransportRange))]
    public PublicTransportRange BusTransportRange { get; set; }
}

[Tablename("train", ParentClass = typeof(PublicTransport))]
public class Train : PublicTransport
{
    [Field("noOfCars", typeof(int))]
    public int NoOfCars { get; set; }
    
    [Field("trainTransportRange", typeof(PublicTransportRange))]
    public PublicTransportRange TrainTransportRange { get; set; }
}