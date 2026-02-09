using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Pheidippides.Models;
using Pheidippides.Services;

namespace Pheidippides.Helpers;

public static class PheidippidesCommands
{
    static PheidippidesCommands()
    {
        var dialogService = DiHelper.ServiceProvider.GetService<IDialogService>();
        var appResourceService = DiHelper.ServiceProvider.GetService<IAppResourceService>();
        var stringFormater = DiHelper.ServiceProvider.GetService<IStringFormater>();
        var viewModelFactory = DiHelper.ServiceProvider.GetService<IPheidippidesViewModelFactory>();
        var alarmUiService = DiHelper.ServiceProvider.GetService<IAlarmUiService>();

        ShowEditAlarmCommand = UiHelper.CreateCommand<AlarmNotify>(
            (item, ct) =>
            {
                var parameters = viewModelFactory.CreateAlarmsParameters(item);

                return dialogService.ShowMessageBoxAsync(
                    new(
                        stringFormater
                            .Format(
                                appResourceService.GetResource<string>("Lang.EditItem"),
                                appResourceService.GetResource<string>("Lang.Alarm")
                            )
                            .DispatchToDialogHeader(),
                        parameters,
                        new(
                            appResourceService.GetResource<string>("Lang.Edit"),
                            UiHelper.CreateCommand(async c =>
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
                        UiHelper.CancelButton
                    ),
                    ct
                );
            }
        );

        ShowCreateAlarmCommand = UiHelper.CreateCommand(ct =>
        {
            var parameters = viewModelFactory.CreateAlarmsParameters(
                ValidationMode.ValidateAll,
                false
            );

            return dialogService.ShowMessageBoxAsync(
                new(
                    stringFormater
                        .Format(
                            appResourceService.GetResource<string>("Lang.Create"),
                            appResourceService.GetResource<string>("Lang.Alarm")
                        )
                        .DispatchToDialogHeader(),
                    parameters,
                    new(
                        appResourceService.GetResource<string>("Lang.Create"),
                        UiHelper.CreateCommand(async c =>
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
                    UiHelper.CancelButton
                ),
                ct
            );
        });

        ShowDeleteAlarmCommand = UiHelper.CreateCommand<AlarmNotify>(
            (item, ct) =>
            {
                var header = appResourceService
                    .GetResource<string>("Lang.Delete")
                    .DispatchToDialogHeader();

                return dialogService.ShowMessageBoxAsync(
                    new(
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
                        new DialogButton(
                            appResourceService.GetResource<string>("Lang.Delete"),
                            UiHelper.CreateCommand(async c =>
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
                        UiHelper.CancelButton
                    ),
                    ct
                );
            }
        );
    }

    public static readonly ICommand ShowDeleteAlarmCommand;
    public static readonly ICommand ShowEditAlarmCommand;
    public static readonly ICommand ShowCreateAlarmCommand;
}
