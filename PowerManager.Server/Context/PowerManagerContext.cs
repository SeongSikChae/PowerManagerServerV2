using Microsoft.EntityFrameworkCore;

namespace PowerManager.Server.Context
{
    using Entity;

    public class PowerManagerContext(DbContextOptions<PowerManagerContext> options) : DbContext(options)
    {
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<DeviceApi> DeviceApi { get; set; }
        public virtual DbSet<ElectricPower> ElectricPower { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ElectricPower>().HasKey(p => p.PowerType);
            modelBuilder.Entity<ElectricPower>().Property(p => p.PowerType).IsRequired().HasMaxLength(20).HasConversion(v => v.ToString(), v => Enum.Parse<ElectricPowerType>(v));
            modelBuilder.Entity<ElectricPower>().Property(p => p.PowerTypeName).IsRequired().HasMaxLength(20);

            modelBuilder.Entity<ElectricPower>().HasData(
                    new ElectricPower
                    {
                        PowerType = ElectricPowerType.HouseLow,
                        PowerTypeName = "주택용 저압"
                    },
                    new ElectricPower
                    {
                        PowerType = ElectricPowerType.HouseHigh,
                        PowerTypeName = "주택용 고압"
                    }
                );

            modelBuilder.Entity<Device>().HasKey(p => p.ID);
            modelBuilder.Entity<Device>().Property(p => p.ID).IsRequired().HasMaxLength(12);
            modelBuilder.Entity<Device>().Property(p => p.DeviceName).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Device>().Property(p => p.Model).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Device>().Property(p => p.Topic).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Device>().Property(p => p.PowerType).IsRequired().HasMaxLength(20).HasConversion(v => v.ToString(), v => Enum.Parse<ElectricPowerType>(v));
            modelBuilder.Entity<Device>().Property(p => p.Password).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Device>().Property(p => p.VoltCalibration).IsRequired(false).HasDefaultValue(0.0);
            modelBuilder.Entity<Device>().Property(p => p.ForwardConnector).IsRequired(false).HasMaxLength(50);

            modelBuilder.Entity<DeviceApi>().HasKey(p => new { p.UserId, p.DeviceId });
            modelBuilder.Entity<DeviceApi>().Property(p => p.UserId).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<DeviceApi>().Property(p => p.DeviceId).IsRequired().HasMaxLength(12);
            modelBuilder.Entity<DeviceApi>().Property(p => p.Verify).HasMaxLength(100);
            modelBuilder.Entity<DeviceApi>().Property(p => p.MqttKey).HasMaxLength(50);

            base.OnModelCreating(modelBuilder);
        }
    }
}
