using Helmut.General.Models;

namespace Helmut.Operations.Features.LocationTranscoder;

public interface ILocationTranscoderService
{
    ValueTask<LocationNameRepresentation> TranscodeCoordinatesAsync(Coordinates coordinates);
}
