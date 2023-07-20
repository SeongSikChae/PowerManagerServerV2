namespace PowerManagerServer.Context
{
    public interface IDeviceStore
    {
        Task<Device?> FindByIdAsync(string id, CancellationToken cancellationToken);

        Task CreateAsync(Device device, CancellationToken cancellationToken);

        Task UpdateAsync(Device device, CancellationToken cancellationToken);

        Task DeleteAsync(string id, CancellationToken cancellationToken);
    }
}
