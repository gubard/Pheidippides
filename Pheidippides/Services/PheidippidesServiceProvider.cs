using Jab;

namespace Pheidippides.Services;

[ServiceProviderModule]
[Singleton(typeof(IAlarmMemoryCache), typeof(AlarmMemoryCache))]
[Singleton(typeof(IPheidippidesViewModelFactory), typeof(PheidippidesViewModelFactory))]
[Singleton(typeof(PheidippidesCommands))]
public interface IPheidippidesServiceProvider;
