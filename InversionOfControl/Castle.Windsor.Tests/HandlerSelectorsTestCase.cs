using System;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Naming;
using Castle.Windsor;
using NUnit.Framework;

namespace Castle.Windsor.Tests
{
    [TestFixture]
    public class HandlerSelectorsTestCase
    {
        public interface IWatcher
        {
            event Action OnSomethingInterestingToWatch;
        }

        public class BirdWatcher : IWatcher
        {
            public event Action OnSomethingInterestingToWatch = delegate { };
        }

        public class SatiWatcher : IWatcher
        {
            public event Action OnSomethingInterestingToWatch = delegate { };
        }

        public enum Interest
        {
            None,
            Biology,
            Astronomy
        }

        public class WatcherSelector : IHandlerSelector
        {
            public Interest Interest = Interest.None;

            public bool HasOpinionAbout(string key, Type service)
            {
                return Interest != Interest.None && service == typeof(IWatcher);
            }

            public IHandler Select(string key, Type service, IHandler[] handlers)
            {
                foreach (IHandler handler in handlers)
                {
                    if (handler.ComponentModel.Name.Contains(Interest.ToString().ToLowerInvariant()))
                        return handler;
                }
                return null;
            }
        }

        [Test]
        public void SelectUsingBusinessLogic()
        {
            IWindsorContainer container = new WindsorContainer();
            container
                .AddComponent<IWatcher, BirdWatcher>("bird.watcher")
                .AddComponent<IWatcher, SatiWatcher>("astronomy.watcher");
            var selector = new WatcherSelector();
            container.Kernel.AddHandlerSelector(selector);

            Assert.IsInstanceOfType(typeof (BirdWatcher), container.Resolve<IWatcher>(), "default");
            selector.Interest=Interest.Biology;
            Assert.IsInstanceOfType(typeof(BirdWatcher), container.Resolve<IWatcher>(), "explicit");
            selector.Interest=Interest.Astronomy;
            Assert.IsInstanceOfType(typeof(BirdWatcher), container.Resolve<IWatcher>(), "change-by-context");
        }
    }
}