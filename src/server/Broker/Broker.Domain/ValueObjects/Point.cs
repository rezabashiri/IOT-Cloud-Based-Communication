namespace Broker.Domain.ValueObjects;

public record struct Point(double Latitude, double Longitude)
{
    public static Point Empty => new(0, 0);

    public static Point New(double latitude, double longitude)
    {
        return new Point(latitude, longitude);
    }
}