using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace MCFire.Modules.EventAggregator
{
    [Export(typeof(IEventAggregator))]
    public class MCFireEventAggregator : Caliburn.Micro.EventAggregator
    {
        public MCFireEventAggregator()
        {
            Console.WriteLine();
        }
    }
}
