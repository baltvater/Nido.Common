using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.BackEnd
{
    public interface IContextAgent<TContext, Entity>
        where TContext : BaseObjectConext, IBaseObjectConext
        where Entity : class, IBaseObject
    {
        TContext GetContext();
        System.Data.Entity.DbSet<Entity> GetObjectSet();
    }
}
