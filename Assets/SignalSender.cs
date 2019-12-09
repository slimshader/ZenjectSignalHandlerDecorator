using UnityEngine;
using Zenject;

namespace SignalHandlerDecorator
{
    public class SignalSender : MonoBehaviour
    {
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        void Start()
        {
            _signalBus.Fire(new MessageSignal() { Message = "Hello from signal" });
        }
    }
}
