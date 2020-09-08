using Newtonsoft.Json;
using System;

namespace DFC.App.CareerPath.FunctionalTests.Support.Interface
{
    internal interface IGeneralSupport
    {
        string RandomString(int length);

        byte[] ConvertObjectToByteArray(object obj);

        T GetResource<T>(string resourceName);
    }
}
