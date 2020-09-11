using Newtonsoft.Json;
using System;

namespace DFC.App.JobProfileSkills.Tests.API.FunctionalTests.Support.Interface
{
    internal interface IGeneralSupport
    {
        string RandomString(int length);

        byte[] ConvertObjectToByteArray(object obj);

        T GetResource<T>(string resourceName);
    }
}
