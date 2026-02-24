using System;

namespace Pheidippides.Models;

public readonly struct AlarmSchedulerItem
{
    public readonly DateTime DueDateTime;

    public AlarmSchedulerItem(DateTime dueDateTime)
    {
        DueDateTime = dueDateTime;
    }
}
