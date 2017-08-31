using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Nido.Common.BackEnd
{
    public class ActiveContextAgent<TContext, Entity> : IContextAgent<TContext, Entity>
        where TContext : BaseObjectConext, IBaseObjectConext
        where Entity : class, IBaseObject
    {
        public ActiveContextAgent(TContext context)
        {
            _context = context;
        }

        protected TContext _context { get; set; }
        protected DbSet<Entity> _entitySet { get; set; }

        /// <summary>
        /// Targetted Get object set method. 
        /// By default this will get the object set of the corresponding entity object.
        /// </summary>
        /// <returns>DbSet of the entity</returns>
        public DbSet<Entity> GetObjectSet()
        {
            var fulltypename = typeof(Entity).AssemblyQualifiedName;
            if (fulltypename == null)
                throw new ArgumentException("Invalid Type passed to GetObjectSet!");
            if (_entitySet == null)
            {
                _entitySet = _context.Set<Entity>();
            }
            return _entitySet;
        }

        public TContext GetContext()
        {
            return _context;
        }
    }
}
