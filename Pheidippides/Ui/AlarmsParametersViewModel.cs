using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Generator;
using Inanna.Models;
using Inanna.Ui;
using Pheidippides.Models;
using Rooster.Contract.Models;

namespace Pheidippides.Ui;

[EditNotify]
public sealed partial class AlarmsParametersViewModel : ParametersViewModelBase
{
    public AlarmsParametersViewModel(
        AlarmNotify item,
        ValidationMode validationMode,
        bool isShowEdit
    )
        : base(validationMode, isShowEdit)
    {
        DueDate = item.DueDateTime.LocalDateTime;
        DueTime = item.DueDateTime.TimeOfDay;
        Name = item.Name;
        ResetEdit();
    }

    public AlarmsParametersViewModel(ValidationMode validationMode, bool isShowEdit)
        : base(validationMode, isShowEdit)
    {
        Name = string.Empty;
        DueDate = DateTime.Now;
        DueTime = DateTime.Now.TimeOfDay;
        ResetEdit();
    }

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial bool IsEditName { get; set; }

    [ObservableProperty]
    public partial DateTime DueDate { get; set; }

    [ObservableProperty]
    public partial bool IsEditDueDate { get; set; }

    [ObservableProperty]
    public partial TimeSpan DueTime { get; set; }

    [ObservableProperty]
    public partial bool IsEditDueTime { get; set; }

    public Alarm CreateAlarm(Guid id)
    {
        return new()
        {
            DueDateTime = new DateTimeOffset(DueDate) + DueTime,
            Name = Name,
            Id = id,
        };
    }

    public EditAlarm CreateEdit(Guid id)
    {
        return CreateEdit([id]);
    }

    public EditAlarm CreateEdit(Guid[] ids)
    {
        return new()
        {
            DueDateTime = new DateTimeOffset(DueDate) + DueTime,
            Name = Name,
            IsEditName = IsEditName,
            Ids = ids,
            IsEditDueDateTime = IsEditDueTime || IsEditDueDate,
        };
    }
}
