using Microsoft.EntityFrameworkCore;
using TransactionReportingAPI.Data;
using AutoMapper;
using TransactionReportingAPI.Models;
using TransactionReportingAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<TransactionProcessingContext>(
    option 
    => { 
        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")); 
    }
    );
builder.Services.Configure<GrpcServer>(builder.Configuration.GetSection("GrpcServer"));
builder.Services.AddScoped<ITransactions, Transactions>();
//builder.Services.AddAutoMapper(typeof(MappingConfig));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
