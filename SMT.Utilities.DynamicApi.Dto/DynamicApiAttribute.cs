using System;

namespace SMT.Utilities.DynamicApi.Dto
{
	/// <summary>
	/// When attached to an interface, causes all implementors to be picked up for dynamic api routing
	/// </summary>
	public class DynamicApiAttribute: Attribute
	{
		public readonly string RouteName;

		/// <summary>
		/// When attached to an interface, causes all implementors to be picked up for dynamic api routing
		/// </summary>
		/// <param name="route">Name of this Object Route e.g. Base/Prefix/{ThisRoute}/Action</param>
		public DynamicApiAttribute(string route)
		{
			this.RouteName = route;
		}
	}
}
