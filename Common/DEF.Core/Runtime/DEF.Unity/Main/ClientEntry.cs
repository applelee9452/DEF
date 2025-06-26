#if DEF_CLIENT

using VContainer;
using VContainer.Unity;

public class ClientEntry : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<TimerShaft>(Lifetime.Singleton);
        builder.Register<UiMgr>(Lifetime.Singleton);
        builder.Register<Updater>(Lifetime.Singleton);

        builder.RegisterEntryPoint<Client>();

        //builder.RegisterComponent(helloScreen);
    }
}

#endif