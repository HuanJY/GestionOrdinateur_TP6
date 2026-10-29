var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();   
builder.Services.AddSwaggerGen();    


builder.Services.AddHttpClient("GestionOrdinateursApi", client =>
{
    client.BaseAddress = new Uri("http://gestionordinateurs-api:5102/api/");
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5122", "http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowBlazorClient");

//interface de test sur /swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// demare l'API
app.Run();