using Avalonia.Controls;
using Inanna.Helpers;

namespace Pheidippides.Ui;

public sealed partial class AlarmsParametersView : UserControl
{
    public AlarmsParametersView()
    {
        InitializeComponent();
        Loaded += (_, _) => NameTextBox.FocusCaretIndex();
    }
}
