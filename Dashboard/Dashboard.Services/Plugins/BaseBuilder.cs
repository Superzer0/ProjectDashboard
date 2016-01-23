﻿using System;
using System.Collections.Generic;

namespace Dashboard.Services.Plugins
{
    internal abstract class BaseBuilder<T>
    {
        protected readonly List<T> Actions = new List<T>();
        private readonly List<Type> _actionTypes = new List<Type>();
        protected bool AllowDuplicateValidators { get; set; }

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