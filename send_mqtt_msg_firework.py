#-*-coding:utf-8-*- 
import paho.mqtt.client as mqtt
#import paho.mqtt.publish as publish
import struct,binascii
import random,time,json


#HOST = "127.0.0.1"
HOST = "olive.fm"
PORT = 1883
SUBSCRIBE_TOPIC = "/boxing"
PUBLISH_TOPIC = "/boxing"

def on_connect(client, userdata, flags, rc):
    print('connect MQTT server,Result Code:{}'.format(str(rc)))
    #client.subscribe(SUBSCRIBE_TOPIC)
    #print('订阅Topic：{}'.format(SUBSCRIBE_TOPIC))

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
    while True:        
            time.sleep(random.randrange(0,1))
            posX = random.randint(100,1820)
            posY = random.randint(100,9080)
            bmsg =  str.encode('pos:{},{}'.format(posX,posY))
            client.publish(PUBLISH_TOPIC, bmsg, qos=0, retain=False)  # 发布消息
            print('Send Message{}'.format(bmsg))        
            time.sleep(2)
        