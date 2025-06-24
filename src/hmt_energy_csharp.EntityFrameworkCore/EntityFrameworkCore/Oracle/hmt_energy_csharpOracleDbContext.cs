using hmt_energy_csharp;
using hmt_energy_csharp.CII.Coefficients;
using hmt_energy_csharp.CII.FuelCoefficients;
using hmt_energy_csharp.CII.Ratings;
using hmt_energy_csharp.Energy.Batteries;
using hmt_energy_csharp.Energy.Configs;
using hmt_energy_csharp.Energy.Flowmeters;
using hmt_energy_csharp.Energy.Generators;
using hmt_energy_csharp.Energy.LiquidLevels;
using hmt_energy_csharp.Energy.PowerUnits;
using hmt_energy_csharp.Energy.Predictions;
using hmt_energy_csharp.Energy.Shafts;
using hmt_energy_csharp.Energy.SternSealings;
using hmt_energy_csharp.Energy.SupplyUnits;
using hmt_energy_csharp.Energy.TotalIndicators;
using hmt_energy_csharp.Engineroom.AssistantDecisions;
using hmt_energy_csharp.Engineroom.CompositeBoilers;
using hmt_energy_csharp.Engineroom.CompressedAirSupplies;
using hmt_energy_csharp.Engineroom.CoolingFreshWaters;
using hmt_energy_csharp.Engineroom.CoolingSeaWaters;
using hmt_energy_csharp.Engineroom.CoolingWaters;
using hmt_energy_csharp.Engineroom.CylinderLubOils;
using hmt_energy_csharp.Engineroom.ExhaustGases;
using hmt_energy_csharp.Engineroom.FOs;
using hmt_energy_csharp.Engineroom.FOSupplyUnits;
using hmt_energy_csharp.Engineroom.LubOilPurifyings;
using hmt_energy_csharp.Engineroom.LubOils;
using hmt_energy_csharp.Engineroom.MainGeneratorSets;
using hmt_energy_csharp.Engineroom.MainSwitchboards;
using hmt_energy_csharp.Engineroom.MERemoteControls;
using hmt_energy_csharp.Engineroom.Miscellaneouses;
using hmt_energy_csharp.Engineroom.ScavengeAirs;
using hmt_energy_csharp.Engineroom.ShaftClutches;
using hmt_energy_csharp.VesselInfos;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace hmt_energy_csharp.EntityFrameworkCore.Oracle
{
    [ConnectionStringName("OracleDefault")]
    public class hmt_energy_csharpOracleDbContext : AbpDbContext<hmt_energy_csharpOracleDbContext>
    {
        public DbSet<VesselInfo> VesselInfos { get; set; }
        public DbSet<Flowmeter> Flowmeters { get; set; }
        public DbSet<Generator> Generators { get; set; }
        public DbSet<LiquidLevel> LiquidLevels { get; set; }
        public DbSet<SternSealing> SternSealings { get; set; }
        public DbSet<SupplyUnit> SupplyUnits { get; set; }
        public DbSet<Battery> Batteries { get; set; }
        public DbSet<Shaft> Shafts { get; set; }
        public DbSet<CIICoefficient> CIICoefficients { get; set; }
        public DbSet<CIIRating> CIIRatings { get; set; }
        public DbSet<FuelCoefficient> FuelCoefficients { get; set; }
        public DbSet<TotalIndicator> TotalIndicators { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<PowerUnit> PowerUnits { get; set; }

        public DbSet<Config> Configs { get; set; }

        public DbSet<CompositeBoiler> CompositeBoilers { get; set; }
        public DbSet<CompressedAirSupply> CompressedAirSupplies { get; set; }
        public DbSet<CoolingFreshWater> CoolingFreshWaters { get; set; }
        public DbSet<CoolingSeaWater> CoolingSeaWaters { get; set; }
        public DbSet<CoolingWater> CoolingWaters { get; set; }
        public DbSet<CylinderLubOil> CylinderLubOils { get; set; }
        public DbSet<ExhaustGas> ExhaustGases { get; set; }
        public DbSet<FO> FOs { get; set; }
        public DbSet<FOSupplyUnit> FOSupplyUnits { get; set; }
        public DbSet<LubOilPurifying> LubOilPurifyings { get; set; }
        public DbSet<LubOil> LubOils { get; set; }
        public DbSet<MainGeneratorSet> MainGeneratorSets { get; set; }
        public DbSet<MainSwitchboard> MainSwitchboards { get; set; }
        public DbSet<MERemoteControl> MERemoteControls { get; set; }
        public DbSet<Miscellaneous> Miscellaneouses { get; set; }
        public DbSet<ScavengeAir> ScavengeAirs { get; set; }
        public DbSet<ShaftClutch> ShaftClutches { get; set; }
        public DbSet<AssistantDecision> AssistantDecisions { get; set; }

        public hmt_energy_csharpOracleDbContext(DbContextOptions<hmt_energy_csharpOracleDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VesselInfo>(b =>
            {
                b.ToTable("vesselinfo", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.SN).IsRequired().HasMaxLength(36);
                b.Property(x => x.Weather).HasMaxLength(100);
                b.Property(x => x.create_time).HasDefaultValueSql("SYSTIMESTAMP");
            });
            modelBuilder.Entity<Flowmeter>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "flowmeter", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.ConsAct).HasPrecision(10, 4);
                b.Property(x => x.ConsAcc).HasPrecision(14, 4);
                b.Property(x => x.Temperature).HasPrecision(6, 2);
                b.Property(x => x.Density).HasPrecision(9, 4);
            });
            modelBuilder.Entity<Generator>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "generator", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.RPM).HasPrecision(6, 0);
                b.Property(x => x.StartPressure).HasPrecision(10, 4);
                b.Property(x => x.ControlPressure).HasPrecision(10, 4);
                b.Property(x => x.ScavengingPressure).HasPrecision(10, 4);
                b.Property(x => x.LubePressure).HasPrecision(10, 4);
                b.Property(x => x.LubeTEMP).HasPrecision(6, 2);
                b.Property(x => x.FuelPressure).HasPrecision(10, 4);
                b.Property(x => x.FuelTEMP).HasPrecision(6, 2);
                b.Property(x => x.FreshWaterPressure).HasPrecision(10, 4);
                b.Property(x => x.FreshWaterTEMPIn).HasPrecision(6, 2);
                b.Property(x => x.FreshWaterTEMPOut).HasPrecision(6, 2);
                b.Property(x => x.CoolingWaterPressure).HasPrecision(10, 4);
                b.Property(x => x.CoolingWaterTEMPIn).HasPrecision(6, 2);
                b.Property(x => x.CoolingWaterTEMPOut).HasPrecision(6, 2);
                b.Property(x => x.CylinderTEMP1).HasPrecision(6, 2);
                b.Property(x => x.CylinderTEMP2).HasPrecision(6, 2);
                b.Property(x => x.CylinderTEMP3).HasPrecision(6, 2);
                b.Property(x => x.CylinderTEMP4).HasPrecision(6, 2);
                b.Property(x => x.CylinderTEMP5).HasPrecision(6, 2);
                b.Property(x => x.CylinderTEMP6).HasPrecision(6, 2);
                b.Property(x => x.SuperchargerTEMPIn).HasPrecision(6, 2);
                b.Property(x => x.SuperchargerTEMPOut).HasPrecision(6, 2);
                b.Property(x => x.ScavengingTEMP).HasPrecision(6, 2);
                b.Property(x => x.BearingTEMP).HasPrecision(6, 2);
                b.Property(x => x.BearingTEMPFront).HasPrecision(6, 2);
                b.Property(x => x.BearingTEMPBack).HasPrecision(6, 2);
                b.Property(x => x.Power).HasPrecision(10, 4);
                b.Property(x => x.WindingTEMPL1).HasPrecision(6, 2);
                b.Property(x => x.WindingTEMPL2).HasPrecision(6, 2);
                b.Property(x => x.WindingTEMPL3).HasPrecision(6, 2);
                b.Property(x => x.VoltageL1L2).HasPrecision(6, 2);
                b.Property(x => x.VoltageL2L3).HasPrecision(6, 2);
                b.Property(x => x.VoltageL1L3).HasPrecision(6, 2);
                b.Property(x => x.FrequencyL1).HasPrecision(8, 0);
                b.Property(x => x.FrequencyL2).HasPrecision(8, 0);
                b.Property(x => x.FrequencyL3).HasPrecision(8, 0);
                b.Property(x => x.CurrentL1).HasPrecision(6, 2);
                b.Property(x => x.CurrentL2).HasPrecision(6, 2);
                b.Property(x => x.CurrentL3).HasPrecision(6, 2);
                b.Property(x => x.ReactivePower).HasPrecision(10, 4);
                b.Property(x => x.PowerFactor).HasPrecision(10, 4);
                b.Property(x => x.LoadRatio).HasPrecision(7, 4);
            });
            modelBuilder.Entity<LiquidLevel>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "liquidlevel", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Level).HasPrecision(6, 4);
                b.Property(x => x.Temperature).HasPrecision(6, 2);
            });
            modelBuilder.Entity<SternSealing>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "sternsealing", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.FrontTEMP).HasPrecision(6, 2);
                b.Property(x => x.BackTEMP).HasPrecision(6, 2);
                b.Property(x => x.BackLeftTEMP).HasPrecision(6, 2);
                b.Property(x => x.BackRightTEMP).HasPrecision(6, 2);
            });
            modelBuilder.Entity<SupplyUnit>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "supplyunit", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Temperature).HasPrecision(6, 2);
                b.Property(x => x.Pressure).HasPrecision(10, 4);
            });
            modelBuilder.Entity<Battery>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "battery", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.SOC).HasPrecision(8, 4);
                b.Property(x => x.SOH).HasPrecision(5, 2);
                b.Property(x => x.MaxTEMP).HasPrecision(10, 4);
                b.Property(x => x.MinTEMP).HasPrecision(10, 4);
                b.Property(x => x.MaxVoltage).HasPrecision(10, 4);
                b.Property(x => x.MinVoltage).HasPrecision(10, 4);
            });
            modelBuilder.Entity<Shaft>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "shaft", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Power).HasPrecision(7, 2);
                b.Property(x => x.RPM).HasPrecision(7, 2);
                b.Property(x => x.Torque).HasPrecision(6, 2);
                b.Property(x => x.Thrust).HasPrecision(7, 2);
            });
            modelBuilder.Entity<CIICoefficient>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "ciicoefficient", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Coefficient1).HasPrecision(16, 0);
                b.Property(x => x.Coefficient2).HasPrecision(6, 4);
                b.Property(x => x.WeightValue).HasPrecision(6, 0);
                b.Property(x => x.LowValue).HasPrecision(6, 0);
                b.Property(x => x.ContainLow).HasPrecision(2, 0);
                b.Property(x => x.HighValue).HasPrecision(6, 0);
                b.Property(x => x.ContainHigh).HasPrecision(2, 0);
                b.Property(x => x.Sort).HasPrecision(4, 0);
            });
            modelBuilder.Entity<CIIRating>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "ciirating", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.RatingValue).HasPrecision(6, 4);
                b.Property(x => x.LowValue).HasPrecision(6, 0);
                b.Property(x => x.ContainLow).HasPrecision(2, 0);
                b.Property(x => x.HighValue).HasPrecision(6, 0);
                b.Property(x => x.ContainHigh).HasPrecision(2, 0);
                b.Property(x => x.Sort).HasPrecision(4, 0);
            });
            modelBuilder.Entity<FuelCoefficient>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "fuelcoefficient", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Value).HasPrecision(6, 4);
                b.Property(x => x.Sort).HasPrecision(4, 0);
            });
            modelBuilder.Entity<TotalIndicator>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "totalindicator", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.DGO).HasPrecision(10, 4);
                b.Property(x => x.LFO).HasPrecision(10, 4);
                b.Property(x => x.HFO).HasPrecision(10, 4);
                b.Property(x => x.LPG_P).HasPrecision(10, 4);
                b.Property(x => x.LPG_B).HasPrecision(10, 4);
                b.Property(x => x.LNG).HasPrecision(10, 4);
                b.Property(x => x.Methanol).HasPrecision(10, 4);
                b.Property(x => x.Ethanol).HasPrecision(10, 4);
                b.Property(x => x.Power).HasPrecision(7, 2);
                b.Property(x => x.Torque).HasPrecision(6, 2);
                b.Property(x => x.Thrust).HasPrecision(7, 2);
            });
            modelBuilder.Entity<Prediction>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "prediction", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.DGO).HasPrecision(10, 4);
                b.Property(x => x.LFO).HasPrecision(10, 4);
                b.Property(x => x.HFO).HasPrecision(10, 4);
                b.Property(x => x.LPG_P).HasPrecision(10, 4);
                b.Property(x => x.LPG_B).HasPrecision(10, 4);
                b.Property(x => x.LNG).HasPrecision(10, 4);
                b.Property(x => x.Methanol).HasPrecision(10, 4);
                b.Property(x => x.Ethanol).HasPrecision(10, 4).HasComment("乙醇");
            });
            modelBuilder.Entity<PowerUnit>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "powerunit", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
            });

            modelBuilder.Entity<Config>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "_" + "config", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
            });

            modelBuilder.Entity<CompositeBoiler>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "compositeboiler", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<CompressedAirSupply>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "compressedairsupply", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<CoolingFreshWater>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "coolingfreshwater", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<CoolingSeaWater>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "coolingseawater", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<CoolingWater>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "coolingwater", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<CylinderLubOil>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "cylinderluboil", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<ExhaustGas>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "exhaustgas", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<FO>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "fo", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<FOSupplyUnit>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "fosupplyunit", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<LubOilPurifying>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "luboilpurifying", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<LubOil>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "luboil", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<MainGeneratorSet>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "maingeneratorset", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<MainSwitchboard>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "mainswitchboard", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<MERemoteControl>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "meremotecontrol", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<Miscellaneous>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "miscellaneous", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<ScavengeAir>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "scavengeair", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<ShaftClutch>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "shaftclutch", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
            modelBuilder.Entity<AssistantDecision>(b =>
            {
                b.ToTable(hmt_energy_csharpConsts.DbTablePrefix1 + "_" + "assistantdecision", hmt_energy_csharpConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Uploaded).HasDefaultValue(0);
            });
        }
    }
}