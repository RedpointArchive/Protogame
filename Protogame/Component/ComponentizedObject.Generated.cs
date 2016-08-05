using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Protoinject;

namespace Protogame
{
	public partial class ComponentizedObject
	{
		private class ImplementedComponentCallable<T> : IInternalComponentCallable, IComponentCallable where T : class
		{
			private readonly Action<T> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke()
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i]);
				}
			}
		}

		protected IComponentCallable RegisterCallable<T>(Action<T> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1> : IInternalComponentCallable, IComponentCallable<T1> where T : class
		{
			private readonly Action<T, T1> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1);
				}
			}
		}

		protected IComponentCallable<T1> RegisterCallable<T, T1>(Action<T, T1> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2> : IInternalComponentCallable, IComponentCallable<T1, T2> where T : class
		{
			private readonly Action<T, T1, T2> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2);
				}
			}
		}

		protected IComponentCallable<T1, T2> RegisterCallable<T, T1, T2>(Action<T, T1, T2> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3> : IInternalComponentCallable, IComponentCallable<T1, T2, T3> where T : class
		{
			private readonly Action<T, T1, T2, T3> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3> RegisterCallable<T, T1, T2, T3>(Action<T, T1, T2, T3> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3, T4> : IInternalComponentCallable, IComponentCallable<T1, T2, T3, T4> where T : class
		{
			private readonly Action<T, T1, T2, T3, T4> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3, T4> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3, arg4);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3, T4> RegisterCallable<T, T1, T2, T3, T4>(Action<T, T1, T2, T3, T4> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3, T4>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3, T4, T5> : IInternalComponentCallable, IComponentCallable<T1, T2, T3, T4, T5> where T : class
		{
			private readonly Action<T, T1, T2, T3, T4, T5> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3, T4, T5> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3, arg4, arg5);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3, T4, T5> RegisterCallable<T, T1, T2, T3, T4, T5>(Action<T, T1, T2, T3, T4, T5> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3, T4, T5>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6> : IInternalComponentCallable, IComponentCallable<T1, T2, T3, T4, T5, T6> where T : class
		{
			private readonly Action<T, T1, T2, T3, T4, T5, T6> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3, T4, T5, T6> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3, arg4, arg5, arg6);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3, T4, T5, T6> RegisterCallable<T, T1, T2, T3, T4, T5, T6>(Action<T, T1, T2, T3, T4, T5, T6> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7> : IInternalComponentCallable, IComponentCallable<T1, T2, T3, T4, T5, T6, T7> where T : class
		{
			private readonly Action<T, T1, T2, T3, T4, T5, T6, T7> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3, T4, T5, T6, T7> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3, arg4, arg5, arg6, arg7);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3, T4, T5, T6, T7> RegisterCallable<T, T1, T2, T3, T4, T5, T6, T7>(Action<T, T1, T2, T3, T4, T5, T6, T7> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8> : IInternalComponentCallable, IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8> where T : class
		{
			private readonly Action<T, T1, T2, T3, T4, T5, T6, T7, T8> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3, T4, T5, T6, T7, T8> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8> RegisterCallable<T, T1, T2, T3, T4, T5, T6, T7, T8>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> : IInternalComponentCallable, IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9> where T : class
		{
			private readonly Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9> RegisterCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IInternalComponentCallable, IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> where T : class
		{
			private readonly Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> RegisterCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : IInternalComponentCallable, IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> where T : class
		{
			private readonly Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RegisterCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : IInternalComponentCallable, IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> where T : class
		{
			private readonly Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RegisterCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : IInternalComponentCallable, IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> where T : class
		{
			private readonly Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> RegisterCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : IInternalComponentCallable, IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> where T : class
		{
			private readonly Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> RegisterCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
		private class ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : IInternalComponentCallable, IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> where T : class
		{
			private readonly Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> _method;
			private T[] _targets;

			public ImplementedComponentCallable(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> method)
			{
				_method = method;
			}

			public void SyncComponents(object[] components)
			{
				var c = new List<T>();
				for (var i = 0; i < components.Length; i++)
				{
					if (components[i] is T)
					{
						c.Add((T)components[i]);
					}
				}
				_targets = c.ToArray();
			}

			public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
			{
				for (var i = 0; i < _targets.Length; i++)
				{
					_method(_targets[i], arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
				}
			}
		}

		protected IComponentCallable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> RegisterCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> method) where T : class
		{
			var callable = new ImplementedComponentCallable<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(method);
			callable.SyncComponents(this.Components);
			_knownCallablesForSync.Add(callable);
			return callable;
		}
	}
}