using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Pheidippides.Models;

namespace Pheidippides.Services;

public sealed class PheidippidesCommands
{
    public PheidippidesCommands(
        IDialogService dialogService,
        IAppResourceService appResourceService,
        IStringFormater stringFormater,
        IPheidippidesViewModelFactory viewModelFactory,
        IAlarmUiService alarmUiService,
        ICommandFactory commandFactory,
        ISafeExecuteWrapper safeExecuteWrapper
    )
    {
        ShowEditAlarmCommand = commandFactory.CreateCommand<AlarmNotify>(
            (item, ct) =>
            {
                var parameters = viewModelFactory.CreateAlarmsParameters(item);

                return dialogService.ShowMessageBoxAsync(
                    stringFormater
                        .Format(
                            appResourceService.GetResource<string>("Lang.EditItem"),
                            appResourceService.GetResource<string>("Lang.Alarm")
                        )
                        .DispatchToDialogHeader(),
                    parameters,
                    ct,
                    new(
                        appResourceService.GetResource<string>("Lang.Edit"),
                        commandFactory.CreateCommand(async c =>
                        {
                            var edit = parameters.CreateEdit(item.Id);
                            await dialogService.CloseMessageBoxAsync(c);

                            return await alarmUiService.PostAsync(
                                Guid.NewGuid(),
                                new() { Edits = [edit] },
                                c
                            );
                        }),
                        null,
                        DialogButtonType.Primary
                    ),
                    dialogService.CancelButton
                );
            }
        );

        ShowCreateAlarmCommand = commandFactory.CreateCommand(ct =>
        {
            var parameters = viewModelFactory.CreateAlarmsParameters(
                ValidationMode.ValidateAll,
                false
            );

            return dialogService.ShowMessageBoxAsync(
                stringFormater
                    .Format(
                        appResourceService.GetResource<string>("Lang.Create"),
                        appResourceService.GetResource<string>("Lang.Alarm")
                    )
                    .DispatchToDialogHeader(),
                parameters,
                ct,
                new(
                    appResourceService.GetResource<string>("Lang.Create"),
                    commandFactory.CreateCommand(async c =>
                    {
                        var alarm = parameters.CreateAlarm(Guid.NewGuid());
                        await dialogService.CloseMessageBoxAsync(c);

                        return await alarmUiService.PostAsync(
                            Guid.NewGuid(),
                            new() { Creates = [alarm] },
                            c
                        );
                    }),
                    null,
                    DialogButtonType.Primary
                ),
                dialogService.CancelButton
            );
        });

        ShowDeleteAlarmCommand = commandFactory.CreateCommand<AlarmNotify>(
            (item, ct) =>
            {
                var header = appResourceService
                    .GetResource<string>("Lang.Delete")
                    .DispatchToDialogHeader();

                return dialogService.ShowMessageBoxAsync(
                    header,
                    Dispatcher.UIThread.Invoke(() =>
                        new TextBlock
                        {
                            Text = stringFormater.Format(
                                appResourceService.GetResource<string>("Lang.AskDelete"),
                                item.Name
                            ),
                            Classes = { "text-wrap" },
                        }
                    ),
                    ct,
                    new DialogButton(
                        appResourceService.GetResource<string>("Lang.Delete"),
                        commandFactory.CreateCommand(async c =>
                        {
                            await dialogService.CloseMessageBoxAsync(c);

                            return await alarmUiService.PostAsync(
                                Guid.NewGuid(),
                                new() { DeleteIds = [item.Id] },
                                c
                            );
                        }),
                        null,
                        DialogButtonType.Primary
                    ),
                    dialogService.CancelButton
                );
            }
        );
    }

    public ICommand ShowDeleteAlarmCommand { get; }
    public ICommand ShowEditAlarmCommand { get; }
    public ICommand ShowCreateAlarmCommand { get; }
}
