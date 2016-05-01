using System;
using System.Collections.Generic;

namespace Dashboard.Services.Plugins
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class BaseBuilder<T>
    {
        protected readonly List<T> Actions = new List<T>();
        private readonly List<Type> _actionTypes = new List<Type>();
        protected bool AllowDuplicateValidators { get; set; }

        /// <summary>
        /// Appends the validator output to list.
        /// </summary>
        /// <param name="actionToExecute">The action to execute.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        protected void AppendValidatorToList(T actionToExecute)
        {
            if (actionToExecute == null) throw new ArgumentNullException($"{nameof(actionToExecute)} cannot be empty");

            var actionType = actionToExecute.GetType();

            if (!AllowDuplicateValidators)
            {
                if (_actionTypes.Contains(actionType))
                {
                    throw new InvalidOperationException(
                        $"Action list already contains {actionType}. Set {nameof(AllowDuplicateValidators)} to allow duplicates in builder actions.");
                }
            }

            _actionTypes.Add(actionType);

            Actions.Add(actionToExecute);
        }

        protected void ClearConfiguration()
        {
            _actionTypes.Clear();
            Actions.Clear();
        }
    }
}
