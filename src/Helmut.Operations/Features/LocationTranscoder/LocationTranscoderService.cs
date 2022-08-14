using Helmut.General.Models;
using what3words.dotnet.wrapper;
using W3wCoordinates = what3words.dotnet.wrapper.models.Coordinates;

namespace Helmut.Operations.Features.LocationTranscoder;

internal sealed class LocationTranscoderService : ILocationTranscoderService
{
    private readonly What3WordsV3 _wrapper;

    public LocationTranscoderService(IConfiguration configuration)
    {
        _wrapper = new What3WordsV3(configuration["W3wKey"]);
    }

    public async ValueTask<LocationNameRepresentation> TranscodeCoordinatesAsync(Coordinates coordinates)
    {
        if (coordinates == Coordinates.Empty)
        {
            return LocationNameRepresentation.Empty;
        }

        var request = _wrapper.ConvertTo3WA(new W3wCoordinates(coordinates.Latitude, coordinates.Longitude));

        var response = await request.RequestAsync();

        if (response.IsSuccessful && LocationNameRepresentation.TryConvert(response.Data.Words, out var w3w))
        {
            return w3w;
        }

        return LocationNameRepresentation.Empty;
    }
}
