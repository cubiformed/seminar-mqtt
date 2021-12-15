using System;

public Interface IClient: IDisposable
{
  public IObservable<bool> GetConnectionStatus();
  public IObservable<string> GetMessages();

  public void Connect(string ip);
  public void Disconnect();

  public void Subscribe(string topic);
  public void Unsubscribe(string topic);
}
