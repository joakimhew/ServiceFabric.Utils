using System.Threading.Tasks;

namespace ServiceFabric.Utils.Ipc.Http
{
    public interface IErrorStore
    {
        Task<int> AddAsync(Error error);
    }
}