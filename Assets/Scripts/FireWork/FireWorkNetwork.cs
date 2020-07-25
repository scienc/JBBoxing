using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine;
public class FireWorkNetwork : MonoBehaviour {
    private MqttClient mqttClient;
    public System.Action<Vector3> delegatePos;
    public string addressID = "192.168.0.1";
    public int addressPort = 1883;
    public string userName = "";
    public string password = "";
    private void Awake () {
        //链接服务器  
        mqttClient = new MqttClient (addressID, addressPort, false, null);
        //注册服务器返回信息接受函数  
        mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        //客户端ID  一个字符串  
        mqttClient.Connect ("FireWork", userName, password);
        //监听FPS字段的返回数据  
        mqttClient.Subscribe (new string[] { "/boxing" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        Debug.Log (mqttClient.IsConnected);
    }

    /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    void OnApplicationQuit () {
        mqttClient.Disconnect ();
    }

    void client_MqttMsgPublishReceived (object sender, MqttMsgPublishEventArgs e) {
        string msg = System.Text.Encoding.Default.GetString (e.Message);
        string[] datas = msg.Split (':');
        if (datas.Length != 2)
            return;
        if (datas[0] == "pos") {
            string[] datas2 = datas[1].Split (',');
            if (datas2.Length == 2) {
                Vector3 pos = new Vector3 (float.Parse (datas2[0]), float.Parse (datas2[1]));
                Debug.Log ("Receive Pos" + pos);
                if (delegatePos != null) {
                    delegatePos (pos);
                }
            } else {
                Debug.LogWarning ("Pos is error " + msg);
            }
        }
    }
}