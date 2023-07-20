namespace PowerManagerServer.Context
{
    public interface IUserDeviceStore
    {
        Task<UserDevice?> FindByIdAsync(string thumbprint, string deviceId, CancellationToken cancellationToken);

        Task<List<string>> GetThumbprintListByDeviceId(string deviceId, CancellationToken cancellationToken);

        Task CreateAsync(UserDevice userDevice, CancellationToken cancellationToken);

        Task UpdateAsync(UserDevice userDevice, CancellationToken cancellationToken);

        Task DeleteAsync(string thumbprint, string deviceId, CancellationToken cancellationToken);
    }
}
