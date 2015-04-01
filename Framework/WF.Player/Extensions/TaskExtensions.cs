// <copyright file="TaskExtensions.cs" company="Wherigo Foundation">
//   WF.Player - A Wherigo Player which use the Wherigo Foundation Core.
//   Copyright (C) 2012-2015  Brice Clocher (mangatome@gmail.com)
//   Copyright (C) 2012-2015  Dirk Weltz (mail@wfplayer.com)
// </copyright>

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WF.Player.Extensions
{
    public static class TaskExtensions
    {
        private static Dictionary<Task, object> _States = new Dictionary<Task, object>();

        private static object _SyncRoot = new object();

        private class TimeoutState<T>
        {
            public Timer Timer { get; set; }

            public TaskCompletionSource<T> TaskCompletionSource { get; set; }

            public Task Task { get; set; }
        }

        /// <summary>
        /// Creates a task that waits for this task, cancelling it if it does not complete before a timeout.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public static Task<T> TimeoutAfter<T>(this Task<T> task, int millisecondsTimeout)
        {
            // Short-circuit #1: infinite timeout or task already completed
            if (task.IsCompleted || (millisecondsTimeout == Timeout.Infinite))
            {
                // Either the task has already completed or timeout will never occur.
                // No proxy necessary.
                return task;
            }

            // tcs.Task will be returned as a proxy to the caller
            TaskCompletionSource<T> tcs =
                new TaskCompletionSource<T>();

            // Short-circuit #2: zero timeout
            if (millisecondsTimeout == 0)
            {
                // We've already timed out.
                tcs.SetException(new TimeoutException());
                return tcs.Task;
            }

            // Prepares a state.
            TimeoutState<T> state = new TimeoutState<T>() { Task = task, TaskCompletionSource = tcs };

            // Set up a timer to complete after the specified timeout period
            Timer timer = new Timer(s =>
            {
                // Recover your state information
                TimeoutState<T> st = (TimeoutState<T>)s;

                // Fault our proxy with a TimeoutException
                st.TaskCompletionSource.TrySetException(new TimeoutException());

                // Removes the state.
                lock (_SyncRoot)
                {
                    _States.Remove(st.Task);
                }
            }, state, millisecondsTimeout, Timeout.Infinite);

            // Prepares the state.
            state.Timer = timer;
            _States.Add(task, state);

            // Wire up the logic for what happens when source task completes
            task.ContinueWith(t =>
            {
                // Recover our state data
                TimeoutState<T> st;
                lock (_SyncRoot)
                {
                    if (!_States.ContainsKey(t))
                    {
                        // The state has been disposed: silently return.
                        return;
                    }
                    
                    st = (TimeoutState<T>)_States[t];
                }

                // Cancel the Timer
                st.Timer.Dispose();

                // Marshal results to proxy
                MarshalTaskResults(t, st.TaskCompletionSource);

                // Removes the state if it's still up.
                lock (_SyncRoot)
                {
                    _States.Remove(t); 
                }
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

            return tcs.Task;
        }

        internal static void MarshalTaskResults<TResult>(Task source, TaskCompletionSource<TResult> proxy)
        {
            switch (source.Status)
            {
                case TaskStatus.Faulted:
                    proxy.TrySetException(source.Exception);
                    break;
                case TaskStatus.Canceled:
                    proxy.TrySetCanceled();
                    break;
                case TaskStatus.RanToCompletion:
                    Task<TResult> castedSource = source as Task<TResult>;
                    proxy.TrySetResult(
                        castedSource == null ? default(TResult) : // source is a Task
                            castedSource.Result); // source is a Task<TResult>
                    break;
            }
        }
    }
}
