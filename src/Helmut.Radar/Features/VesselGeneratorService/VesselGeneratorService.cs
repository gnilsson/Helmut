using Helmut.General.Models;

namespace Helmut.Radar.Features.VesselGeneratorService;

public class VesselGeneratorService : IVesselGeneratorService
{
    internal sealed class Data
    {
        static Data()
        {
            _firstNames = _fullNames.Select(x => x.Split(' ')[0]).ToArray();
            _lastNames = _fullNames.Select(x => x.Split(' ')[1]).ToArray();
        }

        private static readonly string[] _fullNames = new string[]
        {
            "Tadg Fiachna",
            "Gwawl Béibhinn",
            "Dagda Mabon",
            "Llew Gwalchmai",
            "Maeve Conn",
            "Tadhg Arthur",
            "Diarmait Llŷr",
            "Eógan Gwawl",
            "Beli Macsen",
            "Bláthíne Lóegaire",
            "Llew Heilyn",
            "Cáel Pryderi",
            "Céibhfhionn Nechtan",
            "Cáel Gofannon",
            "Manannán Brigid",
            "Llew Gofannon",
            "Angharad Conor",
            "Modred Manawydan",
            "Fergus Nechtan",
            "Ailbe Conn",
            "Gwenddoleu Blodeuedd",
            "Fintan Sadbh",
            "Nessa Arianrhod",
            "Deirdre Muirgen",
            "Gwenhwyfar Eoghan",
            "Fedelmid Cormac",
            "Maponos Arawn",
            "Nodens Céibhfhionn",
            "Bláthnaid Brigit",
            "Ségdae Culhwch",
        };

        private static readonly string[] _groups = new string[]
        {
            "Spe",
            "Arion",
            "Orbitar",
            "Taphao Thong",
            "Taphao Kaew",
            "Dimidium",
            "Galileo",
            "Brahe",
            "Lipperhey",
            "Janssen",
            "Harriot",
            "Halla",
            "Amateru",
            "Finlay",
            "Pirx",
            "Hypatia",
            "AEgir",
        };

        private static readonly string[] _firstNames;
        private static readonly string[] _lastNames;

        internal static string[] FirstNames
        {
            get => _firstNames;
        }

        internal static string[] LastNames
        {
            get => _lastNames;
        }

        internal static string[] Groups
        {
            get => _groups;
        }
    }

    public IEnumerable<Vessel>? GenerateFreshVessels(int count)
    {
        if (count == 0) return null;

        return YieldVessel(count);
    }

    private static IEnumerable<Vessel> YieldVessel(int count)
    {
        var firstRandom = new Random();
        var lastRandom = new Random();

        var names = YieldRandomName(count, firstRandom, lastRandom).ToArray();
        var groups = YieldRandomGroup(count, firstRandom).ToArray();
        var coordinates = YieldRandomCoordinates(count, firstRandom, lastRandom).ToArray();

        for (int i = 0; i < count; i++)
        {
            yield return new Vessel
            {
                Id = Guid.NewGuid(),
                Affinity = new Affinity
                {
                    Name = names[i],
                    Group = groups[i],
                },
                Coordinates = coordinates[i]
            };
        }
    }

    private static IEnumerable<string> YieldRandomName(int count, Random firstRandom, Random lastRandom)
    {
        for (int i = 0; i < count; i++)
        {
            var firstIndex = firstRandom.Next(Data.FirstNames.Length - 1);
            var lastIndex = lastRandom.Next(Data.LastNames.Length - 1);

            yield return string.Join(" ", Data.FirstNames[firstIndex], Data.LastNames[lastIndex]);
        }
    }

    private static IEnumerable<string> YieldRandomGroup(int count, Random random)
    {
        for (int i = 0; i < count; i++)
        {
            var index = random.Next(Data.Groups.Length - 1);

            yield return Data.Groups[index];
        }
    }

    private static IEnumerable<Coordinates> YieldRandomCoordinates(int count, Random firstRandom, Random lastRandom)
    {
        for (int i = 0; i < count; i++)
        {
            var latitute = firstRandom.Next(584000, 598000);
            var longitude = lastRandom.Next(194000, 218000);

            yield return new Coordinates(latitute, longitude);
        }
    }
}
