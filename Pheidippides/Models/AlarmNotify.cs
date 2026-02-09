using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Pheidippides.Models;

public partial class AlarmNotify : ObservableObject
{
    public AlarmNotify(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset DueDateTime { get; set; }
}
