using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nido.Common;

namespace Nido.Common.BackEnd
{
    public class ContextAgentFactory<TContext, Entity>
        where TContext : BaseObjectConext, IBaseObjectConext
        where Entity : class, IBaseObject
    {
        public IContextAgent<TContext, Entity> GetContext(TContext context)
        {
            if (Convert.ToBoolean(ConfigSettings.ReadConfigValue(ConfigSettings.ENABLE_TESTING, "false")))
                return new FakeContextAgent<TContext, Entity>(context);
            else
                return new ActiveContextAgent<TContext, Entity>(context);
        }
    }
}
