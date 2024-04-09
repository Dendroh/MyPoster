using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Unity.Network
{
    sealed class PacketBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type returntype = null;
            assemblyName = Assembly.GetExecutingAssembly().FullName;

            string sharedAssemblyName = "SharedAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            typeName = typeName.Replace(sharedAssemblyName, assemblyName);

            returntype = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
            return returntype;
        }
    }
}