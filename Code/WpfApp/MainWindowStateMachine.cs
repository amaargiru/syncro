using Stateless;
using Stateless.Graph;

namespace WpfApp;

public class MainWindowStateMachine
{
    enum Trigger
    {
        PrimaryEntered,
        SecondaryEntered,
        PrimaryNotEntered,
        SecondaryNotEntered,
        BothEntered,
        DoCompare,
        DoneCompare,
        DoSyncronize,
        CompareAgain
    }

    enum State
    {
        EnterPrimary,
        EnterSecondary,
        CheckPrimary,
        CheckSecondary,
        ReadyForCompare,
        Comparing,
        Compared,
        Synchronizing
    }

    State _state = State.EnterPrimary;
    StateMachine<State, Trigger> _machine;

    public MainWindowStateMachine()
    {
        _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);

        _machine.Configure(State.EnterPrimary)
            .Permit(Trigger.PrimaryEntered, State.CheckSecondary);

        _machine.Configure(State.EnterSecondary)
            .Permit(Trigger.SecondaryEntered, State.CheckPrimary);

        _machine.Configure(State.CheckSecondary)
            .Permit(Trigger.SecondaryNotEntered, State.EnterSecondary)
            .Permit(Trigger.BothEntered, State.ReadyForCompare);

        _machine.Configure(State.CheckPrimary)
            .Permit(Trigger.PrimaryNotEntered, State.EnterPrimary)
            .Permit(Trigger.BothEntered, State.ReadyForCompare);

        _machine.Configure(State.ReadyForCompare)
            .Permit(Trigger.DoCompare, State.Comparing);

        _machine.Configure(State.Comparing)
            .Permit(Trigger.DoneCompare, State.Compared)
            .Permit(Trigger.CompareAgain, State.ReadyForCompare);

        _machine.Configure(State.Compared)
            .Permit(Trigger.DoSyncronize, State.Synchronizing);

        _machine.Configure(State.Synchronizing)
            .Permit(Trigger.CompareAgain, State.ReadyForCompare);
    }

    public string ToDotGraph()
    {
        return UmlDotGraph.Format(_machine.GetInfo());
    }
}
