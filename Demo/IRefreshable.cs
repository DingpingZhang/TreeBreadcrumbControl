using System.Threading.Tasks;

namespace Demo
{
    public interface IRefreshable
    {
        Task<bool> RefreshAsync();
    }
}
