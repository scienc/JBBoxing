using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine;
public class JBBoxingMain : MonoBehaviour {
    private MqttClient mqttClient;
    public System.Action<int> delegatePower;
    public string addressID = "192.168.0.2";
    public int addressPort = 1883;
    private void Awake () {
        //链接服务器  
        mqttClient = new MqttClient (addressID, addressPort, false, null);
        //注册服务器返回信息接受函数  
        mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        //客户端ID  一个字符串  
        mqttClient.Connect ("JBBoxing", "loop", "54240717");
        //监听FPS字段的返回数据  
        mqttClient.Subscribe (new string[] { "/boxing" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
    }

    /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    void OnApplicationQuit () {
        mqttClient.Disconnect ();
    }

    void client_MqttMsgPublishReceived (object sender, MqttMsgPublishEventArgs e) {
        // handle message received  
        //Debug.Log ("返回数据");
        string msg = System.Text.Encoding.Default.GetString (e.Message);
        string[] datas = msg.Split (':');
        if (datas.Length != 2)
            return;
        if (datas[0] == "force") {
            float power = float.Parse (datas[1]);
            Debug.LogWarning (" Power : " + power.ToString ());
            if (delegatePower != null) {
                delegatePower (Mathf.CeilToInt (power * 20));
            }
        }
    }

    private void Update () {
        if (Input.GetKeyDown (KeyCode.Escape)) {
            Application.Quit ();
        }
    }
}