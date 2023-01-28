using Helmut.Radar.Features.VesselGeneratorService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Helmut.Radar.Features;

internal sealed class SqlServerConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        throw new NotImplementedException();
    }
}