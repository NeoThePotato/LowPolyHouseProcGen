using System;
using System.Reflection;

namespace Extensions
{
	public static class Reflection
	{
		public static Type GetType(string typeName)
		{
			var type = Type.GetType(typeName);
			if (type != null)
				return type;

			var currentAssembly = Assembly.GetExecutingAssembly();
			var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
			foreach (var assemblyName in referencedAssemblies)
			{
				var assembly = Assembly.Load(assemblyName);
				if (assembly != null)
				{
					type = assembly.GetType(typeName);
					if (type != null)
						return type;
				}
			}
			return null;
		}
	}
}
