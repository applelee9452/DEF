using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF;

public abstract class SimpleState<T> where T : class
{
    public abstract string GetName();
   
    public abstract Task Enter();

    public abstract Task Exit();

    public abstract Task<string> Update(float tm);

    public abstract Task<string> OnEvent(string ev_name, T ev_param);
}

public class SimpleFsm<T> where T : class
{
    Dictionary<string, SimpleState<T>> MapState { get; set; } = new Dictionary<string, SimpleState<T>>();
    SimpleState<T> Current { get; set; }
   
    public void AddState(SimpleState<T> state, bool is_default = false)
    {
        MapState[state.GetName()] = state;

        if (is_default)
        {
            Current = state;
        }
    }

    public string GetCurrentState()
    {
        return Current.GetName();
    }

    public Task Enter()
    {
        return Current.Enter();
    }

    public Task Exit()
    {
        return Current.Exit();
    }

    public async Task Update(float tm)
    {
        string next_state_name = await Current.Update(tm);
        if (string.IsNullOrEmpty(next_state_name)) return;

        await Current.Exit();

        Current = MapState[next_state_name];

        await Current.Enter();
    }

    public async Task OnEvent(string ev_name, T ev_param)
    {
        string next_state_name = await Current.OnEvent(ev_name, ev_param);
        if (string.IsNullOrEmpty(next_state_name)) return;

        await Current.Exit();

        Current = MapState[next_state_name];

        await Current.Enter();
    }
}