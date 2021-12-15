public class ConnectionManager: MonoBehaviour
{
  [SerializeField]
  private string ip;

  [SerializeField]
  private string topic;

  [SerializeField]
  private Text output;

  private IClient _client;

  private void Start() 
  {
    _client = new MQTTnetClient();

    _client.Connect(ip);

    _client.Subscribe(topic);

    _client.GetMessages().ObserveOnMainThread().DoOnError(error => {
      Debug.LogError(error.Message);
    }).Subscribe(data => {
      output.text = data;
    });
  }

  private void OnDestroy() 
  {
    _client.Disconnect(); 
    _client.Dispose(); 
  }
}