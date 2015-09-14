using System;
using System.Reflection;
using Ninject.Planning.Targets;

namespace Protogame
{
    class ComponentInterfaceTarget : Target<Type>
    {
        public ComponentInterfaceTarget(MemberInfo member, Type site) : base(member, site)
        {
        }

        public override string Name => Site.FullName;
        public override Type Type => Site;
    }
}
