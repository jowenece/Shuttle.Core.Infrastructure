﻿using System;
using NUnit.Framework;

namespace Shuttle.Core.Infrastructure.Tests
{
    [TestFixture]
    public class DefaultComponentContainerFixture
    {
        [Test]
        public void Should_be_able_to_register_and_resolve_a_type()
        {
            var container = new DefaultComponentContainer();
            var serviceType = typeof (IDoSomething);
            var implementationType = typeof (DoSomething);
            var bogusType = typeof (object);

            container.Register(serviceType, implementationType, Lifestyle.Singleton);

            Assert.NotNull(container.Resolve(serviceType));
            Assert.AreEqual(implementationType, container.Resolve(serviceType).GetType());
            Assert.Throws<TypeNotRegisteredException>(()=>container.Resolve(bogusType));
        }

        [Test]
        public void Should_not_be_able_to_register_duplicate_types()
        {
            var container = new DefaultComponentContainer();
            var serviceType = typeof(IDoSomething);
            var implementationType = typeof(DoSomething);

            container.Register(serviceType, implementationType, Lifestyle.Singleton);

            Assert.Throws<DuplicateTypeRegistrationException>(() => container.Register(serviceType, implementationType, Lifestyle.Singleton));

            Assert.NotNull(container.Resolve(serviceType));

            Assert.Throws<DuplicateTypeRegistrationException>(() => container.Register(serviceType, new DoSomething()));
        }

        [Test]
        public void Should_not_be_able_to_register_duplicate_instances()
        {
            var container = new DefaultComponentContainer();
            var serviceType = typeof(IDoSomething);
            var implementationType = typeof(DoSomething);

            container.Register(serviceType, new DoSomething());

            Assert.NotNull(container.Resolve(serviceType));

            Assert.Throws<DuplicateTypeRegistrationException>(() => container.Register(serviceType, implementationType, Lifestyle.Singleton));
            Assert.Throws<DuplicateTypeRegistrationException>(() => container.Register(serviceType, new DoSomething()));
        }

        [Test]
        public void Should_be_able_to_use_constructor_injection()
        {
            var container = new DefaultComponentContainer();
            var serviceType = typeof(IDoSomething);
            var implementationType = typeof(DoSomethingWithDependency);

            container.Register(serviceType, implementationType, Lifestyle.Singleton);

            Assert.Throws<TypeNotRegisteredException>(() => container.Resolve(serviceType));
            Assert.Throws<TypeNotRegisteredException>(() => container.Resolve<IDoSomething>());

            var someDependency = new SomeDependency();

            container.Register(typeof (ISomeDependency), someDependency);

            Assert.AreSame(someDependency, container.Resolve<IDoSomething>().SomeDependency);
        }
    }
}