﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCHLPositions
{
    [Flags]
    public enum StatsType
    {
        GP = 1,
        P = 2,
        PIM = 4,
        SH = 8,
        Hits = 16,
        TkA = 32,
        BkS = 64,
        TOI = 128
    }

    public enum PlayerPosition
    {
        G,
        D,
        C,
        R,
        L
    }

    public enum NCHLTeam
    {
        AGL,
        SUN,
        REB,
        PAC,
        RAM,
        BUC
    }

    public enum NHLTeam
    {
        Ana,
        Ari,
        Bos,
        Buf,
        Cgy,
        Car,
        Chi,
        Col,
        CBJ,
        Dal,
        Det,
        Edm,
        Fla,
        LA,
        Min,
        Mtl,
        Nas,
        NJ,
        NYI,
        NYR,
        Ott,
        Phi,
        Pit,
        SJ,
        StL,
        TB,
        Tor,
        Van,
        VGK,
        Wsh,
        Wpg
    }
}
