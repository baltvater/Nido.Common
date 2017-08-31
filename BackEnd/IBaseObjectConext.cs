using System;
namespace Nido.Common.BackEnd
{
    public interface IBaseObjectConext
    {
        System.Data.Entity.DbSet<AuditTrail> AuditTrails { get; set; }
        System.Data.Entity.Core.Objects.ObjectContext CurrentObjectContext { get; }
        void Dispose();
        bool EnableOptTracking { get; set; }
        int ExecuteStoredProcedure(string nameOfSp, params object[] parameters);
        System.Collections.Generic.IEnumerable<T> ExecuteStoredProcedure<T>(string nameOfSp);
        System.Collections.Generic.IEnumerable<T> ExecuteStoredProcedure<T>(string nameOfSp, params object[] parameters);
        string SystemName { get; set; }
        string UserName { get; set; }
    }
}
