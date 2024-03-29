﻿namespace Helmut.General.Models;

public record Vessel
{
    public Guid Id { get; init; }
    public Affinity Affinity { get; init; } = default!;
    public Coordinates Coordinates { get; init; }
}
