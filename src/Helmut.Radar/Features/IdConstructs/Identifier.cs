﻿using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Helmut.Radar.Features.IdConstructs;

public readonly struct Identifier :
        IEquatable<Identifier>,
        IEquatable<Guid>,
        IFormattable
{
    public static readonly Identifier Empty = new(Guid.Empty);

    private const char EqualsChar = '=';
    private const char Hyphen = '-';
    private const char Underscore = '_';
    private const char Plus = '+';
    private const char Slash = '/';
    private const byte PlusByte = (byte)Plus;
    private const byte SlashByte = (byte)Slash;
    private const string UrlFriendlyBase64RegexPattern = "^(?=(.{22})$)[A-Za-z0-9_-]*([AQgw]==|[AEIMQUYcgkosw048]=)?$";

    private readonly Guid _guidValue;
    private readonly string _base64Value;

    public Identifier(in Guid guidValue)
    {
        _guidValue = guidValue;
        _base64Value = ToIdentifierString(guidValue);
    }

    public Identifier(in string base64Value)
    {
        if (Guid.TryParse(base64Value, out _guidValue))
        {
            _base64Value = ToIdentifierString(_guidValue);
            return;
        }

        _guidValue = ToIdentifierGuid(base64Value);
        _base64Value = base64Value;
    }

    public Identifier(in Guid guidValue, in string base64Value)
    {
        _guidValue = guidValue;
        _base64Value = base64Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Identifier New() => new(Guid.NewGuid());

    public static bool TryParse(in string? valueToParse, out Identifier identifier)
    {
        if (valueToParse is not null
            && Regex.IsMatch(valueToParse, UrlFriendlyBase64RegexPattern, RegexOptions.Compiled)
            && TryToConstruct(valueToParse!, out identifier))
        {
            return true;
        }

        identifier = default;
        return false;
    }

    private static bool TryToConstruct(in string value, out Identifier identifier)
    {
        try
        {
            identifier = new Identifier(value);
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

    public bool Equals(Identifier? other) => _guidValue == other?._guidValue;
    public bool Equals(Identifier other) => _guidValue == other._guidValue;
    public override bool Equals(object? obj) => base.Equals(obj);
    public override int GetHashCode() => base.GetHashCode();
    public override string ToString() => _base64Value;
    public string ToString(string? format, IFormatProvider? formatProvider) => _base64Value.ToString(formatProvider);
    public bool Equals(Guid other) => _guidValue.Equals(other);

    public static implicit operator Identifier(Guid guidValue) => new(guidValue);
    public static implicit operator string(Identifier identifier) => identifier._base64Value;
    public static implicit operator Guid(Identifier identifier) => identifier._guidValue;

    public static bool operator ==(Identifier id1, Identifier id2) => id1.Equals(id2);
    public static bool operator !=(Identifier id1, Identifier id2) => !id1.Equals(id2);
    public static bool operator ==(Identifier id1, Guid id2) => id1._guidValue.Equals(id2);
    public static bool operator !=(Identifier id1, Guid id2) => !id1._guidValue.Equals(id2);
}
