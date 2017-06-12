using System.Threading.Tasks;

namespace ServiceFabric.Utils.Logging
{
    public interface IErrorStore
    {
        Task<int> AddAsync(Error error);
    }
}