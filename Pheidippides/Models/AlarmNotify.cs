using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Services;

namespace Pheidippides.Models;

public sealed partial class AlarmNotify : ObservableObject, IStaticFactory<Guid, AlarmNotify>
{
    public AlarmNotify(Guid id)
    {
        Id = id;
        _name = string.Empty;
        _dueDateTime = DateTimeOffset.Now;
    }

    public Guid Id { get; }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private DateTimeOffset _dueDateTime;

    [ObservableProperty]
    private bool _isCompleted;

    public static AlarmNotify Create(Guid input)
    {
        return new(input);
    }
}
