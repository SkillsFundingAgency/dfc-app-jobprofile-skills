using DFC.App.CareerPath.FunctionalTests.Model.API;
using RestSharp;
using System.Threading.Tasks;

namespace DFC.App.CareerPath.FunctionalTests.Support.API
{
    public interface ICareerPathAPI
    {
        Task<IRestResponse<CareerPathAPIResponse>> GetById(string id);
    }
}
