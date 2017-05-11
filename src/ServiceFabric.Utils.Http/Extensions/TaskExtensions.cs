using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceFabric.Utils.Http.Extensions
{
    /// <summary>
    /// Provides a set of static methods for collections of <see cref="Task{TResult}"/>.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Waits for any of the provided <see cref="Task{TResult}"/> to complete execution and returns the <see cref="Task{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="tasks"></param>
        /// <returns>The the completed task in the tasks <see cref="IEnumerable{T}"/> argument.</returns>
        /// <remarks>Will enumarate the <paramref name="tasks"/>.</remarks>
        public static Task<TResult> WaitAny<TResult>(this IEnumerable<Task<TResult>> tasks)
        {
            var taskArray = tasks.ToArray();
            var index = Task.WaitAny(taskArray);

            return taskArray[index];
        }

        /// <summary>
        /// Waits for any of the provided <see cref="Task{TResult}"/> to complete execution, removes it from <paramref name="tasks"/> and returns the <see cref="Task{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="tasks"></param>
        /// <returns>The the completed task in the tasks <see cref="IEnumerable{T}"/> argument.</returns>
        /// <remarks>Will enumarate the <paramref name="tasks"/>.</remarks>
        public static Task<TResult> WaitAndRemoveAny<TResult>(this ICollection<Task<TResult>> tasks)
        {
            var taskArray = tasks.ToArray();
            var index = Task.WaitAny(taskArray);
            var task = taskArray[index];

            tasks.Remove(task);

            return task;
        }
    }
}
