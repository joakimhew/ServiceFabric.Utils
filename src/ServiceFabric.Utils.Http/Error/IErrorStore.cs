using System.Threading.Tasks;

namespace ServiceFabric.Utils.Http.Error
{
    public interface IErrorStore
    {
        Task<int> AddAsync(Error error);
    }
}