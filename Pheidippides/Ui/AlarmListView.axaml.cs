using System;
using Avalonia.Controls;

namespace Pheidippides.Ui;

public sealed partial class AlarmListView : UserControl
{
    public AlarmListView()
    {
        InitializeComponent();
    }

    public AlarmListViewModel ViewModel =>
        DataContext as AlarmListViewModel ?? throw new InvalidOperationException();
}
