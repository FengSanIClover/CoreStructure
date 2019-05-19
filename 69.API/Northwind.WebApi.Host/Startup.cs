using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Northwind.Entities.Models.PublicCloud;
using TP5.ModuleResolver.NetCore;
using Unity;
using Unity.Lifetime;
using URF.Core.Abstractions;
using URF.Core.EF;

namespace Northwind.WebApi.Host
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        public static IUnityContainer Container { get; set; } 
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddControllersAsServices();
            services.AddHttpContextAccessor();
            services.AddCors();
            services.AddMvc();
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            //需安裝 Microsoft.AspNetCore.Mvc.WebApiCompatShim
            services.AddMvc().AddWebApiConventions();
            // appsettings.json內設定的
            var connectionString = this.configuration.GetConnectionString("NorthwindContext");

            #region 分頁設定
            //SQL Server 2008以上的版本
            //services.AddDbContext<InsuranceOnPublicCloudContext>(options => options.UseSqlServer(connectionString));

            //SQL Server 2008以下的版本，分頁得加 UseRowNumberForPaging
            //services.AddDbContext<NorthwindContext>(options =>
            //{
            //    options.UseSqlServer(connectionString, opt =>
            //    {
            //        opt.UseRowNumberForPaging();
            //    });
            //});
            #endregion

            #region 加入swagger
            // Swagger 產生器是負責取得 API 的規格並產生 SwaggerDocument 物件。
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc
            //    (
            //    // name: 攸關 SwaggerDocument 的 URL 位置。
            //        name: "v1",
            //        // info: 是用於 SwaggerDocument 版本資訊的顯示(內容非必填)。
            //        info: new Info
            //        {
            //            Title = "RESTful API",
            //            Version = "1.0.0",
            //            Description = "This is ASP.NET Core RESTful API Sample.",
            //            TermsOfService = "None",
            //            Contact = new Contact
            //            {
            //                Name = "John Wu",
            //                Url = "https://blog.johnwu.cc"
            //            },
            //            License = new License
            //            {
            //                Name = "CC BY-NC-SA 4.0",
            //                Url = "https://creativecommons.org/licenses/by-nc-sa/4.0/"
            //            }
            //        }
            //     );

            //    var filePath = Path.Combine(AppContext.BaseDirectory, "Api.xml");
            //    c.IncludeXmlComments(filePath);

            //    //filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Api.xml");
            //    //c.IncludeXmlComments(filePath);
            //    var appRoot = AppContext.BaseDirectory.Replace("\\69.API\\ThinkPower.WebApi.Host\\bin\\Debug\\netcoreapp2.2", "");
            //    //AppContext.BaseDirectory == "C:\\Users\\User\\Desktop\\Candel\\ThinkPowerPractice1\\69.API\\ThinkPower.WebApi.Host\\bin\\Debug\\netcoreapp2.2\\"
            //    // #if Debug
            //    filePath = $"{appRoot}00.Library\\TP5.Core.NetCore\\bin\\Debug\\netcoreapp2.2\\Api.xml";
            //    c.IncludeXmlComments(filePath);

            //    filePath = $"{appRoot}62.Addons\\ThinkPower.SystemManagement\\ThinkPower.SystemManagement.WebApi\\bin\\Debug\\netcoreapp2.2\\Api.xml";
            //    c.IncludeXmlComments(filePath);

            //    filePath = $"{appRoot}12.Entities\\ThinkPower.Entities\\bin\\Debug\\netcoreapp2.2\\Api.xml";
            //    c.IncludeXmlComments(filePath);
            //    // #endif
            //});
            #endregion

            #region 加入JWT
            //// configure strongly typed settings objects
            //var appSettingsSection = _configure.GetSection("AppSettings");
            //services.Configure<AppSettings>(appSettingsSection);
            //// configure jwt authentication
            //var appSettings = appSettingsSection.Get<AppSettings>();
            //var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            //services.AddAuthentication(x =>
            //{   //安裝Microsoft.AspNetCore.Authentication.JwtBearer
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(x =>
            //{
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = true;
            //    //安裝Microsoft.IdentityModel.Tokens
            //    x.TokenValidationParameters = new TokenValidationParameters()
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = false,
            //        ValidateAudience = false
            //    };
            //});
            //services.AddScoped<IUserService, UserService>();
            #endregion

            services.AddScoped<DbContext, NorthwindContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //services.AddMvc().AddApplicationPart(Assembly.Load(new AssemblyName("")));

            #region 整合Unity 並回傳
            //整合Unity 參考網址：https://csharpkh.blogspot.com/2018/11/ASP-NET-Core-Dependency-Injection-IoC-Container-Container-Unity.html

            Unity.Microsoft.DependencyInjection.ServiceProvider serviceProvider = Unity.Microsoft.DependencyInjection.ServiceProvider.ConfigureServices(services) as Unity.Microsoft.DependencyInjection.ServiceProvider;

            this.ConfigureContainer(Container = (UnityContainer)serviceProvider);

            return serviceProvider;
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }


        // 1.
        public void ConfigureContainer(IUnityContainer container)
        {
            // Could be used to register more types
            // container.RegisterType<IMyService, MyService>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            // IMapper mapper = mappingConfig.CreateMapper();
            container.RegisterInstance<IMapper>(mappingConfig.CreateMapper());
            container.RegisterType<DbContext, NorthwindContext>("NorthwindContext", new PerResolveLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>(new PerResolveLifetimeManager());
            // 透過 ModuleLoader 載入各模組(Addons)的 Unity RegisterType 設定。
            ModuleLoader.LoadContainer(container, ".\\bin\\Debug\\netcoreapp2.2", "Northwind.*.dll");
            //ModuleLoader.LoadContainer(container, ".\\bin\\Debug\\netcoreapp2.2", "ThinkPower.SystemManagement.Interface.dll");
            //ModuleLoader.LoadContainer(container, ".\\bin\\Debug\\netcoreapp2.2", "ThinkPower.SystemManagement.WebApi.dll");
        }
    }
}
