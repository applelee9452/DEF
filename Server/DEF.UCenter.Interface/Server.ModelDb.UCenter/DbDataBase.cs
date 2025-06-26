namespace DEF.UCenter;

using System;

#if !DEF_CLIENT
[GenerateSerializer]
#endif
public abstract class DataBase
{
#if !DEF_CLIENT
    [Id(0)]
#endif
    public string Id { get; set; }

#if !DEF_CLIENT
    [Id(1)]
#endif
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

#if !DEF_CLIENT
    [Id(2)]
#endif
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
}