using LocalJsonModule.Data;
using LocalJsonModule.Repositories;
using LocalJsonModule.Repositories.postgresSQL;
using LocalJsonModule.Services;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<IDBConnectionFactory>(new DbConnectionFactory(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:63342")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDataPathProvider, DataPathProvider>();

builder.Services.AddScoped<IUserRepository, UserPostgresRepository>();
builder.Services.AddScoped<IFolderRepository, FolderPostgresRepository>();
builder.Services.AddScoped<INoteRepository, NotePostgresRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUpdateService, UpdateService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();

app.Run();
