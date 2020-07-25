#-*-coding:utf-8-*- 
import paho.mqtt.client as mqtt
# import paho.mqtt.publish as publish
import struct,binascii
import random,time,json


# HOST = "127.0.0.1"
HOST = "olive.fm"
PORT = 1883
SUBSCRIBE_TOPIC = "/boxing"
PUBLISH_TOPIC = "/boxing"

def on_connect(client, userdata, flags, rc):
    print('连接到MQTT服务器,返回码:{}'.format(str(rc)))
    # client.subscribe(SUBSCRIBE_TOPIC)
    # print('订阅Topic：{}'.format(SUBSCRIBE_TOPIC))

def on_message(client, userdata, msg):
    print('[{}] {}'.format(msg.topic,msg.payload.decode("utf-8")))

if __name__ == '__main__':
    client_id = time.strftime('TRIGGER-mockup')
    client = mqtt.Client(client_id)    # ClientId不能重复，所以使用当前时间
    client.username_pw_set("loop", "54240717")  # 必须设置，否则会返回「Connected with result code 4」
    client.on_connect = on_connect
    client.on_message = on_message
    client.connect(HOST, PORT, 60)
    client.loop_start()
    num = 0 
    while True:
        if num < 100:
            time.sleep(random.randrange(0,1))
            frorce = random.random()+1
            bmsg =  str.encode('force:{}'.format(frorce))
            client.publish(PUBLISH_TOPIC, bmsg, qos=0, retain=False)  # 发布消息
            print('发送消息{}'.format(bmsg))
            num += 1
        else:
            time.sleep(10)
            num = 0
        