using MassTransit;
using System.Buffers.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Helmut.Radar.Features.IdConstructs;

internal static class NewIdConverter
{
    private const char EQUALS_CHAR = '=';
    private const char HYPHEN = '-';
    private const char UNDERSCORE = '_';
    private const char PLUS = '+';
    private const char SLASH = '/';
    private const byte PLUS_BYTE = (byte)PLUS;
    private const byte SLASH_BYTE = (byte)SLASH;
    private const string URL_FRIENDLY_BASE64_REGEX_PATTERN = "^(?=(.{22})$)[A-Za-z0-9_-]*([AQgw]==|[AEIMQUYcgkosw048]=)?$";

    public static string ToStringBase64(this NewId newId)
    {
        return ToIdentifierString(newId.ToSequentialGuid());
    }

    public static bool TryParseFromBase64(in string? base64, out NewId newId)
    {
        try
        {
            if (base64 is not null && Regex.IsMatch(base64, URL_FRIENDLY_BASE64_REGEX_PATTERN, RegexOptions.Compiled))
            {
                newId = ToIdentifierGuid(base64).ToNewIdFromSequential();
                return true;
            }

            newId = NewId.Empty;
            return false;
        }
        catch (Exception)
        {
            newId = NewId.Empty;
            return false;
        }
    }

    public static bool TryParseFromGuid(Guid guid, out NewId newId)
    {
        try
        {
            newId = guid.ToNewIdFromSequential();
            _ = newId.Timestamp;
            return true;
        }
        catch (Exception)
        {
            newId = NewId.Empty;
            return false;
        }
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
                SLASH_BYTE => HYPHEN,
                PLUS_BYTE => UNDERSCORE,
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
                HYPHEN => SLASH,
                UNDERSCORE => PLUS,
                _ => id[i]
            };
        }

        base64Chars[22] = EQUALS_CHAR;
        base64Chars[23] = EQUALS_CHAR;

        Span<byte> idBytes = stackalloc byte[16];

        Convert.TryFromBase64Chars(base64Chars, idBytes, out _);

        return new Guid(idBytes);
    }
}
