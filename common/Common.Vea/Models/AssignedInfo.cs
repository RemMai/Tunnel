using System;
using System.Text.Json.Serialization;
using Common.Server.Interfaces;

namespace Common.Vea.Models;

public sealed class AssignedInfo
{
    public uint IP { get; set; }
    public bool OnLine { get; set; }
    public DateTime LastTime { get; set; } = DateTime.Now;

    [JsonIgnore] public IConnection Connection { get; set; }
    public string Name { get; set; }
}