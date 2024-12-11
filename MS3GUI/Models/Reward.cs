using System;
using System.Collections.Generic;

namespace MS3GUI.Models;

public partial class Reward
{
    public int RewardId { get; set; }

    public int? Rvalue { get; set; }

    public string? Rdescription { get; set; }

    public string? Rtype { get; set; }

    public virtual ICollection<QuestReward> QuestRewards { get; set; } = new List<QuestReward>();
}
