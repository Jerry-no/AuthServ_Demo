using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuthService.Common.Helpers;

public sealed class UpperCaseEnumConverter<TEnum>
    : ValueConverter<TEnum, string>
    where TEnum : struct, Enum
{
    public UpperCaseEnumConverter()
        : base(
            v => v.ToString().ToUpperInvariant(),
            v => Enum.Parse<TEnum>(v, true))
    {
    }
}