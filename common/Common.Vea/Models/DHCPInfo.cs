using System;
using System.Collections.Generic;

namespace Common.Vea.Models;

public sealed class DHCPInfo
{
    public uint IP { get; set; }
    public Dictionary<ulong, AssignedInfo> Assigned { get; set; } = new Dictionary<ulong, AssignedInfo>();
    public ulong[] Used { get; set; } = new ulong[4] { 0, 0, 0, 0 };

    public void Parse()
    {
        Used[0] = 0;
        Used[1] = 0;
        Used[2] = 0;
        Used[3] = 0;
        Add(1);
        Add(254);
        Add(255);
        foreach (var item in Assigned.Values)
        {
            Add((byte)(item.IP & 0xff));
        }
    }

    public void Add(byte value)
    {
        int arrayIndex = value / 64;
        int length = value - arrayIndex * 64;
        Used[arrayIndex] |= (ulong)1 << (length - 1);
    }

    public bool Exists(byte value)
    {
        int arrayIndex = value / 64;
        int length = value - arrayIndex * 64;

        return (Used[arrayIndex] >> (length - 1) & 0b1) == 1;
    }

    public void Delete(byte value)
    {
        int arrayIndex = value / 64;
        int length = value - arrayIndex * 64;
        Used[arrayIndex] &= ~((ulong)1 << (length - 1));
    }

    public bool Find(out byte value)
    {
        value = 0;
        if (Used.Length != 4) throw new Exception("array length must be 4");

        if (Used[0] < ulong.MaxValue) value = Find(Used[0], 0);
        else if (Used[1] < ulong.MaxValue) value = Find(Used[1], 1);
        else if (Used[2] < ulong.MaxValue) value = Find(Used[2], 2);
        else if (Used[3] < ulong.MaxValue) value = Find(Used[3], 3);
        return value > 0;
    }

    public byte Find(ulong group, byte index)
    {
        byte value = (byte)(index * 64);
        //每次对半开，也可以循环，循环稍微会慢3-4ns，常量值快一点
        ulong _group = (group & uint.MaxValue);
        if (_group >= uint.MaxValue)
        {
            _group = group >> 32;
            value += 32;
        }

        group = _group;

        _group = (group & ushort.MaxValue);
        if (_group >= ushort.MaxValue)
        {
            _group = group >> 16;
            value += 16;
        }

        group = _group;

        _group = (group & byte.MaxValue);
        if (_group >= byte.MaxValue)
        {
            _group = group >> 8;
            value += 8;
        }

        group = _group;

        _group = (group & 0b1111);
        if (_group >= 0b1111)
        {
            _group = group >> 4;
            value += 4;
        }

        group = _group;

        _group = (group & 0b11);
        if (_group >= 0b11)
        {
            _group = group >> 2;
            value += 2;
        }

        group = _group;

        _group = (group & 0b1);
        if (_group >= 0b1)
        {
            value += 1;
        }

        value += 1;

        return value;
    }
}