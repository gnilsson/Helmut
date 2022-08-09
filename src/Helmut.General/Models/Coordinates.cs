namespace Helmut.General.Models;

public readonly struct Coordinates : IEquatable<Coordinates>
{
    public static Coordinates Empty => new(0, 0);

    private readonly double _latitude;
    private readonly double _longitude;

    public Coordinates(double latitude, double longitude)
    {
        _latitude = latitude;
        _longitude = longitude;
    }

    public double Latitude
    {
        get => _latitude;
    }

    public double Longitude
    {
        get => _longitude;
    }

    public override string ToString() => $"{_latitude}, {_longitude}";

    public static bool TryParse(in string @string, out Coordinates coordinates)
    {
        var parts = @string.Split(',');

        if (parts.Length == 2 && double.TryParse(parts[0], out var latitude) && double.TryParse(parts[1], out var longitude))
        {
            coordinates = new Coordinates(latitude, longitude);
            return true;
        }

        coordinates = Empty;
        return false;
    }

    public override bool Equals(object? other)
    {
        return other is Coordinates coordinates && Equals(coordinates);
    }

    public bool Equals(Coordinates other)
    {
        return Latitude == other.Latitude && Longitude == other.Longitude;
    }

    public override int GetHashCode()
    {
        return Latitude.GetHashCode() ^ Longitude.GetHashCode();
    }

    public static bool operator ==(Coordinates left, Coordinates right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Coordinates left, Coordinates right)
    {
        return !(left == right);
    }
}
