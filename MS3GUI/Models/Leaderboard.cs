﻿using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class Leaderboard
{
    public int BoardId { get; set; }

    public string? Season { get; set; }

    public virtual ICollection<Ranking> Rankings { get; set; } = new List<Ranking>();
}
