using hmt_energy_csharp;
using hmt_energy_csharp.CollectionDrafts;
using hmt_energy_csharp.CollectionFlowmeters;
using hmt_energy_csharp.CollectionPowers;
using hmt_energy_csharp.Connections;
using hmt_energy_csharp.Sentences;
using hmt_energy_csharp.VdrDpts;
using hmt_energy_csharp.VdrGgas;
using hmt_energy_csharp.VdrGnss;
using hmt_energy_csharp.VdrHdgs;
using hmt_energy_csharp.VdrMwds;
using hmt_energy_csharp.VdrMwvs;
using hmt_energy_csharp.VdrPrcs;
using hmt_energy_csharp.VdrRmcs;
using hmt_energy_csharp.VdrRpms;
using hmt_energy_csharp.VDRs;
using hmt_energy_csharp.VdrTrcs;
using hmt_energy_csharp.VdrTrds;
using hmt_energy_csharp.VdrVbws;
using hmt_energy_csharp.VdrVlws;
using hmt_energy_csharp.VdrVtgs;
using hmt_energy_csharp.VdrXdrs;
using hmt_energy_csharp.WhiteLists;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace hmt_energy_csharp.EntityFrameworkCore.MySql;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class hmt_energy_csharpDbContext :
    AbpDbContext<hmt_energy_csharpDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }

    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    //hmt_energy_csharp
    public DbSet<Connection> Connections { get; set; }

    public DbSet<Sentence> Sentences { get; set; }

    public DbSet<VdrDpt> VdrDpts { get; set; }
    public DbSet<VdrGga> VdrGgas { get; set; }
    public DbSet<VdrGns> VdrGnss { get; set; }
    public DbSet<VdrHdg> VdrHdgs { get; set; }
    public DbSet<VdrMwd> VdrMwds { get; set; }
    public DbSet<VdrMwv> VdrMwvs { get; set; }
    public DbSet<VdrPrc> VdrPrcs { get; set; }
    public DbSet<VdrRmc> VdrRmcs { get; set; }
    public DbSet<VdrRpm> VdrRpms { get; set; }
    public DbSet<VdrTrc> VdrTrcs { get; set; }
    public DbSet<VdrTrd> VdrTrds { get; set; }
    public DbSet<VdrVbw> VdrVbws { get; set; }
    public DbSet<VdrVlw> VdrVlws { get; set; }
    public DbSet<VdrVtg> VdrVtgs { get; set; }
    public DbSet<VdrXdr> VdrXdrs { get; set; }

    public DbSet<CollectionDraft> CollectionDrafts { get; set; }
    public DbSet<CollectionFlowmeter> CollectionFlowmeters { get; set; }
    public DbSet<CollectionPower> CollectionPowers { get; set; }

    public DbSet<VdrTotalEntity> vdrTotalEntities { get; set; }

    public DbSet<WhiteList> WhiteLists { get; set; }

    public DbSet<VdrTimeEntity> vdrTimeEntities { get; set; }

    #endregion Entities from the modules

    public hmt_energy_csharpDbContext(DbContextOptions<hmt_energy_csharpDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(hmt_energy_csharpConsts.DbTablePrefix + "YourEntities", hmt_energy_csharpConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        builder.Entity<Connection>(b =>
        {
            b.ToTable("connection", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.number).HasMaxLength(32);
            b.Property(x => x.host).HasMaxLength(32);
        });
        builder.Entity<Sentence>(b =>
        {
            b.ToTable("sentence", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.data).HasMaxLength(1024);
            b.Property(x => x.category).HasMaxLength(32);
            b.Property(x => x.vdr_id).HasMaxLength(32);
        });

        builder.Entity<VdrDpt>(b =>
        {
            b.ToTable("vdr_dpt", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.depth).HasMaxLength(32);
            b.Property(x => x.offset).HasMaxLength(32);
            b.Property(x => x.mrs).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrGga>(b =>
        {
            b.ToTable("vdr_gga", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.latitude).HasMaxLength(32);
            b.Property(x => x.longitude).HasMaxLength(32);
            b.Property(x => x.satnum).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrGns>(b =>
        {
            b.ToTable("vdr_gns", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.gnsdatetime).HasMaxLength(32);
            b.Property(x => x.latitude).HasMaxLength(32);
            b.Property(x => x.longtitude).HasMaxLength(32);
            b.Property(x => x.satnum).HasMaxLength(32);
            b.Property(x => x.hdop).HasMaxLength(32);
            b.Property(x => x.antennaaltitude).HasMaxLength(32);
            b.Property(x => x.geoidalseparation).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrHdg>(b =>
        {
            b.ToTable("vdr_hdg", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.msh).HasMaxLength(32);
            b.Property(x => x.md).HasMaxLength(32);
            b.Property(x => x.mv).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrMwd>(b =>
        {
            b.ToTable("vdr_mwd", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.tdirection).HasMaxLength(32);
            b.Property(x => x.magdirection).HasMaxLength(32);
            b.Property(x => x.knspeed).HasMaxLength(32);
            b.Property(x => x.speed).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrMwv>(b =>
        {
            b.ToTable("vdr_mwv", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.angle).HasMaxLength(32);
            b.Property(x => x.reference).HasMaxLength(32);
            b.Property(x => x.speed).HasMaxLength(32);
            b.Property(x => x.unit).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrPrc>(b =>
        {
            b.ToTable("vdr_prc", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.leverdemand).HasMaxLength(32);
            b.Property(x => x.leverstatus).HasMaxLength(32);
            b.Property(x => x.rpmdemand).HasMaxLength(32);
            b.Property(x => x.rpmstatus).HasMaxLength(32);
            b.Property(x => x.pitchdemand).HasMaxLength(32);
            b.Property(x => x.pitchstatus).HasMaxLength(32);
            b.Property(x => x.number).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrRmc>(b =>
        {
            b.ToTable("vdr_rmc", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.rmcdatetime).HasMaxLength(32);
            b.Property(x => x.latitude).HasMaxLength(32);
            b.Property(x => x.longtitude).HasMaxLength(32);
            b.Property(x => x.grdspeed).HasMaxLength(32);
            b.Property(x => x.grdcoz).HasMaxLength(32);
            b.Property(x => x.magvar).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrRpm>(b =>
        {
            b.ToTable("vdr_rpm", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.source).HasMaxLength(32);
            b.Property(x => x.number).HasMaxLength(32);
            b.Property(x => x.speed).HasMaxLength(32);
            b.Property(x => x.propellerpitch).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrTrc>(b =>
        {
            b.ToTable("vdr_trc", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.number).HasMaxLength(32);
            b.Property(x => x.rpmdemand).HasMaxLength(32);
            b.Property(x => x.rpmindicator).HasMaxLength(32);
            b.Property(x => x.pitchdemand).HasMaxLength(32);
            b.Property(x => x.pitchindicator).HasMaxLength(32);
            b.Property(x => x.azimuth).HasMaxLength(32);
            b.Property(x => x.oli).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrTrd>(b =>
        {
            b.ToTable("vdr_trd", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.number).HasMaxLength(32);
            b.Property(x => x.rpmresponse).HasMaxLength(32);
            b.Property(x => x.rpmindicator).HasMaxLength(32);
            b.Property(x => x.pitchresponse).HasMaxLength(32);
            b.Property(x => x.pitchindicator).HasMaxLength(32);
            b.Property(x => x.azimuth).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrVbw>(b =>
        {
            b.ToTable("vdr_vbw", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.lngwatspd).HasMaxLength(32);
            b.Property(x => x.tvswatspd).HasMaxLength(32);
            b.Property(x => x.watspdstatus).HasMaxLength(32);
            b.Property(x => x.lnggrdspd).HasMaxLength(32);
            b.Property(x => x.tvsgrdspd).HasMaxLength(32);
            b.Property(x => x.grdspdstatus).HasMaxLength(32);
            b.Property(x => x.tvswatspdstern).HasMaxLength(32);
            b.Property(x => x.watspdstatusstern).HasMaxLength(32);
            b.Property(x => x.tvsgrdspdstern).HasMaxLength(32);
            b.Property(x => x.grdspdstatusstern).HasMaxLength(32);
            b.Property(x => x.watspd).HasMaxLength(32);
            b.Property(x => x.grdspd).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrVlw>(b =>
        {
            b.ToTable("vdr_vlw", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.watdistotal).HasMaxLength(32);
            b.Property(x => x.watdisreset).HasMaxLength(32);
            b.Property(x => x.grddistotal).HasMaxLength(32);
            b.Property(x => x.grddisreset).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrVtg>(b =>
        {
            b.ToTable("vdr_vtg", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.grdcoztrue).HasMaxLength(32);
            b.Property(x => x.grdcozmag).HasMaxLength(32);
            b.Property(x => x.grdspdknot).HasMaxLength(32);
            b.Property(x => x.grdspdkm).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<VdrXdr>(b =>
        {
            b.ToTable("vdr_xdr", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.sensortype).HasMaxLength(32);
            b.Property(x => x.sensorvalue).HasMaxLength(32);
            b.Property(x => x.sensorunit).HasMaxLength(32);
            b.Property(x => x.sensorid).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });

        builder.Entity<CollectionDraft>(b =>
        {
            b.ToTable("collection_draft", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.bow).HasMaxLength(32);
            b.Property(x => x.stern).HasMaxLength(32);
            b.Property(x => x.port).HasMaxLength(32);
            b.Property(x => x.starboard).HasMaxLength(32);
            b.Property(x => x.trim).HasMaxLength(32);
            b.Property(x => x.heel).HasMaxLength(32);
            b.Property(x => x.draft).HasMaxLength(32);
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<CollectionFlowmeter>(b =>
        {
            b.ToTable("collection_flowmeter", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.fuelconsume).HasMaxLength(32);
            b.Property(x => x.fueltype).HasMaxLength(32);
            b.Property(x => x.fuelconsumeaccumulative).HasMaxLength(32);
            b.Property(x => x.devicetype).HasMaxLength(32);
            b.Property(x => x.deviceno).HasMaxLength(32);
            b.Property(x => x.fcpernm).HasMaxLength(32);
            b.Property(x => x.fcperpow).HasMaxLength(32);
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });
        builder.Entity<CollectionPower>(b =>
        {
            b.ToTable("collection_power", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.rpm).HasMaxLength(32);
            b.Property(x => x.torque).HasMaxLength(32);
            b.Property(x => x.power).HasMaxLength(32);
            b.Property(x => x.slip).HasMaxLength(32);
            b.Property(x => x.type).HasMaxLength(32);
            b.Property(x => x.sentenceid).HasMaxLength(64);
        });

        builder.Entity<VdrTotalEntity>(b =>
        {
            b.ToView("vw_vdr_total", hmt_energy_csharpConsts.DbSchema);
            b.HasNoKey();
        });

        builder.Entity<WhiteList>(b =>
        {
            b.ToTable("WhiteList", hmt_energy_csharpConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TargetId).IsRequired().HasMaxLength(32);
            b.Property(x => x.TargetIp).IsRequired().HasMaxLength(32);
        });
    }
}