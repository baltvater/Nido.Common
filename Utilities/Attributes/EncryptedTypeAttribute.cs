using System;
using System.Collections.Generic;
using System.Security.Cryptography;
/*
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Aspects.Dependencies;

namespace Nido.Common.Utilities.Attributes
{
    public interface IEncrypted
    {
        object AsClear(string Name);
    }

    [Serializable]
    [AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.Before, typeof(EncryptedAttribute))]
    [IntroduceInterface(typeof(IEncrypted), OverrideAction = InterfaceOverrideAction.Ignore)]
    [AttributeUsage(AttributeTargets.Class)]
    public class EncryptedTypeAttribute : InstanceLevelAspect, IEncrypted
    {

        [IntroduceMember(IsVirtual = false, OverrideAction = MemberOverrideAction.OverrideOrFail, Visibility = PostSharp.Reflection.Visibility.Public)]
        public Dictionary<string, string> EncryptedValues { get; set; }

        public override void RuntimeInitialize(Type type)
        {
            EncryptedValues = new Dictionary<string, string>();
        }

        public object AsClear(string Name)
        {
            if (null != EncryptedValues)
            {
                if (EncryptedValues.ContainsKey(Name))
                    return System.Text.UnicodeEncoding.Unicode.GetString(ProtectedData.Unprotect(
                        Convert.FromBase64String(EncryptedValues[Name].Substring(136)),
                        Convert.FromBase64String(EncryptedValues[Name].Substring(0, 136)),
                        DataProtectionScope.CurrentUser));
            }
            return null;
        }
    }
}*/

