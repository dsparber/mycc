using System.Threading.Tasks;
using System;

namespace MyCryptos.Core.Tasks
{
	public static partial class ApplicationTasks
	{
		private static bool CanBeStarted(this Task task) => task == null || !task.Status.Equals(TaskStatus.Running);

		private static async Task GetTask(this Task task, Task actionTask) => await GetTask(task, actionTask);

		private static Task GetTask(this Task task, Action action)
		{
			if (task.CanBeStarted())
			{
				task = Task.Factory.StartNew(action);
			}
			return task;
		}
	}
}