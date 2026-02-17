using StatePulse.Net;

namespace Warden.Stores;

public class TestEffect : IEffect<TestAction>
{
    public async Task EffectAsync(TestAction action, IDispatcher dispatcher) { }
}
