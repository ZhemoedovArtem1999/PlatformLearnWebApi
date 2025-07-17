
using DataAccessLayer.Dependency;
using GrpcContracts;

namespace PlatformLearnWebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .WithExposedHeaders("Grpc-Status", "Grpc-Message"); ;
                });
            });


            //var assemblyAuthorizationPath = Path.Combine(AppContext.BaseDirectory, "Authentication.dll");
            //var assemblyAuthorizaion = Assembly.LoadFrom(assemblyAuthorizationPath);

            //builder.Services.AddControllers().AddApplicationPart(assemblyAuthorizaion);
           
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers();

            // ���������� ��������������� �����
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = "PlatformLearn.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(60);
            });

            builder.Services.DependencyInjectionDataAccessLayer(builder.Configuration);

            builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(options =>
            {
                options.Address = new Uri("http://localhost:5777"); // TODO: �������� �� ������������
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseRouting();


            app.UseCors("AllowAllOrigins");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();


            app.MapControllers();


            app.UseHttpsRedirection();

            await app.RunAsync();
        }
    }
}
