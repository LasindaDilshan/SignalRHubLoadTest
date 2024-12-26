using SignalRHubLoadTest;


int totalClients = 500;
int batchSize = 100; // Number of clients to connect per batch
int delayBetweenBatches = 1000; // Delay between each batch (in milliseconds)
int maxConnectionsPerSecond = 20; // Maximum number of connections per second
int maxConnectionsIn10Seconds = 50; // Maximum number of connections in 10 seconds

// Create all clients
var allClients = Enumerable.Range(1, totalClients)
                           .Select(x => new HubClient(x.ToString()))
                           .ToList();

// The task to establish connections for all clients
var tasks = new List<Task>();

int clientsConnected = 0;

while (clientsConnected < totalClients)
{
    // Calculate how many clients to connect in this batch
    int clientsToConnect = Math.Min(batchSize, totalClients - clientsConnected);
    var batchClients = allClients.Skip(clientsConnected).Take(clientsToConnect).ToList();

    // Start all connections in parallel for this batch
    var connectionTasks = batchClients.Select(client => EstablishConnectionWithDelay(client)).ToList();

    // Wait for the batch to complete
    await Task.WhenAll(connectionTasks);

    // Increment the number of clients connected
    clientsConnected += clientsToConnect;

    Console.WriteLine($"Connected {clientsConnected} clients.");

    // Introduce delay between batches to respect server's connection rules
    await Task.Delay(delayBetweenBatches);
}

Console.WriteLine("All connections established.");

var Success = allClients.Where(c => c.IsSucces).Count();
var Failed = allClients.Where(c => !c.IsSucces).Count();

Console.WriteLine($"Total number of clients {totalClients} | Success : {Success}  | Failed : {Failed}  to connect.");

Console.ReadLine(); 


// Establish connection with a delay to simulate rate limiting
static async Task EstablishConnectionWithDelay(HubClient client)
{
    // Add a small delay to avoid overloading the server (for example, simulate max 20 connections/sec)
    await Task.Delay(50); // 50 ms delay per client, so 20 clients can be connected per second

    await client.EstabilshConnection();
}

//int userCount = 500;

//var clients1 = Enumerable.Range(1, userCount).Select(x => new HubClient(x.ToString())).ToList();
//var clients2 = Enumerable.Range(1, userCount).Select(x => new HubClient(x.ToString())).ToList();
//var clients3 = Enumerable.Range(1, userCount).Select(x => new HubClient(x.ToString())).ToList();
//var clients4 = Enumerable.Range(1, userCount).Select(x => new HubClient(x.ToString())).ToList();
//var clients5 = Enumerable.Range(1, userCount).Select(x => new HubClient(x.ToString())).ToList();
//var clients6 = Enumerable.Range(1, userCount).Select(x => new HubClient(x.ToString())).ToList();


////foreach (var client in clients)
////{
////    await client.EstabilshConnection();
////}

//Parallel.ForEach(clients1, async (client) =>
//{
//    await client.EstabilshConnection();
//});

//Console.ReadLine();



//foreach (var item in clients)
//{
//    await item.CloseConnection();
//    if (item.IsSucces)
//    {
//        Console.WriteLine(item.GetMessagesReceived());
//    }
//    else
//        Console.WriteLine(item.GetErrorMessage());
//}


//Console.ReadLine();

//string hubUrl = "https://localhost:5062/dfn/DfnPriceHub";

//// Create a HubConnectionBuilder instance
//var hubConnection = new HubConnectionBuilder()
//    .WithUrl(hubUrl) // Corrected line
//    .Build();


//// Register a handler for messages from the SignalR hub
//// "ReceiveStockPrice" is the topic to which SignalR sending the singnals
//hubConnection.On<string,string>("ReceiveMessage", (groupName,message) =>
//{
//    Console.WriteLine($"Message received--> {groupName}  | {message}");
//});

//try
//{
//    // Start the connection
//    hubConnection.StartAsync().Wait();
//    Console.WriteLine("SignalR connection started.");

//    var lst = new List<string> { "2_4150", "2_2010", "2_3008", "8_2222" };

//    await hubConnection.SendAsync("JoinGroup", lst);
//    Console.WriteLine("SignalR joined Groups.");


//}
//catch (Exception ex)
//{
//    Console.WriteLine($"Error connecting to SignalR: {ex.Message}");
//    throw;
//}

////Create a cancellation token to stop the connection
//CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
////hubConnection.StopAsync().Wait();
//var cancellationToken = cancellationTokenSource.Token;
//// Handle Ctrl+C to gracefully shut down the application
//Console.CancelKeyPress += (sender, a) =>
//{
//    a.Cancel = true;
//    Console.WriteLine("Stopping SignalR connection...");
//    cancellationTokenSource.Cancel();
//};

//try
//{
//    // Keep the application running until it is cancelled
//    await Task.Delay(Timeout.Infinite, cancellationToken);
//}
//catch (TaskCanceledException)
//{
//}

//// Stop the connection gracefully
//await hubConnection.StopAsync();

//Console.WriteLine("SignalR connection closed.");