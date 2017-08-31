using System;
namespace Nido.Common.BackEnd
{
    public interface IBaseObject 
    {
        bool CanDelete { get; set; }
        string CreatePopupText(string text);
        void EncryptRecords<T>() where T : IBaseObject;
        void DecryptRecords<T>() where T : IBaseObject;
        string DeleteError { get; }
        string DisplayName { get; set; }
        string Text { get; set; }
        string Value { get; set; }
    }
}
