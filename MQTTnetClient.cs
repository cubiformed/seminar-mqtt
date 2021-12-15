using System;
using MQTTnet;
using UniRx;

public class MQTTnetClient: IClient {

  private readonly Subject<bool> _connectionStatusSubject = new Subject<bool>();
  public IObservable<bool> GetConnectionStatus() => _connectionStatusSubject.AsObservable();
  
  private readonly Subject<string> _messageSubject = new Subject<string>();
  public IObservable<string> GetMessages() => _messageSubject.AsObservable();

  private readonly IMqttclient _client;

  public MQTTnetClient() 
  {
    _client = new MqttFactory().CreateMqttClient();

    _client.UseConnectedHandler(_ => {
      _connectionStatusSubject.OnNext(true);
    });
    _client.UseDisconnectedHandler(_ => {
      _connectionStatusSubject.OnNext(true);
    });

    _client.UseApplicationMessageReceivedHandler(args => {
      var message = BitConverter.ToString(args.ApplicationMessage.Payload);
      _messageSubject.OnNext(message);
    });
  }

  public void Connect(string ip) 
  {
    try 
    {
      var options = new MqttclientOptionsBuilder().WithTcpServer(ip).Build();
      _client.ConnectAsync(options).Wait();
      _connectionStatusSubject.OnNext(true);
    }
    catch(Exception exception)
    {
      _connectionStatusSubject.OnError(exception);
    }
  }
  public void Disonnect() 
  {
    try 
    {
      _client.DisonnectAsync(options).Wait();
      _connectionStatusSubject.OnNext(false);
    }
    catch(Exception exception)
    {
      _connectionStatusSubject.OnError(exception);
    }
  }

  public void Subscribe(string topic) 
  {
    try 
    {
      _client?.SubscribeAsync(topic).Wait();
    }
    catch(Exception exception) 
    {
      _messageSubject.OnError(exception);
    }
  }

  public void Unsubscribe(string topic) 
  {
    try 
    {
      _client?.UnsubscribeAsync(topic).Wait();
    }
    catch(Exception exception) 
    {
      _messageSubject.OnError(exception);
    }
  }

  public void Dispose()
  {
    _client?.Dispose();
    _connectionStatusSubject?.Dispose();
    _messageSubject?.Dispose();
  }
}