using Jab;
using Pheidippides.Ui;

namespace Pheidippides.Services;

[ServiceProviderModule]
[Transient(typeof(AlarmsViewModel))]
[Singleton(typeof(IAlarmMemoryCache), typeof(AlarmMemoryCache))]
[Singleton(typeof(IPheidippidesViewModelFactory), typeof(PheidippidesViewModelFactory))]
public interface IPheidippidesServiceProvider;
