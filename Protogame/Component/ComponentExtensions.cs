using System;
using System.Linq;

namespace Protogame
{ 
    public static class ComponentExtensions
    {
        public static T GetPublicComponent<T>(this ComponentizedObject componentizedObject)
        {
            var components = componentizedObject.PublicComponents.OfType<T>().ToArray();

            if (components.Length == 0)
            {
                throw new InvalidOperationException("A public component of type " + typeof(T).FullName +
                                                    " could not be found on this object.  Is it private?");
            }
            if (components.Length == 2)
            {
                throw new InvalidOperationException("More than one public component of type " + typeof(T).FullName +
                                                    " was found on this object.");
            }

            return components[0];
        }

        public static object GetPublicComponent(this ComponentizedObject componentizedObject, Type componentType)
        {
            var components = componentizedObject.PublicComponents.Where(componentType.IsInstanceOfType).ToArray();

            if (components.Length == 0)
            {
                throw new InvalidOperationException("A public component of type " + componentType.FullName +
                                                    " could not be found on this object.  Is it private?");
            }
            if (components.Length == 2)
            {
                throw new InvalidOperationException("More than one public component of type " + componentType.FullName +
                                                    " was found on this object.");
            }

            return components[0];
        }
    }
}
