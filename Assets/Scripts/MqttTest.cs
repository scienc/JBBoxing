using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine;

[System.Serializable]
public class DeviceInfoJson {
	public int Code;
	public int Data;
}

[System.Serializable]
public class JsonUserInfo {
	public string Uid;
	public int Code;
	public int Data;
}

public class MqttTest : MonoBehaviour {

	private MqttClient mqttClient = null;

	public void SendMqtt () {
		// 加载证书
		var cert = Resources.Load ("cacert") as TextAsset;
		// 使用TLS证书连接
		mqttClient = new MqttClient ("127.0.0.1", 3563, false, new X509Certificate (cert.bytes), new RemoteCertificateValidationCallback (
			// 测试服务器未设置公钥证书，返回true即跳过检查，直接通过，否则抛出服务器证书无效Error
			(srvPoint, certificate, chain, errors) => true
		));
		// 消息接收事件
		mqttClient.MqttMsgPublishReceived += msgReceived;
		// 连接
		mqttClient.Connect ("test1", "admin", "password");

		var joinReg = new DeviceInfoJson ();
		joinReg.Code = 3;
		joinReg.Data = 100;

		// 发送登录消息
		//mqttClient.Publish("Login/HD_Login/2", Encoding.UTF8.GetBytes("{\"Did\": \"username\",\"passWord\": \"Hello,anyone!\"}")); 
		mqttClient.Publish ("Login/HD_Login/2", Encoding.UTF8.GetBytes (JsonUtility.ToJson (joinReg)));

		//join the chat
		mqttClient.Publish ("Chat/HD_JoinChat/1", Encoding.UTF8.GetBytes ("{\"roomName\": \"room1\"}"));

	}

	void msgReceived (object sender, MqttMsgPublishEventArgs e) {
		Debug.Log ("服务器返回数据");
		string msg = System.Text.Encoding.Default.GetString (e.Message);
		Debug.Log (msg + "topic : " + e.Topic);

		if (e.Topic.CompareTo ("Chat/OnPut") == 0) {
			var info = JsonUtility.FromJson<JsonUserInfo> (msg);
			if (info != null)
				Debug.Log ("info is " + info.Uid);
		}

	}

	public void OnDisable () {
		if (mqttClient != null && mqttClient.IsConnected)
			mqttClient.Disconnect ();
	}

}