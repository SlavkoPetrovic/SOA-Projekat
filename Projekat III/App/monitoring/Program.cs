using System.Text;
using monitoring.Limit;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Limits>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var mqttFactory = new MqttFactory();
var mqttClient = mqttFactory.CreateMqttClient();
var options = new MqttClientOptionsBuilder()
    .WithTcpServer("broker.emqx.io", 1883)
    .WithCleanSession()
    .Build();

mqttClient.ConnectedAsync += async (e) =>
{
    var topicFilter = new MqttTopicFilterBuilder()
    .WithTopic("projekatIII")
    .Build();

    await mqttClient.SubscribeAsync(topicFilter);
};

mqttClient.ApplicationMessageReceivedAsync += async (e) =>
{
    var limits = app.Services.GetRequiredService<Limits>();
    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

    dynamic toJson = JObject.Parse(payload);
    var name = toJson.readings[0].name.ToString();
    var value = toJson.readings[0].value;


    byte[] bytes = Convert.FromBase64String(value.ToString());
    Array.Reverse(bytes, 0, 8);
    var newValue = BitConverter.ToDouble(bytes);
    float floatValue = (float)newValue;

    string color = "green";

    //Slanje komande
    if (floatValue > limits.valueLimits[name][1])
    {
        color = "blue";
    }
    else if (floatValue < limits.valueLimits[name][0])
    {
        color = "red";
    }
    Console.WriteLine(color);


    using (var httpClient = new HttpClient())
    {
        var paramss = new
        {
            color = color,
            parameterName = name
        };
        var c = JsonConvert.SerializeObject(paramss);
        StringContent content = new StringContent(c, Encoding.UTF8, "application/json");
        using (var response = await httpClient.PutAsync("http://host.docker.internal:48082/api/v1/device/e77135b3-c62f-4829-a172-2f0a47ad577e/command/18354ec7-9870-4d7a-941b-4a732a581b7d", content))
        {
        }
    }
};

await mqttClient.ConnectAsync(options, CancellationToken.None);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
