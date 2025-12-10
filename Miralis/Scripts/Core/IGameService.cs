using Cysharp.Threading.Tasks;

namespace VSNL.Core
{
    /// <summary>
    /// Base interface for all VSNL Engine services.
    /// Follows the Service Locator pattern.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Asynchronously initializes the service.
        /// </summary>
        UniTask InitializeAsync();

        /// <summary>
        /// Resets the service to its default state (e.g. for a new game).
        /// </summary>
        void ResetService();
    }
}
