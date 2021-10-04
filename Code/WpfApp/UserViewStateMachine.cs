using Stateless;

namespace WpfApp
{
    internal class UserViewStateMachine
    {
        enum Trigger
        {
            PrimaryEntered,
            SecondaryEntered,
            PrimaryNotEntered,
            SecondaryNotEntered,
            BothEntered,
            TryToCompare,
            DoneCompare,
            TryToSyncronize
        }

        enum State
        {
            EnterPrimary,
            EnterSecondary,
            CheckPrimary,
            CheckSecondary,
            ReadyForCompare,
            Compare,
            Compared,
            Syncronized
        }

        StateMachine<State, Trigger> _machine;
    }
}
