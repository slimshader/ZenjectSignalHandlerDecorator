using UnityEngine;
using Zenject;

namespace SignalHandlerDecorator
{
    public interface IUserContext
    {
        bool IsLoggedIn { get; }
    }

    public class MessageSignal
    {
        public string Message;
    }

    public interface ISignalHandler<TSignal>
    {
        void Handle(TSignal signal);
    }

    public sealed class ConcreteSignalHandler : ISignalHandler<MessageSignal>
    {
        public void Handle(MessageSignal signal)
        {
            Debug.Log(signal.Message);
        }
    }

    public sealed class DefaultUserContext : IUserContext
    {
        public DefaultUserContext(bool isLoggedIn)
        {
            IsLoggedIn = isLoggedIn;
        }

        public bool IsLoggedIn { get; private set; }
    }

    public sealed class LoginLockedHanlderDecoratror<TSignal> : ISignalHandler<TSignal>
    {
        private readonly ISignalHandler<TSignal> _target;
        private readonly IUserContext _userContext;

        public LoginLockedHanlderDecoratror(ISignalHandler<TSignal> target, IUserContext userContext)
        {
            _target = target;
            _userContext = userContext;

            Debug.Log($"Decorator({target.GetType().Name})");
        }

        public void Handle(TSignal signal)
        {
            if (_userContext.IsLoggedIn)
            {
                Debug.Log("User logged in");
                _target.Handle(signal);
            }
            else
            {
                Debug.Log("User not logged in");
            }
        }
    }


    public class TestInstaller : MonoInstaller
    {
        [SerializeField]
        bool _isUserLoggedIn;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<MessageSignal>();

            Container.Bind<ISignalHandler<MessageSignal>>().To<ConcreteSignalHandler>().AsSingle();

            Container.BindSignal<MessageSignal>()
                .ToMethod<ISignalHandler<MessageSignal>>(h => h.Handle)
                .FromResolve();

            Container.Bind<IUserContext>().FromInstance(new DefaultUserContext(_isUserLoggedIn));

            Container.Decorate<ISignalHandler<MessageSignal>>()
                .With<LoginLockedHanlderDecoratror<MessageSignal>>();

        }
    }
}
