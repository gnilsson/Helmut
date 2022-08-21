using MassTransit;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Boolkit;

public readonly struct SequentialIdentifier :
        IIdentifier,
        IComparable<SequentialIdentifier>,
        IComparable,
        IFormattable
{
    public static readonly SequentialIdentifier Empty = new(NewId.Empty);

    private readonly Identifier _identifier;
    public readonly NewId _newId;

    public SequentialIdentifier(in NewId newIdValue)
    {
        _identifier = new Identifier(newIdValue.ToSequentialGuid());
        _newId = newIdValue;
    }

    public SequentialIdentifier(in Guid guidValue)
    {
        _identifier = new Identifier(guidValue);
        _newId = guidValue.ToNewIdFromSequential();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SequentialIdentifier New() => new(NewId.Next());

    public readonly DateTime Timestamp => _newId.Timestamp;

    public static bool TryParse(in string? valueToParse, out SequentialIdentifier seqIdentifier)
    {
        if (Identifier.TryParse(valueToParse, out var identifier)
            && TryToConstruct(identifier, out seqIdentifier))
        {
            return true;
        }

        seqIdentifier = default;
        return false;
    }

    private static bool TryToConstruct(in Guid identifier, out SequentialIdentifier seqIdentifier)
    {
        try
        {
            var newId = identifier.ToNewIdFromSequential();
            _ = newId.Timestamp;
            seqIdentifier = new SequentialIdentifier(newId);
        }
        catch
        {
            seqIdentifier = default;
            return false;
        }

        return true;
    }

    public bool Equals(SequentialIdentifier other) => _identifier.Equals(other._identifier);
    public bool Equals(SequentialIdentifier? other) => _identifier.Equals(other?._identifier);
    public bool Equals(Identifier other) => _identifier.Equals(other);
    public bool Equals(Guid other) => _identifier.Equals(other);
    public string ToString(string? format, IFormatProvider? formatProvider) => _identifier.ToString(null, formatProvider);
    public int CompareTo(SequentialIdentifier other) => _newId.CompareTo(other._newId);
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;

        if (obj is not SequentialIdentifier) throw new ArgumentException("Argument must be a SequentialIdentifier");

        return CompareTo((SequentialIdentifier)obj);
    }
    public override bool Equals(object? obj) => Equals(obj);
    public override int GetHashCode() => base.GetHashCode();
    public override string ToString() => _identifier;

    public static implicit operator Identifier(SequentialIdentifier seqIdentifier) => seqIdentifier._identifier;
    public static implicit operator Guid(SequentialIdentifier seqIdentifier) => seqIdentifier._identifier;
    public static implicit operator string(SequentialIdentifier seqIdentifier) => seqIdentifier._identifier;

    public static bool operator <(SequentialIdentifier id1, SequentialIdentifier id2) => id1.CompareTo(id2) < 0;
    public static bool operator >(SequentialIdentifier id1, SequentialIdentifier id2) => id1.CompareTo(id2) > 0;

    public static bool operator ==(SequentialIdentifier id1, SequentialIdentifier id2) => id1.Equals(id2);
    public static bool operator !=(SequentialIdentifier id1, SequentialIdentifier id2) => !id1.Equals(id2);
    public static bool operator ==(Identifier id1, SequentialIdentifier id2) => id1.Equals(id2._identifier);
    public static bool operator !=(Identifier id1, SequentialIdentifier id2) => !id1.Equals(id2._identifier);
    public static bool operator ==(SequentialIdentifier id1, Identifier id2) => id1.Equals(id2);
    public static bool operator !=(SequentialIdentifier id1, Identifier id2) => !id1.Equals(id2);
    public static bool operator ==(Guid id1, SequentialIdentifier id2) => id1.Equals(id2._identifier);
    public static bool operator !=(Guid id1, SequentialIdentifier id2) => !id1.Equals(id2._identifier);
}


public readonly struct SequentialIdentifier2 :
     //   IEquatable<Guid>,
        IComparable<SequentialIdentifier2>,
        IComparable,
        IFormattable
{
    public static readonly SequentialIdentifier2 Empty = new(NewId.Empty);

    private const char EqualsChar = '=';
    private const char Hyphen = '-';
    private const char Underscore = '_';
    private const char Plus = '+';
    private const char Slash = '/';
    private const byte PlusByte = (byte)Plus;
    private const byte SlashByte = (byte)Slash;
    private const string UrlFriendlyBase64RegexPattern = "^(?=(.{22})$)[A-Za-z0-9_-]*([AQgw]==|[AEIMQUYcgkosw048]=)?$";

  //  private readonly Guid _guidValue;
  //  private readonly string _base64Value;
    private readonly NewId _newId;

    public SequentialIdentifier2(in NewId newIdValue)
    {
        //_guidValue = newIdValue.ToSequentialGuid();
        // _base64Value = ToIdentifierString(_guidValue);
       // _base64Value = ToIdentifierString(newIdValue.ToSequentialGuid());
        _newId = newIdValue;
    }

    public SequentialIdentifier2(in Guid guidValue)
    {
   //     _guidValue = guidValue;
   //     _base64Value = ToIdentifierString(guidValue);
        _newId = guidValue.ToNewIdFromSequential();
    }

    //public SequentialIdentifier2(in string base64Value)
    //{
    //    if (Guid.TryParse(base64Value, out _guidValue))
    //    {
    //        _base64Value = ToIdentifierString(_guidValue);
    //    }
    //    else
    //    {
    //        _guidValue = ToIdentifierGuid(base64Value);
    //        _base64Value = base64Value;
    //    }

    //    _newId = _guidValue.ToNewIdFromSequential();
    //}


    //public SequentialIdentifier2(in Guid guidValue, in string base64Value)
    //{
    //    _guidValue = guidValue;
    //    _base64Value = base64Value;
    //    _newId = _guidValue.ToNewIdFromSequential();
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SequentialIdentifier2 New() => new(Guid.NewGuid());

    public static bool TryParse(in string? valueToParse, out SequentialIdentifier2 identifier)
    {
        if (valueToParse is not null
            && Regex.IsMatch(valueToParse, UrlFriendlyBase64RegexPattern, RegexOptions.Compiled)
            && TryToConstruct(valueToParse, out identifier))
        {
            return true;
        }

        identifier = default;
        return false;
    }

    private static bool TryToConstruct(in string? value, out SequentialIdentifier2 seqIdentifier)
    {
        try
        {
            var identifier = ToIdentifierGuid(value);
            var newId = identifier.ToNewIdFromSequential();
            _ = newId.Timestamp;
            seqIdentifier = new SequentialIdentifier2(newId);
        }
        catch
        {
            seqIdentifier = default;
            return false;
        }

        return true;
    }

    private static bool TryToIdentifierGuid(in string value, out SequentialIdentifier2 identifier)
    {
        try
        {
            identifier = ToIdentifierGuid(value);
        }
        catch
        {
            identifier = default;
            return false;
        }

        return true;
    }

    private static string ToIdentifierString(Guid id)
    {
        Span<byte> idBytes = stackalloc byte[16];
        Span<byte> base64Bytes = stackalloc byte[24];

        MemoryMarshal.TryWrite(idBytes, ref id);
        Base64.EncodeToUtf8(idBytes, base64Bytes, out _, out _);

        Span<char> finalChars = stackalloc char[22];

        for (var i = 0; i < 22; i++)
        {
            finalChars[i] = base64Bytes[i] switch
            {
                SlashByte => Hyphen,
                PlusByte => Underscore,
                _ => (char)base64Bytes[i],
            };
        }

        return new string(finalChars);
    }

    private static Guid ToIdentifierGuid(in ReadOnlySpan<char> id)
    {
        Span<char> base64Chars = stackalloc char[24];

        for (var i = 0; i < 22; i++)
        {
            base64Chars[i] = id[i] switch
            {
                Hyphen => Slash,
                Underscore => Plus,
                _ => id[i]
            };
        }

        base64Chars[22] = EqualsChar;
        base64Chars[23] = EqualsChar;

        Span<byte> idBytes = stackalloc byte[16];

        Convert.TryFromBase64Chars(base64Chars, idBytes, out _);

        return new Guid(idBytes);
    }

    public int CompareTo(SequentialIdentifier2 other) => _newId.CompareTo(other._newId);
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;

        if (obj is not SequentialIdentifier2) throw new ArgumentException("Argument must be a SequentialIdentifier");

        return CompareTo((SequentialIdentifier2)obj);
    }

    public bool Equals(SequentialIdentifier2? other) => _newId == other?._newId;
    public bool Equals(SequentialIdentifier2 other) => _newId == other._newId;
    public override bool Equals(object? obj) => base.Equals(obj);
    public override int GetHashCode() => base.GetHashCode();
    public override string ToString() => ToIdentifierString(_newId.ToSequentialGuid());
    public string ToString(string? format, IFormatProvider? formatProvider) => ToIdentifierString(_newId.ToSequentialGuid()).ToString(formatProvider);
 //   public bool Equals(Guid other) => _guidValue.Equals(other);

    public static implicit operator SequentialIdentifier2(Guid guidValue) => new(guidValue);
    public static implicit operator string(SequentialIdentifier2 identifier) => ToIdentifierString(identifier._newId.ToSequentialGuid());
    public static implicit operator Guid(SequentialIdentifier2 identifier) => identifier._newId.ToSequentialGuid();

    public static bool operator ==(SequentialIdentifier2 id1, SequentialIdentifier2 id2) => id1.Equals(id2);
    public static bool operator !=(SequentialIdentifier2 id1, SequentialIdentifier2 id2) => !id1.Equals(id2);
    public static bool operator ==(SequentialIdentifier2 id1, Guid id2) => id1._newId.ToSequentialGuid().Equals(id2);
    public static bool operator !=(SequentialIdentifier2 id1, Guid id2) => !id1._newId.ToSequentialGuid().Equals(id2);
}
