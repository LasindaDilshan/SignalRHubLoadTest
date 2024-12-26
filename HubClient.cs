using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRHubLoadTest;

public class HubClient
{
    HubConnection hubConnection;
  //  const string hubUrl = "https://localhost:5062/dfn/DfnPriceHub";
   const string hubUrl = "https://v3.dinar-dev-sa.com/dfn/DfnPriceHub";
    string ClientNumber = string.Empty;

    List<string> MessagesReceived;
    string ErrorMessage = string.Empty;

    public bool IsSucces;

    public HubClient (string clientNumber)
    {
        ClientNumber = clientNumber;
        MessagesReceived = [];
        IsSucces = true;
    }


    public async Task EstabilshConnection()
    {
        hubConnection = new HubConnectionBuilder()
       .WithUrl(hubUrl)
       .WithAutomaticReconnect()
       //.with(HttpTransportType.WebSockets, HttpTransportType.ServerSentEvents, HttpTransportType.LongPolling)
       .Build();

        hubConnection.On<string, string>("ReceiveMessage", (groupName, message) =>
        {
            MessagesReceived.Add($"Client : {ClientNumber} | Message received--> {groupName}  | {message}");
          //  Console.WriteLine($"Client : {ClientNumber} | Message received--> {groupName}  | {message}");
        });


        try
        {
            // Start the connection
            hubConnection.StartAsync().Wait();
            Console.WriteLine($"Client : {ClientNumber} |connection started.");
            

            var lst = new List<string> { "2_4150", "2_2010", "2_3008", "8_2222" };

            await hubConnection.SendAsync("JoinGroup", lst);
            Console.WriteLine($"Client : {ClientNumber} | joined Groups.");


        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            IsSucces  = false;
            Console.WriteLine($"Error connecting to SignalR: {ex.Message}");
            await CloseConnection();
        }
    }

    public async Task CloseConnection()
    {
        await hubConnection.StopAsync();
    }

    public string GetMessagesReceived() => $"Client : {ClientNumber} have receieved {MessagesReceived.Count} messsages";
    public string GetErrorMessage() => $"Client : {ClientNumber} has not be able to connect , Error Message :  {ErrorMessage}";

}
