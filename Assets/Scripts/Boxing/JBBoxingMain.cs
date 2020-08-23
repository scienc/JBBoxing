using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine;

public class PowerData
{
    public float value;
    public int direction;
}

public class JBBoxingMain : MonoBehaviour
{
    private MqttClient mqttClientLeft;
    private MqttClient mqttClientRight;
    public System.Action<PowerData> delegatePower;
    public string addressID = "192.168.0.2";
    public int addressPort = 1883;

    public float powerOffset = 20;
    private void Awake()
    {
        //链接服务器  
        mqttClientLeft = new MqttClient(addressID, addressPort, false, null);
        //注册服务器返回信息接受函数  
        mqttClientLeft.MqttMsgPublishReceived += client_MqttMsgPublishReceivedLeft;
        //客户端ID  一个字符串  
        mqttClientLeft.Connect("/boxing", "loop", "54240717");
        //监听FPS字段的返回数据  
        mqttClientLeft.Subscribe(new string[] { "/boxing" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

        // mqttClientRight = new MqttClient(addressID, addressPort, false, null);
        // //注册服务器返回信息接受函数  
        // mqttClientRight.MqttMsgPublishReceived += client_MqttMsgPublishReceivedRight;
        // //客户端ID  一个字符串  
        // mqttClientRight.Connect("TRIGGER1B", "loop", "54240717");
        // //监听FPS字段的返回数据  
        // mqttClientRight.Subscribe(new string[] { "/boxing" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
    }

    /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    void OnApplicationQuit()
    {
        mqttClientLeft.Disconnect();
        mqttClientRight.Disconnect();
    }

    void client_MqttMsgPublishReceivedLeft(object sender, MqttMsgPublishEventArgs e)
    {
        // handle message received  
        //Debug.Log ("返回数据");
        string msg = System.Text.Encoding.Default.GetString(e.Message);
        string[] datas = msg.Split(':');
        if (datas.Length != 2)
            return;
        if (datas[0] == "force")
        {
            float power = float.Parse(datas[1]);
            Debug.LogWarning(" Power : " + power.ToString());
            if (delegatePower != null)
            {
                PowerData data = new PowerData();
                data.value = power * powerOffset;
                data.direction = 0;
                delegatePower(data);
            }
        }
    }
    void client_MqttMsgPublishReceivedRight(object sender, MqttMsgPublishEventArgs e)
    {
        // handle message received  
        //Debug.Log ("返回数据");
        string msg = System.Text.Encoding.Default.GetString(e.Message);
        string[] datas = msg.Split(':');
        if (datas.Length != 2)
            return;
        if (datas[0] == "force")
        {
            float power = float.Parse(datas[1]);
            Debug.LogWarning(" Power : " + power.ToString());
            if (delegatePower != null)
            {
                PowerData data = new PowerData();
                data.value = power * powerOffset;
                data.direction = 1;
                delegatePower(data);
            }
        }
    }
}