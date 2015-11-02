using System.Collections.Generic;
using System.Linq;
using Ninject;
using Prototest.Library.Version1;

namespace Protogame.Tests
{
    public class ComponentTests
    {
        private readonly IAssert _assert;

        private class ComponentA
        {
        }

        private class ComponentB : ComponentizedObject
        {
            private readonly ComponentA _componentA;

            public ComponentB(
                IRequireComponent<ComponentA> componentA,
                IInstantiateComponent<ComponentD> componentD)
            {
                _componentA = componentA.Component;
                RegisterPublicComponent(componentD.Component);
            }
        }

        private class ComponentC
        {
            private readonly ComponentD _componentD;

            public ComponentC(
                IRequireComponentInDescendants<ComponentD> componentD)
            {
                _componentD = componentD.Component;
            }
        }

        private class ComponentD
        {
        }

        private class EntityB : ComponentizedEntity
        {
            public EntityB(
                IRequireComponentInHierarchy<ComponentA> componentA)
            {
            }
        }

        private class EntityA : ComponentizedEntity
        {
            public ComponentA ComponentA { get; set; }

            public EntityA(
                IInstantiateComponent<ComponentA> componentA)
            {
                ComponentA = componentA.Component;
                this.RegisterPublicComponent(componentA);
            }
        }

        private class EntityC : ComponentizedEntity
        {
            public ComponentD ComponentD { get; set; }

            public EntityC(
                IInstantiateComponent<ComponentD> componentD)
            {
                ComponentD = componentD.Component;
            }
        }

        private class EntityD : ComponentizedEntity
        {
            public ComponentA ComponentA { get; set; }

            public EntityD(
                IRequireComponent<ComponentA> componentA)
            {
                ComponentA = componentA.Component;
                this.RegisterPublicComponent(componentA);
            }
        }

        private class MyDesignedWorld : IWorld
        {
            public MyDesignedWorld(IEntityFactory entityFactory)
            {
                entityFactory.HierarchyRoot = this;
                entityFactory.PlanForEntityCreation<EntityA>();
                entityFactory.PlanForEntityCreation<EntityB>();
                Entities = new List<IEntity>(entityFactory.CreateEntities());
            }

            public void Dispose()
            {
            }

            public IList<IEntity> Entities { get; }

            public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
            {
            }

            public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
            {
            }

            public void Update(IGameContext gameContext, IUpdateContext updateContext)
            {
            }
        }

        public ComponentTests(IAssert assert)
        {
            _assert = assert;
        }

        public void TestInstantiateComponentResolution()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameComponentModule>();

            var factory = kernel.Get<IEntityFactory>();
            factory.HierarchyRoot = new object();
            factory.PlanForEntityCreation<EntityC>();
            var entities = factory.CreateEntities();

            _assert.NotNull(((EntityC)entities[0]).ComponentD);
        }
        
        public void TestRequireComponentResolution()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameComponentModule>();

            var factory = kernel.Get<IEntityFactory>();
            factory.HierarchyRoot = new object();
            factory.PlanForEntityCreation<EntityA>();
            factory.PlanForEntityCreation<EntityD>();
            var entities = factory.CreateEntities();

            var entityA = entities.OfType<EntityA>().First();
            var entityD = entities.OfType<EntityD>().First();

            _assert.Same(entityA.ComponentA, entityD.ComponentA);
        }
        
        public void TestRequireComponentResolutionInReverse()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameComponentModule>();

            var factory = kernel.Get<IEntityFactory>();
            factory.HierarchyRoot = new object();
            factory.PlanForEntityCreation<EntityD>(); // These are swapped around
            factory.PlanForEntityCreation<EntityA>();
            var entities = factory.CreateEntities();

            var entityA = entities.OfType<EntityA>().First();
            var entityD = entities.OfType<EntityD>().First();

            _assert.Same(entityA.ComponentA, entityD.ComponentA);
        }
        
        public void TestComponentResolution()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameComponentModule>();

            var world = kernel.Get<MyDesignedWorld>();
        }
    }
}
