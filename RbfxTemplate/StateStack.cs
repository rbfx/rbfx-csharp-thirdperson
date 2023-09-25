using System;
using System.Collections.Generic;
using Urho3DNet;

namespace RbfxTemplate
{
    /// <summary>
    ///     Application state stack.
    ///     Keeps stack of application states to navigate between them when Esc button is pressed.
    /// </summary>
    public class StateStack : IDisposable
    {
        /// <summary>
        ///     Cached state manager subsystem.
        /// </summary>
        private readonly StateManager _stateManager;

        /// <summary>
        ///     Stack of aplication states.
        /// </summary>
        private readonly Stack<SharedPtr<ApplicationState>> _stack = new Stack<SharedPtr<ApplicationState>>();

        /// <summary>
        ///     Construct instance of StateStack.
        /// </summary>
        /// <param name="stateManager">State manager subsystem instance.</param>
        public StateStack(StateManager stateManager)
        {
            _stateManager = stateManager;
        }

        /// <summary>
        ///     Current state from the top of the stack.
        /// </summary>
        public ApplicationState State => _stack.Count > 0 ? _stack.Peek() : null;

        /// <summary>
        ///     Push state to the stack and make it current.
        /// </summary>
        /// <param name="state">State to push.</param>
        public void Push(ApplicationState state)
        {
            if (state != null)
            {
                _stateManager.EnqueueState(state);
                _stack.Push(state);
            }
        }

        /// <summary>
        ///     Pop state from the top of the stack.
        ///     The application transitions to the previous state.
        /// </summary>
        public void Pop()
        {
            if (_stack.Count > 0)
            {
                _stack.Peek().Dispose();
                _stack.Pop();
            }

            _stateManager.EnqueueState(State);
        }

        /// <summary>
        ///     Switch between states.
        ///     Application transitions to the new state without going to the previous first.
        /// </summary>
        /// <param name="state">State to transition to.</param>
        public void Switch(ApplicationState state)
        {
            if (_stack.Count > 0)
            {
                _stack.Peek().Dispose();
                _stack.Pop();
            }

            if (state != null)
            {
                _stack.Push(state);
                _stateManager.EnqueueState(State);
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            while (_stack.Count > 0)
            {
                _stack.Peek().Dispose();
                _stack.Pop();
            }
        }
    }
}