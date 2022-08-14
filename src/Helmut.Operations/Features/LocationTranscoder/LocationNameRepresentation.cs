using System.Collections.Immutable;

namespace Helmut.Operations.Features.LocationTranscoder;

public struct LocationNameRepresentation : IEquatable<LocationNameRepresentation>, IFormattable
{
    public static readonly LocationNameRepresentation Empty = new(string.Empty, ImmutableArray<string>.Empty);
    private readonly string _nameString;
    private readonly ImmutableArray<string> _names;

    private LocationNameRepresentation(string nameString, ImmutableArray<string> names)
    {
        _nameString = nameString;
        _names = names;
    }

    public static bool TryConvert(string request, out LocationNameRepresentation words)
    {
        if (string.IsNullOrEmpty(request))
        {
            words = Empty;
            return false;
        }

        var split = request.Split('.').ToImmutableArray();

        if (split.Length != 3)
        {
            words = Empty;
            return false;
        }

        words = new(request, split);
        return true;
    }

    public bool IsEmpty
    {
        get => _names.IsEmpty;
    }

    public bool Equals(LocationNameRepresentation other)
    {
        return _nameString == other._nameString;
    }

    public override bool Equals(object? obj)
    {
        return obj is LocationNameRepresentation w && Equals(w);
    }

    public static bool operator ==(LocationNameRepresentation left, LocationNameRepresentation right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LocationNameRepresentation left, LocationNameRepresentation right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return _nameString.ToString(formatProvider);
    }

    public override string ToString()
    {
        return _nameString;
    }
}
